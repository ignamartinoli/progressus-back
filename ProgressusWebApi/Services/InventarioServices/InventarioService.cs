using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProgressusWebApi.DataContext;
using ProgressusWebApi.Dtos.InventarioDtos;
using ProgressusWebApi.Services.InventarioServices.Interfaces;
using ProgressusWebApi.Models.InventarioModels;
using ProgressusWebApi.Models.NotificacionModel;
using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Services.AuthServices.Interfaces;
using ProgressusWebApi.Services.AuthServices;

namespace ProgressusWebApi.Services.InventarioServices
{
    public class InventarioService : IInventarioService
    {
        private readonly ProgressusDataContext _context;
        private readonly IAuthService _authService;

        public InventarioService(ProgressusDataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;

        }

        public async Task<IActionResult> CrearInventarioAsync(InventarioDtos inventarioDto)
        {
            var inventario = new Inventario
            {
                Nombre = inventarioDto.Nombre,
                Descripcion = inventarioDto.Descripcion,
                Estado = inventarioDto.Estado
            };

            _context.Inventario.Add(inventario);
            await _context.SaveChangesAsync();

            return new OkObjectResult(inventario);
        }

        public async Task<IActionResult> ObtenerTodosInventariosAsync()
        {
            var inventarios = await _context.Inventario.ToListAsync();
            return new OkObjectResult(inventarios);
        }

        public async Task<IActionResult> ObtenerInventarioPorIdAsync(string id)
        {
            var inventario = await _context.Inventario.FindAsync(int.Parse(id));
            if (inventario == null)
            {
                return new NotFoundObjectResult("Inventario no encontrado.");
            }

            return new OkObjectResult(inventario);
        }

        public async Task<IActionResult> EditarInventarioAsync(string id, InventarioDtos inventarioDto)
        {
            var inventario = await _context.Inventario.FindAsync(int.Parse(id));
            if (inventario == null)
            {
                return new NotFoundObjectResult("Inventario no encontrado.");
            }

            // Verificar si el estado cambia a "En Reparación/Mantenimiento"
            bool estadoCambioAMantenimiento = inventario.Estado != "En Reparación/Mantenimiento"
                                              && inventarioDto.Estado == "En Reparación/Mantenimiento";

            inventario.Nombre = inventarioDto.Nombre;
            inventario.Descripcion = inventarioDto.Descripcion;
            inventario.Estado = inventarioDto.Estado;

            _context.Inventario.Update(inventario);
            await _context.SaveChangesAsync();

            // Crear notificaciones si el estado cambió a "En Reparación/Mantenimiento"
            if (estadoCambioAMantenimiento)
            {
                var usuariosEntrenadores = await _authService.ObtenerUsuariosEntrenadoresAsync();
                if (usuariosEntrenadores == null || !usuariosEntrenadores.Any())
                {
                    return new NotFoundObjectResult("No se encontraron usuarios con el rol de Entrenador para notificar.");
                }

                foreach (var usuario in usuariosEntrenadores)
                {
                    var notificacion = new Notificacion
                    {
                        UsuarioId = usuario.IdentityUserId,
                        Mensaje = $"El objeto de inventario '{inventario.Nombre}' id: '{inventario.Id}' ha cambiado su estado a 'En Reparación/Mantenimiento'.",
                        Estado = false
                    };

                    _context.Notificaciones.Add(notificacion);
                }

                await _context.SaveChangesAsync();
            }

            // **Verificar si todas las máquinas con el mismo nombre están en reparación**
            var maquinasMismoNombre = await _context.Inventario
                .Where(m => m.Nombre == inventario.Nombre)
                .ToListAsync();

            bool todasEnReparacion = maquinasMismoNombre.All(m => m.Estado == "En Reparación/Mantenimiento");

            if (todasEnReparacion)
            {
                // **Actualizar MaquinaEnReparacion en los ejercicios**
                var ejerciciosAsociados = await _context.Ejercicios
                    .Where(e => e.NombreMaquina == inventario.Nombre)
                    .ToListAsync();

                foreach (var ejercicio in ejerciciosAsociados)
                {
                    ejercicio.MaquinaEnReparacion = true;
                }

                _context.Ejercicios.UpdateRange(ejerciciosAsociados);
                await _context.SaveChangesAsync();
            }

            return new OkObjectResult(inventario);
        }


        public async Task<IActionResult> EliminarInventarioAsync(string id)
        {
            // Convertir id de string a int
            if (!int.TryParse(id, out int parsedId))
            {
                return new BadRequestObjectResult("El id proporcionado no es válido.");
            }

            var inventario = await _context.Inventario.FindAsync(parsedId);
            if (inventario == null)
            {
                return new NotFoundObjectResult("Inventario no encontrado.");
            }

            _context.Inventario.Remove(inventario);
            await _context.SaveChangesAsync();

            return new OkObjectResult("Inventario eliminado.");
        }

        public async Task<IActionResult> ObtenerInventarioMantenimientoAsync()
        {
            var inventariosMantenimiento = await _context.Inventario
                .Where(i => i.Estado == "En Reparación/Mantenimiento")
                .ToListAsync();

            if (inventariosMantenimiento == null || !inventariosMantenimiento.Any())
            {
                return new NotFoundObjectResult("No se encontraron inventarios en mantenimiento.");
            }

            return new OkObjectResult(inventariosMantenimiento);
        }


    }
}

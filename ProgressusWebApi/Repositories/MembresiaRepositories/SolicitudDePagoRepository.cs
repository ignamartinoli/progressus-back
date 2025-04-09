﻿using MercadoPago.Resource.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressusWebApi.DataContext;
using ProgressusWebApi.Dtos.MembresiaDtos;
using ProgressusWebApi.Models.MembresiaModels;
using ProgressusWebApi.Repositories.MembresiaRepositories.Interfaces;

namespace ProgressusWebApi.Repositories.MembresiaRepositories
{
    public class SolicitudDePagoRepository : ISolicitudDePagoRepository
    {
        private readonly ProgressusDataContext _context;
        private readonly IMembresiaRepository _membresiaRepository;

        public SolicitudDePagoRepository(
            ProgressusDataContext context,
            IMembresiaRepository membresiaRepository
        )
        {
            _context = context;
            _membresiaRepository = membresiaRepository;
        }

        // Métodos para SolicitudDePago
        public async Task<SolicitudDePago> CrearSolicitudDePagoAsync(SolicitudDePago solicitud)
        {
            await _context.SolicitudDePagos.AddAsync(solicitud);
            await _context.SaveChangesAsync();
            return solicitud;
        }

        public async Task<SolicitudDePago> ModificarSolicitudDePagoAsync(SolicitudDePago solicitud)
        {
            _context.SolicitudDePagos.Update(solicitud);
            await _context.SaveChangesAsync();
            return solicitud;
        }

        public async Task<List<SolicitudDePago>> ObtenerSolicitudesDePagoDeSocio(
            string identityUserId
        )
        {
            return await _context
                .SolicitudDePagos.Where(s => s.IdentityUserId == identityUserId) // Filtrar por IdentityUserId y excluir MembresiaId = 15
                .Include(s => s.Membresia) // Incluir la membresía
                .Include(s => s.TipoDePago) // Incluir el tipo de pago
                .Include(s => s.HistorialSolicitudDePagos) // Incluir el historial
                .ThenInclude(h => h.EstadoSolicitud) // Incluir el estado de solicitud dentro del historial
                .OrderByDescending(s => s.FechaCreacion) // Ordenar por fecha de creación (más reciente primero)
                .ToListAsync();
        }

        public async Task<SolicitudDePago> ObtenerSolicitudDePagoPorIdAsync(int solicitudId)
        {
            return await _context
                .SolicitudDePagos.Include(s => s.TipoDePago)
                .Include(s => s.Membresia)
                .Include(s => s.HistorialSolicitudDePagos)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);
        }

        // Métodos para TipoDePago
        public async Task<TipoDePago> ObtenerTipoDePagoPorNombreAsync(string nombre)
        {
            return await _context.TipoDePagos.FirstOrDefaultAsync(tp => tp.Nombre == nombre);
        }

        // Métodos para EstadoSolicitud
        public async Task<EstadoSolicitud> ObtenerEstadoSolicitudPorNombreAsync(string nombre)
        {
            return await _context.EstadoSolicitudes.FirstOrDefaultAsync(es => es.Nombre == nombre);
        }

        public async Task<EstadoSolicitud> ObtenerEstadoSolicitudPorIdAsync(int id)
        {
            return await _context.EstadoSolicitudes.FirstOrDefaultAsync(es => es.Id == id);
        }

        // Métodos para HistorialSolicitudDePago
        public async Task<HistorialSolicitudDePago> CrearHistorialSolicitudDePagoAsync(
            HistorialSolicitudDePago historial
        )
        {
            await _context.HistorialSolicitudDePagos.AddAsync(historial);
            await _context.SaveChangesAsync();
            return historial;
        }

        public async Task<
            IEnumerable<HistorialSolicitudDePago>
        > ObtenerTodoElHistorialDeUnaSolicitudAsync(int solicitudId)
        {
            return await _context
                .HistorialSolicitudDePagos.Where(h => h.SolicitudDePagoId == solicitudId)
                .OrderBy(h => h.FechaCambioEstado)
                .ToListAsync();
        }

        public async Task<HistorialSolicitudDePago> ObtenerUltimoHistorialDeUnaSolicitudAsync(
            int solicitudId
        )
        {
            HistorialSolicitudDePago historialSolicitudDePago = await _context
                .HistorialSolicitudDePagos.Where(h => h.SolicitudDePagoId == solicitudId)
                .OrderByDescending(h => h.FechaCambioEstado)
                .FirstOrDefaultAsync();
            return (historialSolicitudDePago);
        }

        public Task<TipoDePago> ObtenerTipoDePagoPorIdAsync(int id)
        {
            return _context.TipoDePagos.FirstOrDefaultAsync(tp => tp.Id == id);
        }

        public async Task<SolicitudDePago> ObtenerSolicitudDePagoDeSocio(string identityUserId)
        {
            // Obtener la solicitud de pago más reciente del socio, excluyendo las que tienen MembresiaId = 15
            SolicitudDePago? solicitud = await _context
                .SolicitudDePagos.Where(s => s.IdentityUserId == identityUserId) // Filtrar por IdentityUserId y excluir MembresiaId = 15
                .Include(s => s.HistorialSolicitudDePagos) // Incluye el historial completo
                .ThenInclude(h => h.EstadoSolicitud) // Incluye también el EstadoSolicitud de cada historial
                .OrderByDescending(s => s.FechaCreacion) // Ordenar por fecha de creación descendente
                .FirstOrDefaultAsync(); // Obtener la primera coincidencia

            // Si no se encuentra ninguna solicitud, devolver null
            if (solicitud == null)
            {
                return null;
            }

            // Cargar el TipoDePago y la Membresia de manera asíncrona
            solicitud.TipoDePago = await this.ObtenerTipoDePagoPorIdAsync(solicitud.TipoDePagoId);
            solicitud.Membresia = await _membresiaRepository.GetById(solicitud.MembresiaId);

            return solicitud;
        }

        public async Task<IActionResult> ObtenerTiposDePagos()
        {
            var tipos = await _context.TipoDePagos.ToListAsync();
            return new OkObjectResult(tipos);
        }

        public async Task<IActionResult> ConsultarVigenciaDeMembresia(string userId)
        {
            var solicitud = await _context
                .SolicitudDePagos.Where(s => s.IdentityUserId == userId.ToString())
                .Include(s => s.HistorialSolicitudDePagos) // Incluye el historial completo
                .ThenInclude(h => h.EstadoSolicitud) // Incluye también el EstadoSolicitud de cada historial
                .OrderByDescending(s =>
                    s.HistorialSolicitudDePagos.Where(h => h.EstadoSolicitud.Nombre == "Confirmado")
                        .OrderByDescending(h => h.FechaCambioEstado)
                        .FirstOrDefault()
                        .FechaCambioEstado
                )
                .FirstOrDefaultAsync();

            // Verifica si no se encontró una solicitud con estado "Confirmado"
            if (solicitud == null)
            {
                return new NotFoundObjectResult(
                    "No se encontró ninguna solicitud confirmada para el usuario."
                );
            }

            var historial = await ObtenerUltimoHistorialDeUnaSolicitudAsync(solicitud.Id);
            solicitud.TipoDePago = this.ObtenerTipoDePagoPorIdAsync(solicitud.TipoDePagoId).Result;
            solicitud.Membresia = _membresiaRepository.GetById(solicitud.MembresiaId).Result;

            // Obtiene la fecha del último cambio de estado "Confirmado" y la duración de la membresía
            var fechaConfirmacion = historial.FechaCambioEstado;

            var duracionMeses = solicitud.Membresia.MesesDuracion;

            // Calcula la fecha de vigencia sumando los meses de duración de la membresía
            var fechaVigencia = fechaConfirmacion.AddMonths(duracionMeses);

            VigenciaDeMembresiaDto vigenciaDto = new VigenciaDeMembresiaDto
            {
                VigenteDesde = fechaConfirmacion,
                VigenteHasta = fechaVigencia,
                EsVigente = fechaVigencia >= DateTime.Now,
            };

            return new OkObjectResult(vigenciaDto);
        }

        public async Task<IActionResult> ObtenerTodasLasSolicitudesDeUnSocio(string identityUserId)
        {
            var solicitudes = await _context
                .SolicitudDePagos.Where(s =>
                    s.IdentityUserId == identityUserId.ToString()
                    && s.HistorialSolicitudDePagos.Any(h =>
                        h.EstadoSolicitud.Nombre == "Confirmado"
                    )
                ) // Filtra por historial con estado "Confirmado"
                .Include(s => s.HistorialSolicitudDePagos) // Incluye el historial completo
                .ThenInclude(h => h.EstadoSolicitud)
                .Include(s => s.TipoDePago)
                .Include(s => s.Membresia)
                .OrderByDescending(s => s.FechaCreacion) // Ordena por fecha de creación de la solicitud
                .ToListAsync();

            return new OkObjectResult(solicitudes);
        }

        public async Task<SolicitudDePago> ActualizarSolicitud(SolicitudDePago solicitudDePago)
        {
            _context.SolicitudDePagos.Update(solicitudDePago);
            await _context.SaveChangesAsync();
            return solicitudDePago;
        }
    }
}

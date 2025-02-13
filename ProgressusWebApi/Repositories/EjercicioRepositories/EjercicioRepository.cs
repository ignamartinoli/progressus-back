using Microsoft.EntityFrameworkCore;
using ProgressusWebApi.DataContext;
using ProgressusWebApi.Dtos.EjercicioDtos.EjercicioDto;
using ProgressusWebApi.Models.EjercicioModels;
using ProgressusWebApi.Repositories.EjercicioRepositories.Interfaces;

namespace ProgressusWebApi.Repositories.EjercicioRepositories
{
    public class EjercicioRepository : IEjercicioRepository
    {
        private readonly ProgressusDataContext _context;

        public EjercicioRepository(ProgressusDataContext context)
        {
            _context = context;
        }

        // 1️Asociar ejercicios
        public async Task AsociarEjercicios(int ejercicioId, List<int> ejerciciosAsociadosIds)
        {
            foreach (var asociadoId in ejerciciosAsociadosIds)
            {
                var asociacion = new EjercicioAsociado
                {
                    EjercicioId = ejercicioId,
                    EjercicioAlternativoId = asociadoId
                };
                _context.EjerciciosAsociados.Add(asociacion);
            }

            await _context.SaveChangesAsync();
        }

        // 2️Obtener ejercicios asociados filtrando por MaquinaEnReparacion = true
        public async Task<EjerciciosAsociadoDto> ObtenerEjerciciosAsociados(int ejercicioId)
        {
            var ejerciciosAsociados = await _context.EjerciciosAsociados
                .Where(ea => ea.EjercicioId == ejercicioId)
                .Select(ea => ea.EjercicioAlternativo)
                .Where(e => e.MaquinaEnReparacion) // Filtra los ejercicios cuya máquina está en reparación
                .ToListAsync();

            return new EjerciciosAsociadoDto
            {
                EjercicioId = ejercicioId,
                EjerciciosAsociados = ejerciciosAsociados
            };
        }

        public async Task<Ejercicio?> Actualizar(int id, Ejercicio ejercicio)
        {
            var existingEjercicio = await _context.Ejercicios.FindAsync(id);
            if (existingEjercicio == null) return null;
            existingEjercicio.Nombre = ejercicio.Nombre;
            existingEjercicio.Descripcion = ejercicio.Descripcion;
            existingEjercicio.ImagenMaquina = ejercicio.ImagenMaquina;
            existingEjercicio.VideoEjercicio = ejercicio.VideoEjercicio;
            await _context.SaveChangesAsync();
            return existingEjercicio;
        }

        public async Task<Ejercicio> Crear(Ejercicio ejercicio)
        {
            _context.Ejercicios.Add(ejercicio);
            await _context.SaveChangesAsync();
            return ejercicio;
        }

        public async Task<Ejercicio?> Eliminar(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);
            if (ejercicio == null) return null;
            _context.Ejercicios.Remove(ejercicio);
            await _context.SaveChangesAsync();
            return ejercicio;
        }

        public async Task<List<Ejercicio>> ObtenerPorGrupoMuscular(int grupoMuscularId)
        {
            return await _context.Ejercicios
                            .Include(e => e.MusculosDeEjercicio)
                            .ThenInclude(me => me.Musculo)
                            .ThenInclude(m => m.GrupoMuscular)
                            .Where(e => e.MusculosDeEjercicio.Any(me => me.Musculo.GrupoMuscularId == grupoMuscularId))
                            .ToListAsync();
        }

        public async Task<Ejercicio?> ObtenerPorId(int id)
        {
            return await _context.Ejercicios
                .Include(e => e.MusculosDeEjercicio)
                    .ThenInclude(e => e.Musculo)
                    .ThenInclude(m => m.GrupoMuscular)
                                 .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Ejercicio>> ObtenerPorMusculo(int musculoId)
        {
            return await _context.Ejercicios
                         .Include(e => e.MusculosDeEjercicio)
                         .ThenInclude(me => me.Musculo)
                         .Where(e => e.MusculosDeEjercicio.Any(me => me.MusculoId == musculoId))
                         .ToListAsync();
        }

        public async Task<List<Ejercicio>> ObtenerTodos()
        {
            return await _context.Ejercicios
                .Select(e => new Ejercicio
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    ImagenMaquina = e.ImagenMaquina,
                    VideoEjercicio = e.VideoEjercicio,
                    MusculosDeEjercicio = e.MusculosDeEjercicio
                        .Select(mde => new MusculoDeEjercicio
                        {
                            EjercicioId = mde.EjercicioId,
                            MusculoId = mde.MusculoId,
                            Musculo = new Musculo
                            {
                                Id = mde.Musculo.Id,
                                Nombre = mde.Musculo.Nombre // Solo el nombre
                            }
                        }).ToList()
                })
                .ToListAsync();
        }
         
    }
}

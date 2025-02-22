using ProgressusWebApi.Dtos.RerservaDto;
//using ProgressusWebApi.Services.ReservaService.Interfaces;
using ProgressusWebApi.DataContext;
using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.DataContext;
using ProgressusWebApi.Dtos.RerservaDto;
using ProgressusWebApi.Models.RerservasModels;
using ProgressusWebApi.Services.ReservaService.cs.interfaces;
using Microsoft.EntityFrameworkCore;
using MercadoPago.Resource.User;
using ProgressusWebApi.Models.AsistenciaModels;
using ProgressusWebApi.Services.NotificacionesServices.Interfaces;

namespace ProgressusWebApi.Services.ReservaServices
{
    public class ReservaService : IReservaService
    {
        private readonly ProgressusDataContext _context;
        private readonly INotificacionesUsuariosService _notificaciones;

        public ReservaService(ProgressusDataContext context, INotificacionesUsuariosService notificaciones)
        {
            _context = context;
            _notificaciones = notificaciones;
        }

        public async Task<IActionResult> CrearReservaAsync(RerservaDto reservaDto)
        {
            // Lógica para crear una reserva
            var existeConflicto = _context.Reservas
                .Any(r => r.FechaReserva == reservaDto.Fecha &&
                          r.HoraInicio < reservaDto.HoraFin &&
                          r.HoraFin > reservaDto.HoraInicio);

            if (existeConflicto)
            {
                return new BadRequestObjectResult("El horario seleccionado ya está reservado.");
            }

            var reserva = new ReservaTurno
            {
                UserId = reservaDto.UserId,
                FechaReserva = reservaDto.Fecha,
                HoraInicio = reservaDto.HoraInicio,
                HoraFin = reservaDto.HoraFin,
                Confirmada = true // Confirmada automáticamente
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return new OkObjectResult("Reserva creada exitosamente.");
        }

        public async Task<IActionResult> VerificarDisponibilidadAsync(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            // Lógica para verificar disponibilidad
            var existeConflicto = _context.Reservas
                .Any(r => r.FechaReserva == fecha && r.HoraInicio < horaFin && r.HoraFin > horaInicio);

            return new OkObjectResult(!existeConflicto);
        }

        public async Task<IActionResult> ObtenerReservasPorUsuarioAsync(string userId)
        {
            var reservas = _context.Reservas
                .Where(r => r.UserId == userId)
                .ToList();

            return new OkObjectResult(reservas);
        }
        public async Task<IActionResult> ObtenerReservasPorHorarioAsync(DateTime fecha, TimeSpan horaInicio)
        {
            var reservas = await _context.Reservas
                .Where(r => r.FechaReserva == fecha && r.HoraInicio == horaInicio)
                .ToListAsync();

            if (!reservas.Any())
            {
                return new NotFoundObjectResult("No se encontraron reservas para la fecha y hora especificadas.");
            }
           
            return new OkObjectResult(reservas);
        }
        public async Task<bool> RegistrarAsistenciaAsync(string userId)
        {
            var ahora = DateTime.Now.AddHours(-3).TimeOfDay; // Obtener solo la hora actual

            // Buscar una reserva válida para el usuario dentro de la franja horaria
            var reserva = await _context.Reservas
                .Where(r => r.UserId == userId
                            && r.HoraInicio <= ahora   // La hora actual está dentro del rango
                            && r.HoraFin >= ahora)
                .FirstOrDefaultAsync();

            if (reserva == null)
            {
                // No se encontró ninguna reserva válida
                return false;
            }

            // Registrar el log de asistencia
            await RegistrarLogDeAsistenciaAsync(userId, reserva.Id);

            return true;
        }
        // Obtener asistencias por usuario
        public async Task<List<AsistenciaLog>> ObtenerAsistenciasPorUsuarioAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("El userId es requerido.", nameof(userId));
            }

            // Buscar las asistencias del usuario
            var asistencias = await _context.AsistenciaLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.FechaAsistencia) // Ordenar por fecha descendente
                .ToListAsync();

            return asistencias;
        }

        // Obtener todas las asistencias
        public async Task<List<AsistenciaLog>> ObtenerTodasLasAsistenciasAsync()
        {
            // Traer todas las asistencias
            var asistencias = await _context.AsistenciaLogs
                .OrderByDescending(a => a.FechaAsistencia) // Ordenar por fecha descendente
                .ToListAsync();

            return asistencias;
        }


        public async Task RegistrarLogDeAsistenciaAsync(string userId, int reservaId)
        {
            // Crear un nuevo registro en la tabla AsistenciaLogs
            var log = new AsistenciaLog
            {
                UserId = userId,                 // ID del usuario
                ReservaId = reservaId,           // ID de la reserva correspondiente
                FechaAsistencia = DateTime.Now   // Fecha y hora actuales
            };

            _context.AsistenciaLogs.Add(log); // Agregar el registro al contexto
            await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
        }
        public async Task<IActionResult> ObtenerEstadisticasReservasAsync()
        {
            var estadisticas = await _context.Reservas
                .GroupBy(r => new { r.FechaReserva.Year, r.FechaReserva.Month })
                .Select(g => new
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    TotalReservas = g.Count()
                })
                .OrderBy(e => e.Anio)
                .ThenBy(e => e.Mes)
                .ToListAsync();

            return new OkObjectResult(estadisticas);
        }

        public async Task<IActionResult> ObtenerEstadisticasPorDiaSemanaAsync()
        {
            // Reduce la cantidad de datos primero (consulta en la base de datos)
            var reservas = await _context.Reservas
                .Select(r => new { r.FechaReserva, r.Id }) // Solo trae los campos necesarios
                .ToListAsync(); // Ejecuta la consulta en la base de datos

            // Realiza la agrupación y conteo en memoria
            var estadisticas = reservas
                .AsEnumerable() // Cambia a LINQ en memoria
                .GroupBy(r => new { r.FechaReserva.Year, DiaSemana = r.FechaReserva.DayOfWeek })
                .Select(g => new
                {
                    Anio = g.Key.Year,
                    DiaSemana = g.Key.DiaSemana.ToString(),
                    TotalReservas = g.Count()
                })
                .OrderBy(e => e.Anio)
                .ThenBy(e => e.DiaSemana)
                .ToList();

            return new OkObjectResult(estadisticas);
        }


        // Método para eliminar una reserva
        public async Task<IActionResult> EliminarReservaAsync(string UserId)
        {
            // Buscar la reserva por ID
            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.UserId == UserId);
            if (reserva == null)
            {
                return new NotFoundObjectResult("No se encontró la reserva especificada.");
            }

            // Eliminar la reserva
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return new OkObjectResult("Reserva eliminada exitosamente.");
        }
        public async Task<IActionResult> EliminarReservaAsyncID(int idReserva)
        {
            // Buscar la reserva por ID
            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.Id == idReserva);
            if (reserva == null)
            {
                return new NotFoundObjectResult("No se encontró la reserva especificada.");
            }

            // Eliminar la reserva
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return new OkObjectResult("Reserva eliminada exitosamente.");
        }


        public async Task<List<AsistenciaLog>> ObtenerAsistenciasPorHoraAsync(string hora)
        {
            if (string.IsNullOrEmpty(hora))
            {
                throw new ArgumentException("La hora es requerida.", nameof(hora));
            }

            // Validar el formato de la hora
            if (!TimeSpan.TryParse(hora, out TimeSpan horaTimeSpan))
            {
                throw new ArgumentException("El formato de la hora no es válido. Ejemplo: 10:00:00.000", nameof(hora));
            }

            // Definir el rango de tiempo
            var horaInicio = horaTimeSpan;
            var horaFin = horaTimeSpan.Add(TimeSpan.FromHours(1)).Subtract(TimeSpan.FromMilliseconds(1)); // Hasta el último milisegundo de la hora

            // Buscar asistencias dentro del rango horario
            var asistencias = await _context.AsistenciaLogs
                .Where(a => a.FechaAsistencia.TimeOfDay >= horaInicio && a.FechaAsistencia.TimeOfDay <= horaFin)
                .OrderByDescending(a => a.FechaAsistencia) // Ordenar por fecha descendente
                .ToListAsync();

            return asistencias;
        }


        public async Task<List<AsistenciasPorDia>> ObtenerNumeroDeAsistenciasPorDiaDeSemanaAsync()
        {
            // Trae todos los registros a memoria y realiza la agrupación en C#
            var asistencias = await _context.AsistenciaLogs
                .ToListAsync(); // Carga los datos desde la base de datos

            var asistenciasPorDiaDeSemana = asistencias
                .GroupBy(a => a.FechaAsistencia.DayOfWeek) // Agrupa por día de la semana
                .Select(grupo => new AsistenciasPorDia
                {
                    DiaDeSemana = grupo.Key.ToString(), // Día de la semana como texto
                    NumeroDeAsistencias = grupo.Count() // Cuenta las asistencias en ese día
                })
                .OrderBy(grupo => grupo.DiaDeSemana) // Ordena por día de la semana (opcional)
                .ToList();

            return asistenciasPorDiaDeSemana;
        }

        public async Task<List<AsistenciasPorMes>> ObtenerAsistenciasPorMesAsync()
        {
            // Trae todos los registros a memoria
            var asistencias = await _context.AsistenciaLogs
                .ToListAsync(); // Carga los datos desde la base de datos

            var asistenciasPorMes = asistencias
                .GroupBy(a => new { a.FechaAsistencia.Year, a.FechaAsistencia.Month }) // Agrupa por año y mes
                .Select(grupo => new AsistenciasPorMes
                {
                    Anio = grupo.Key.Year, // Año de la asistencia
                    Mes = grupo.Key.Month, // Mes de la asistencia
                    NumeroDeAsistencias = grupo.Count() // Cuenta las asistencias en ese mes
                })
                .OrderBy(grupo => grupo.Anio).ThenBy(grupo => grupo.Mes) // Ordena por año y luego por mes
                .ToList();

            return asistenciasPorMes;
        }
        public async Task<List<AsistenciasPorMesYDia>> ObtenerAsistenciasPorMesYDiaAsync()
        {
            // Trae todos los registros a memoria
            var asistencias = await _context.AsistenciaLogs
                .ToListAsync(); // Carga los datos desde la base de datos

            var asistenciasPorMesYDia = asistencias
                .GroupBy(a => new { a.FechaAsistencia.Year, a.FechaAsistencia.Month, DiaDeSemana = a.FechaAsistencia.DayOfWeek }) // Agrupa por año, mes y día de la semana
                .Select(grupo => new AsistenciasPorMesYDia
                {
                    Anio = grupo.Key.Year, // Año de la asistencia
                    Mes = grupo.Key.Month, // Mes de la asistencia
                    Dia = grupo.Key.DiaDeSemana.ToString(), // Día de la semana como texto (Lunes, Martes, etc.)
                    NumeroDeAsistencias = grupo.Count() // Cuenta las asistencias en ese día de la semana
                })
                .OrderBy(grupo => grupo.Anio)  // Ordena por año
                .ThenBy(grupo => grupo.Mes)   // Luego por mes
                .ThenBy(grupo => Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList().IndexOf(Enum.Parse<DayOfWeek>(grupo.Dia))) // Ordena por el día de la semana
                .ToList();

            return asistenciasPorMesYDia;
        }

        public async Task<bool> TodasLasReservasSonAntiguas(string userId)
        {            
            return await _context.Reservas.Where(r => r.UserId == userId).AllAsync(r => r.FechaReserva < DateTime.Now.AddDays(-4));
        }





    }
}

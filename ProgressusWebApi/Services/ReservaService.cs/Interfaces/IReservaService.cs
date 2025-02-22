using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Dtos.RerservaDto;
using ProgressusWebApi.Models.AsistenciaModels;

namespace ProgressusWebApi.Services.ReservaService.cs.interfaces
{
    public interface IReservaService
    {
        Task<IActionResult> CrearReservaAsync(RerservaDto reservaDto);
        Task<IActionResult> VerificarDisponibilidadAsync(DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin);
        Task<IActionResult> ObtenerReservasPorUsuarioAsync(string userId);

        // Agregar el método EliminarReserva
        Task<IActionResult> EliminarReservaAsync(string userId);

        // Agregar el método EliminarReserva
        Task<IActionResult> EliminarReservaAsyncID(int idReserva);

        // Método para obtener reservas por fecha y hora de inicio
        Task<IActionResult> ObtenerReservasPorHorarioAsync(DateTime fecha, TimeSpan horaInicio);

        // Método para registrar la asistencia de un usuario
        Task<bool> RegistrarAsistenciaAsync(string userId);

        // Método para registrar el log de asistencia
        Task RegistrarLogDeAsistenciaAsync(string userId, int reservaId);
        Task<IActionResult> ObtenerEstadisticasReservasAsync();
        Task<IActionResult> ObtenerEstadisticasPorDiaSemanaAsync();

        Task<List<AsistenciaLog>> ObtenerAsistenciasPorHoraAsync(string hora);

        Task<List<AsistenciaLog>> ObtenerAsistenciasPorUsuarioAsync(string userId);

        Task<List<AsistenciaLog>> ObtenerTodasLasAsistenciasAsync();
        Task<List<AsistenciasPorDia>> ObtenerNumeroDeAsistenciasPorDiaDeSemanaAsync();

        Task<List<AsistenciasPorMesYDia>> ObtenerAsistenciasPorMesYDiaAsync();

        Task<List<AsistenciasPorMes>> ObtenerAsistenciasPorMesAsync();

        Task<bool> TodasLasReservasSonAntiguas(string userId);


        }
}
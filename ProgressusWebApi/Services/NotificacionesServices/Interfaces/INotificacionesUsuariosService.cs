using ProgressusWebApi.Dtos.NotificacionDtos;
using ProgressusWebApi.Models.NotificacionesModel;

namespace ProgressusWebApi.Services.NotificacionesServices.Interfaces
{
	public interface INotificacionesUsuariosService
	{
		Task<List<NotificacionDto>> ObtenerNotificacionesPorUsuarioAsync(string usuarioId);
		Task<bool> CrearNotificacionAsync(int plantillaId, string usuarioId);
		Task<bool> CambiarEstadoNotificacionAsync(int notificacionId, string nuevoEstado);
		Task<bool> EliminarNotificacionAsync(int notificacionId);

		Task<bool> EnviarNotificacionesPendientes();
        
		Task<bool> NotificarActualizacionPlan(List<string> usuariosId);

		Task<bool> NotificarMaquinaEnMantenimiento(List<string> usuariosId, string maquina, int dias, string motivo);

		Task<bool> NotificarMembresiaPorVencer(string usuarioId, string fechaVencimiento, List<string> planes);

		Task<bool> CrearNotificacionMasivaAsync(NotificacionMasiva plantillaId);


        Task<bool> NotificarReservasAntiguas(string usuarioId);
    }
}

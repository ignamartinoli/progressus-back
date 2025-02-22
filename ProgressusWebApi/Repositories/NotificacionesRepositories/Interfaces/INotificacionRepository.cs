using ProgressusWebApi.Models.NotificacionesModel;

namespace ProgressusWebApi.Repositories.NotificacionesRepositories.Interfaces
{
	public interface INotificacionRepository
	{
		Task<List<Notificacion>> ObtenerNotificacionesPorUsuarioAsync(string usuarioId);
		Task<bool> CrearNotificacionAsync(Notificacion notificacion);
		Task<bool> EliminarNotificacionAsync(int notificacionId);
		Task<bool> CambiarEstadoNotificacionAsync(int notificacionId, string nuevoEstado);

		Task<List<Notificacion>> ObtenerNotificacionesPendientesAsync();

		Task<bool> CambiarEstadoNotifiacionesMasivo(List<int> idNotificaciones, int idEstado);

    }
}

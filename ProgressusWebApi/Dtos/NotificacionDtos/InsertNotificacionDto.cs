namespace ProgressusWebApi.Dtos.NotificacionDtos
{
	public class InsertNotificacionDto
	{
		public int PlantillaNotificacionId { get; set; }
		public string UsuarioId { get; set; }
		public int EstadoNotificacionId { get; set; }
		public DateTime? FechaEnvio { get; set; }
    }
}

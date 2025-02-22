using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Dtos.NotificacionDtos;
using ProgressusWebApi.Models.NotificacionesModel;
using ProgressusWebApi.Request;
using ProgressusWebApi.Services.NotificacionesServices.Interfaces;


namespace ProgressusWebApi.Controllers.NotificacionControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NotificacionesUsuarioController : ControllerBase
	{
		private readonly INotificacionesUsuariosService _notificacionesService;

		public NotificacionesUsuarioController(INotificacionesUsuariosService notificacionesService)
		{
			_notificacionesService = notificacionesService;
		}

		[HttpGet("{usuarioId}")]
		public async Task<IActionResult> ObtenerNotificacionesPorUsuario(string usuarioId)
		{
			var notificaciones = await _notificacionesService.ObtenerNotificacionesPorUsuarioAsync(usuarioId);
			return Ok(notificaciones);
		}

		[HttpPost]
		public async Task<IActionResult> CrearNotificacion([FromBody] InsertNotificacionDto request)
		{
			var result = await _notificacionesService.CrearNotificacionAsync(request.PlantillaNotificacionId, request.UsuarioId);


			return result ? Ok(new { Message = "Notificación creada correctamente" }) : BadRequest(new { Message = "Error al crear la notificación" });
		}

		[HttpPut("{notificacionId}")]
		public async Task<IActionResult> CambiarEstadoNotificacion(int notificacionId, [FromBody] CambiarEstadoRequest request)
		{
			var result = await _notificacionesService.CambiarEstadoNotificacionAsync(notificacionId, request.NuevoEstado);
			return result ? Ok(new { Message = "Estado de la notificación cambiado correctamente" }) : BadRequest(new { Message = "Error al cambiar el estado de la notificación" });
		}

		[HttpDelete("{notificacionId}")]
		public async Task<IActionResult> EliminarNotificacion(int notificacionId)
		{
			var result = await _notificacionesService.EliminarNotificacionAsync(notificacionId);
			return result ? Ok(new { Message = "Notificación eliminada correctamente" }) : BadRequest(new { Message = "Error al eliminar la notificación" });
		}

        [HttpPut("EnviarNotificacionesPendientes")]
        public async Task<IActionResult> EnviarNotificacionesPendientes()
        {
            var result = await _notificacionesService.EnviarNotificacionesPendientes();
            
			return result ? Ok(new { Message = "Notificaciones enviadas correctamente" }) : BadRequest(new { Message = "Error al enviar las notificaciones" });
        }


        
        [HttpPost ("CrearNotificacionMasiva")]        
        public async Task<IActionResult> CrearNotificacionMasiva([FromBody] NotificacionMasiva request)
        {
            var result = await _notificacionesService.CrearNotificacionMasivaAsync(request);

            return result ? Ok(new { Message = "Notificaciones masiva creada correctamente" }) : BadRequest(new { Message = "Error al crear la notificación" });
        }

    }
}

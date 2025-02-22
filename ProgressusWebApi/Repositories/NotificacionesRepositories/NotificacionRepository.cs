using Microsoft.EntityFrameworkCore;
using ProgressusWebApi.DataContext;
using ProgressusWebApi.Models.NotificacionesModel;
using ProgressusWebApi.Repositories.NotificacionesRepositories.Interfaces;

namespace ProgressusWebApi.Repositories.NotificacionesRepositories
{
	public class NotificacionRepository : INotificacionRepository
	{
		private readonly ProgressusDataContext _context;

		public NotificacionRepository(ProgressusDataContext context)
		{
			_context = context;
		}

        public async Task<List<Notificacion>> ObtenerNotificacionesPendientesAsync()
        {
            return await _context.NotificacionesUsuarios
                .Include(n => n.EstadoNotificacion)
                .Include(n => n.PlantillaNotificacion)
                .ThenInclude(p => p.TipoNotificacion)
                .Where(n => !n.FechaEnvio.HasValue)
				.ToListAsync(); // En realidad esto te va a cagar la performance, pero así está en todos lados
								// Debería ser un IQueryable
        }

        public async Task<List<Notificacion>> ObtenerNotificacionesPorUsuarioAsync(string usuarioId)
		{
			return await _context.NotificacionesUsuarios
				.Include(n => n.EstadoNotificacion)
				.Include(n => n.PlantillaNotificacion)
				.ThenInclude(p => p.TipoNotificacion)
				.Where(n => n.UsuarioId == usuarioId)
				.ToListAsync();
		}

		public async Task<bool> CrearNotificacionAsync(Notificacion notificacion)
		{
			try
			{
				_context.NotificacionesUsuarios.Add(notificacion);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> EliminarNotificacionAsync(int notificacionId)
		{
			try
			{
				var notificacion = await _context.NotificacionesUsuarios.FindAsync(notificacionId);
				if (notificacion == null)
					return false;

				_context.NotificacionesUsuarios.Remove(notificacion);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> CambiarEstadoNotifiacionesMasivo(List<int> idNotificaciones, int idEstado)
		{
			// La razón de este metodo aparte es evitar problemas de performance
			// (no hacer db.savechanges por cada noti)
			// y/o inconsistencia
			// (si falla una noti el resto quedaría enviada)
			try
			{
				if (!idNotificaciones.Any())
					return true;

				var notificaciones = _context.NotificacionesUsuarios.Where(n => idNotificaciones.Contains(n.Id)).ToList();
				if (!notificaciones.Any())
					return false;


				notificaciones = notificaciones.Select(n => 
				{
					n.EstadoNotificacionId = idEstado;
					n.FechaEnvio = DateTime.UtcNow;
					return n; 

				}).ToList();


				await _context.SaveChangesAsync();

				return true;
			}
			catch
			{
				return false;	
			}
		}

		public async Task<bool> CambiarEstadoNotificacionAsync(int notificacionId, string nuevoEstado)
		{
			try
			{
				var notificacion = await _context.NotificacionesUsuarios.FindAsync(notificacionId);
				if (notificacion == null)
					return false;

				var estado = await _context.EstadosNotificaciones.FirstOrDefaultAsync(e => e.Nombre == nuevoEstado);
				if (estado == null)
					return false;

				notificacion.EstadoNotificacionId = estado.Id;
				notificacion.FechaEnvio = nuevoEstado == "Enviada" ? DateTime.UtcNow : null;

				_context.NotificacionesUsuarios.Update(notificacion);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}

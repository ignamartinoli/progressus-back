using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgressusWebApi.Model;
using ProgressusWebApi.Models.CobroModels;
using ProgressusWebApi.Models.EjercicioModels;
using ProgressusWebApi.Models.InventarioModels;
using ProgressusWebApi.Models.MembresiaModels;
using ProgressusWebApi.Models.PlanEntrenamientoModels;
using ProgressusWebApi.Models.RerservasModels;
using ProgressusWebApi.Models.InventarioModels;
using ProgressusWebApi.Models.AsistenciaModels;
using ProgressusWebApi.Models.AsistenciaModels;
using ProgressusWebApi.Models.RolesUsuarioModels;
using ProgressusWebApi.Models.MerchModels;
using ProgressusWebApi.Models.NotificacionModel;
using ProgressusWebApi.Models.CategoriasMerch;
using System.Collections.Generic;
using ProgressusWebApi.Models.NotificacionesModel;
using ProgressusWebApi.Models.AlimentosModels;
using ProgressusWebApi.Models.PlanNutricional;


namespace ProgressusWebApi.DataContext
{
    public class ProgressusDataContext : IdentityDbContext
    {
        public ProgressusDataContext(DbContextOptions<ProgressusDataContext> options) : base(options) { }

        public DbSet<EjercicioAsociado> EjerciciosAsociados { get; set; }
        public DbSet<RegistroDesempeñoSerie> RegistrosDesempeñoSeries { get; set; }

        public DbSet<PlanDeEntrenamiento> PlanesDeEntrenamiento { get; set; }
        public DbSet<DiaDePlan> DiasDePlan { get; set; }
        public DbSet<EjercicioEnDiaDelPlan> EjerciciosDelDia { get; set; }
        public DbSet<SerieDeEjercicio> SeriesDeEjercicio { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<MusculoDeEjercicio> MusculosDeEjercicios { get; set; }
        public DbSet<Musculo> Musculos { get; set; }
        public DbSet<GrupoMuscular> GruposMusculares { get; set; }
        public DbSet<AsignacionDePlan> AsignacionesDePlanes { get; set; }
        public DbSet<ObjetivoDelPlan> ObjetivosDePlanes { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Entrenador> Entrenadores { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<TipoDePago> TipoDePagos { get; set; }
        public DbSet<SolicitudDePago> SolicitudDePagos { get; set; }
        public DbSet<EstadoSolicitud> EstadoSolicitudes { get; set; }
        public DbSet<HistorialSolicitudDePago> HistorialSolicitudDePagos { get; set; }
        public DbSet<ReservaTurno> Reservas { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<AsistenciaLog> AsistenciaLogs { get; set; }
        public DbSet<Merch> Merch { get; set; }
        public DbSet<Models.NotificacionModel.Notificacion> Notificaciones { get; set; }
        public DbSet<CategoriasMerch> CategoriasMerch { get; set; }
        public DbSet<MedicionesUsuario> MedicionesUsuario { get; set; }
        public DbSet<RutinasFinalizadasXUsuario> RutinasFinalizadasXUsuarios { get; set; }
        public DbSet<AsistenciasPorFranjaHoraria> AsistenciasPorFranjaHoraria { get; set; }
        public DbSet<TipoNotificacion> TiposNotificaciones { get; set; }
        public DbSet<EstadoNotificacion> EstadosNotificaciones { get; set; }
        public DbSet<PlantillaNotificacion> PlantillasNotificaciones { get; set; }
        public DbSet<Models.NotificacionesModel.Notificacion> NotificacionesUsuarios { get; set; }
        public DbSet<PlanNutricional> PlanesNutricionales { get; set; }
        public DbSet<DiaPlan> DiasPlan { get; set; }
        public DbSet<Comida> Comidas { get; set; }
        public DbSet<AlimentoComida> AlimentosComida { get; set; }
        public DbSet<Alimento> Alimento { get; set; }
        public DbSet<AsignacionPlanNutricional> AsignacionesPlanNutricional { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Carrito> Carrito { get; set; }
        public DbSet<CarritoItem> CarritoItem { get; set; }
        public DbSet<Pedido> Pedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // **EjercicioAsociado**
            modelBuilder.Entity<EjercicioAsociado>()
                .HasOne(ea => ea.Ejercicio)
                .WithMany()
                .HasForeignKey(ea => ea.EjercicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EjercicioAsociado>()
                .HasOne(ea => ea.EjercicioAlternativo)
                .WithMany()
                .HasForeignKey(ea => ea.EjercicioAlternativoId)
                .OnDelete(DeleteBehavior.Restrict);

            // **RegistroDesempeñoSerie**
            modelBuilder.Entity<RegistroDesempeñoSerie>()
                .HasOne(r => r.EjercicioEnDiaDelPlan)
                .WithMany()
                .HasForeignKey(r => r.EjercicioEnDiaDelPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // **Restricciones de claves compuestas**
            modelBuilder.Entity<AsignacionDePlan>()
                .HasKey(ap => new { ap.PlanDeEntrenamientoId, ap.SocioId });

            modelBuilder.Entity<MusculoDeEjercicio>()
                .HasKey(me => new { me.EjercicioId, me.MusculoId });

            modelBuilder.Entity<EjercicioEnDiaDelPlan>()
                .HasKey(edp => new { edp.EjercicioId, edp.DiaDePlanId });

            // **Relaciones adicionales**
            modelBuilder.Entity<MusculoDeEjercicio>()
                .HasOne(me => me.Ejercicio)
                .WithMany(e => e.MusculosDeEjercicio)
                .HasForeignKey(me => me.EjercicioId);

            modelBuilder.Entity<DiaDePlan>()
                .HasOne(dp => dp.PlanDeEntrenamiento)
                .WithMany(p => p.DiasDelPlan)
                .HasForeignKey(dp => dp.PlanDeEntrenamientoId);

            modelBuilder.Entity<EjercicioEnDiaDelPlan>()
                .HasOne(ed => ed.DiaDePlan)
                .WithMany(dp => dp.EjerciciosDelDia)
                .HasForeignKey(ed => ed.DiaDePlanId);

            modelBuilder.Entity<EjercicioEnDiaDelPlan>()
                .HasOne(ed => ed.Ejercicio)
                .WithMany()
                .HasForeignKey(ed => ed.EjercicioId);

            modelBuilder.Entity<SerieDeEjercicio>()
                .HasKey(se => new { se.Id, se.DiaDePlanId, se.EjercicioId });

            modelBuilder.Entity<SerieDeEjercicio>()
                .HasOne(se => se.EjercicioDelDia)
                .WithMany(ed => ed.SeriesDeEjercicio)
                .HasForeignKey(se => new { se.EjercicioId, se.DiaDePlanId });

            modelBuilder.Entity<HistorialSolicitudDePago>()
                .HasOne(h => h.SolicitudDePago)
                .WithMany(sp => sp.HistorialSolicitudDePagos)
                .HasForeignKey(h => h.SolicitudDePagoId);

            modelBuilder.Entity<HistorialSolicitudDePago>()
                .HasOne(h => h.EstadoSolicitud)
                .WithMany()
                .HasForeignKey(h => h.EstadoSolicitudId);

            modelBuilder.Entity<SolicitudDePago>()
                .HasOne(h => h.TipoDePago)
                .WithMany()
                .HasForeignKey(h => h.TipoDePagoId);

            modelBuilder.Entity<SolicitudDePago>()
                .HasOne(h => h.Membresia)
                .WithMany()
                .HasForeignKey(h => h.MembresiaId);
        }
    }

}


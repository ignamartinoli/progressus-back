﻿using Microsoft.AspNetCore.Identity;
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


namespace ProgressusWebApi.DataContext
{
    public class ProgressusDataContext : IdentityDbContext
    {
        public ProgressusDataContext(DbContextOptions<ProgressusDataContext> options) : base(options) { }
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
        public DbSet<SolicitudDePago> SolicitudDePagos  { get; set; }
        public DbSet<EstadoSolicitud> EstadoSolicitudes  { get; set; }
        public DbSet<HistorialSolicitudDePago> HistorialSolicitudDePagos { get; set; }

        public DbSet<ReservaTurno> Reservas { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<AsistenciaLog> AsistenciaLogs { get; set; }
        public DbSet<Merch> Merch { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<CategoriasMerch> CategoriasMerch { get; set; }
        public DbSet<MedicionesUsuario> MedicionesUsuario { get; set; }


        public DbSet<RutinasFinalizadasXUsuario> RutinasFinalizadasXUsuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de claves compuestas
            modelBuilder.Entity<AsignacionDePlan>()
                .HasKey(ap => new { ap.PlanDeEntrenamientoId, ap.SocioId });

            modelBuilder.Entity<MusculoDeEjercicio>()
                .HasKey(me => new { me.EjercicioId, me.MusculoId });

            modelBuilder.Entity<EjercicioEnDiaDelPlan>()
                .HasKey(edp => new { edp.EjercicioId, edp.DiaDePlanId });


            // Configuración de las relaciones entre las entidades
            modelBuilder.Entity<MusculoDeEjercicio>()
                .HasOne(me => me.Ejercicio)
                .WithMany(e => e.MusculosDeEjercicio)
                .HasForeignKey(me => me.EjercicioId);

            modelBuilder.Entity<MusculoDeEjercicio>()
                .HasOne(me => me.Musculo)
                .WithMany(m => m.MusculosDeEjercicio)
                .HasForeignKey(me => me.MusculoId);

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

            modelBuilder.Entity<Musculo>()
                .HasOne(m => m.GrupoMuscular)
                .WithMany(gm => gm.MusculosDelGrupo)
                .HasForeignKey(m => m.GrupoMuscularId);

            //modelBuilder.Entity<AsignacionDePlan>()
                //.HasOne(ap => ap.Socio)
                //.WithMany()
                //.HasForeignKey(ap => ap.SocioId);

            modelBuilder.Entity<AsignacionDePlan>()
                .HasOne(ap => ap.PlanDeEntrenamiento)
                .WithMany(p => p.Asignaciones)
                .HasForeignKey(ap => ap.PlanDeEntrenamientoId);

            modelBuilder.Entity<PlanDeEntrenamiento>()
                .HasOne(m => m.ObjetivoDelPlan)
                .WithMany(op => op.PlanesDeEntrenamiento)
                .HasForeignKey(m => m.ObjetivoDelPlanId);

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
            modelBuilder.Entity<ReservaTurno>()
            .HasOne<IdentityUser>()  // O reemplaza con tu clase personalizada si la tienes
            .WithMany()
             .HasForeignKey(r => r.UserId)
             .HasConstraintName("FK_Reservas_Users");

            // Configurar relación entre AsistenciaLog y ReservaTurno
            modelBuilder.Entity<AsistenciaLog>()
                .HasOne(a => a.ReservaTurno) // Relación con ReservaTurno
                .WithMany()                 // Relación de uno a muchos
                .HasForeignKey(a => a.ReservaId) // Clave foránea
                .HasConstraintName("FK_AsistenciaLog_ReservaTurno");

            // Configurar relación entre AsistenciaLog y IdentityUser
            modelBuilder.Entity<AsistenciaLog>()
                .HasOne<IdentityUser>() // Relación con el usuario
                .WithMany()
                .HasForeignKey(a => a.UserId) // Clave foránea
                .HasConstraintName("FK_AsistenciaLog_Users");

            modelBuilder.Entity<RutinasFinalizadasXUsuario>()
                .ToTable("RutinasFinalizadasXUsuario");

            modelBuilder.Entity<MedicionesUsuario>()
                .ToTable("Mediciones");

        }
    }
}

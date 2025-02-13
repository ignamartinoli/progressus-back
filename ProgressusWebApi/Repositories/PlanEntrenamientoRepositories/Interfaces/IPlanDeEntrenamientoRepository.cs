using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Model;
using ProgressusWebApi.Models.PlanEntrenamientoModels;

namespace ProgressusWebApi.Repositories.Interfaces
{
    public interface IPlanDeEntrenamientoRepository
    {
        Task CrearRegistrosDeDesempeño(List<RegistroDesempeñoSerie> desempeños);
        Task<List<RegistroDesempeñoSerie>> ObtenerRegistrosEntreFechas(DateTime fechaInicio, DateTime fechaFin);
        Task<PlanDeEntrenamiento> Crear(PlanDeEntrenamiento planDeEntrenamiento);
        Task<bool> Eliminar(int id);
        Task<PlanDeEntrenamiento> ObtenerPorId(int id);
        Task<List<PlanDeEntrenamiento>> ObtenerPorNombre(string nombre);
        Task<PlanDeEntrenamiento?> Actualizar(int id, PlanDeEntrenamiento planDeEntrenamiento);
        Task<List<PlanDeEntrenamiento>> ObtenerPorObjetivo(int objetivoDePlanId);
        Task<List<PlanDeEntrenamiento>> ObtenerPlantillasDePlanes();

        Task<List<PlanDeEntrenamiento>> ObtenerTodosLosPlanes();

        Task<List<PlanDeEntrenamiento>> ObtenerPlanesDelUsuario(string identityUser);

        Task<IActionResult> ObtenerPorIdSimplificado(int id);
    }
}

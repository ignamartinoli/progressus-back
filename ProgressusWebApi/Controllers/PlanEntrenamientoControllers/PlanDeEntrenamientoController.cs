using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgressusWebApi.Dtos.PlanDeEntrenamientoDtos.PlanDeEntrenamiento;
using ProgressusWebApi.Dtos.PlanDeEntrenamientoDtos.PlanDeEntrenamientoDto;
using ProgressusWebApi.Model;
using ProgressusWebApi.Models.PlanEntrenamientoModels;
using ProgressusWebApi.Services.PlanEntrenamientoServices.Interfaces;

namespace ProgressusWebApi.Controllers.PlanEntrenamientoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanDeEntrenamientoController : ControllerBase
    {
        private readonly IPlanDeEntrenamientoService _planDeEntrenamientoService;
        public PlanDeEntrenamientoController(IPlanDeEntrenamientoService planDeEntrenamientoService)
        {
            _planDeEntrenamientoService = planDeEntrenamientoService;
        }

        //ObtenerPorId, ObtenerPlantillas, ObtenerPorNombre, ObtenerPorObjetivo
        // 1️⃣ Endpoint para crear registros de desempeño
        [HttpPost("CrearRegistrosDesempeño")]
        public async Task<IActionResult> CrearRegistrosDesempeño([FromBody] DesempeñoDto desempeñoDto)
        {
            if (desempeñoDto == null || desempeñoDto.desempeños == null || !desempeñoDto.desempeños.Any())
            {
                return BadRequest("Debe proporcionar al menos un registro de desempeño.");
            }

            await _planDeEntrenamientoService.CrearRegistrosDeDesempeño(desempeñoDto);
            return Ok("Registros de desempeño creados correctamente.");
        }

        // 2️⃣ Endpoint para obtener registros entre fechas
        [HttpGet("ObtenerRegistrosEntreFechas")]
        public async Task<IActionResult> ObtenerRegistrosEntreFechas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            var registros = await _planDeEntrenamientoService.ObtenerRegistrosEntreFechas(fechaInicio, fechaFin);
            return Ok(registros);
        }



        [HttpPost("CrearPlanDeEntrenamiento")]
        public async Task<IActionResult> CrearPlanDeEntrenamiento([FromBody] CrearPlanDeEntrenamientoDto planDto)
        {
            PlanDeEntrenamiento planCreado = await _planDeEntrenamientoService.Crear(planDto);
            return Ok(planCreado);
        }

        [HttpPost("ActualizarPlanDeEntrenamiento")]
        public async Task<IActionResult> ActualizarPlanDeEntrenamiento(int id, ActualizarPlanDeEntrenamientoDto planDto)
        {
            PlanDeEntrenamiento planActualizado = await _planDeEntrenamientoService.Actualizar(id, planDto);
            return Ok(planActualizado);
        }

        [HttpPut("ActualizarEjerciciosDelPlan")]
        public async Task<IActionResult> ActualizarEjerciciosDelPlan(AgregarQuitarEjerciciosAPlanDto ejerciciosEnPlanDto)
        {
            PlanDeEntrenamiento? ejerciciosActualizados = await _planDeEntrenamientoService.ActualizarEjerciciosDelPlan(ejerciciosEnPlanDto.PlanDeEntrenamientoId, ejerciciosEnPlanDto);
            return Ok(ejerciciosActualizados);
        }

        [HttpPut("ConvertirPlanEnPlantilla")]
        public async Task<IActionResult> ConvertirEnPlantilla(int id)
        {
            PlanDeEntrenamiento? nuevaPlantilla = await _planDeEntrenamientoService.ConvertirEnPlantilla(id);
            return Ok(nuevaPlantilla);
        }

        [HttpPut("QuitarConvertirPlanEnPlantilla")]
        public async Task<IActionResult> QuitarConvertirEnPlantilla(int id)
        {
            PlanDeEntrenamiento? nuevaPlantilla = await _planDeEntrenamientoService.QuitarConvertirEnPlantilla(id);
            return Ok(nuevaPlantilla);
        }

        [HttpDelete("EliminarPlanDeEntrenamiento")]
        public async Task<IActionResult> EliminarPlanDeEntrenamiento(int id)
        {
            bool? planEliminado = await _planDeEntrenamientoService.Eliminar(id);
            return Ok();
        }


        [HttpGet("ObtenerPlanesPlantillas")]
        public async Task<IActionResult> ObtenerPlanesPlantillas()
        {
            List<PlanDeEntrenamiento> planes = await _planDeEntrenamientoService.ObtenerPlantillasDePlanes();
            return Ok(planes);
        }

        [HttpGet("ObtenerPlanesDelUsuario")]
        public async Task<IActionResult> ObtenerPlanesDelUsuario(string identityUser)
        {
            List<PlanDeEntrenamiento?> planes = await _planDeEntrenamientoService.ObtenerPlanesDelUsuario(identityUser);
            return Ok(planes);
        }

        [HttpGet("ObtenerTodosLosPlanes")]
        public async Task<IActionResult> ObtenerTodosLosPlanes()
        {
            List<PlanDeEntrenamiento?> planes = await _planDeEntrenamientoService.ObtenerTodosLosPlanes();
            return Ok(planes);
        }

        [HttpGet("ObtenerPlanPorId")]
        public async Task<IActionResult> ObtenerPlanPorId(int id)
        {
            var plan =  _planDeEntrenamientoService.ObtenerPorId(id).Result;
            return Ok(plan);
        }
        [HttpPost("AgregarEjercicioAPlan")]
        public async Task<IActionResult> AgregarEjercicioAPlan(AgregarQuitarUnSoloEjercicioDto dto)
        {
            PlanDeEntrenamiento planActualizado = await _planDeEntrenamientoService.AgregarEjercicioDePlan(dto);
            return Ok(planActualizado);
        }

        [HttpDelete("QuitarEjercicioAPlan")]
        public async Task<IActionResult> QuitarEjercicioAPlan(AgregarQuitarUnSoloEjercicioDto dto)
        {
            PlanDeEntrenamiento planActualizado = await _planDeEntrenamientoService.QuitarEjercicioDePlan(dto);
            return Ok(planActualizado);
        }

    }
}

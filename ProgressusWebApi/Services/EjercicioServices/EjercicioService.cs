using ProgressusWebApi.Dtos.EjercicioDtos.EjercicioDto;
using ProgressusWebApi.Models.EjercicioModels;
using ProgressusWebApi.Repositories.EjercicioRepositories;
using ProgressusWebApi.Repositories.EjercicioRepositories.Interfaces;
using ProgressusWebApi.Services.EjercicioServices.Interfaces;

namespace ProgressusWebApi.Services.EjercicioServices
{
    public class EjercicioService : IEjercicioService
    {
        private readonly IEjercicioRepository _repository;
        public EjercicioService(IEjercicioRepository repository)
        {
            _repository = repository;
        }

        public async Task AsociarEjercicios(int ejercicioId, List<int> ejerciciosAsociadosIds)
        {
            await _repository.AsociarEjercicios(ejercicioId, ejerciciosAsociadosIds);
        }

        public async Task<EjerciciosAsociadoDto> ObtenerEjerciciosAsociados(int ejercicioId)
        {
            return await _repository.ObtenerEjerciciosAsociados(ejercicioId);
        }
        public async Task<Ejercicio?> Actualizar(int id, CrearActualizarEjercicioDto ejercicioDto)
        {
            Ejercicio ejercicio = new Ejercicio()
            {
                Nombre = ejercicioDto.Nombre,
                Descripcion = ejercicioDto.Descripcion,
                ImagenMaquina = ejercicioDto.ImagenMaquina,
                VideoEjercicio = ejercicioDto.VideoEjercicio
            };

            return await _repository.Actualizar(id, ejercicio);
        }

        public async Task<Ejercicio> Crear(CrearActualizarEjercicioDto ejercicioDto)
        {
            Ejercicio ejercicio = new Ejercicio()
            {
                Nombre = ejercicioDto.Nombre,
                Descripcion = ejercicioDto.Descripcion,
                ImagenMaquina = ejercicioDto.ImagenMaquina,
                VideoEjercicio = ejercicioDto.VideoEjercicio
            };
            return await _repository.Crear(ejercicio);
        }

        public async Task<Ejercicio?> Eliminar(int id)
        {
            return await _repository.Eliminar(id);
        }

        public async Task<List<Ejercicio?>> ObtenerPorGrupoMuscular(int grupoMuscularId)
        {
            return await _repository.ObtenerPorGrupoMuscular(grupoMuscularId);
        }

        public async Task<Ejercicio?> ObtenerPorId(int id)
        {
            return await _repository.ObtenerPorId(id);
        }

        public async Task<List<Ejercicio?>> ObtenerPorMusculo(int musculoId)
        {
            return await _repository.ObtenerPorMusculo(musculoId);
        }

        public async Task<List<Ejercicio?>> ObtenerTodos()
        {
            return await _repository.ObtenerTodos();
        }
    }
}

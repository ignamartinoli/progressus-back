using ProgressusWebApi.Models.EjercicioModels;

namespace ProgressusWebApi.Dtos.EjercicioDtos.EjercicioDto
{
    public class EjerciciosAsociadoDto
    {
        public int EjercicioId { get; set; }
        public List<Ejercicio> EjerciciosAsociados { get; set; }
    }

}

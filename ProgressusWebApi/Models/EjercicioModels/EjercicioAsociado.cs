namespace ProgressusWebApi.Models.EjercicioModels
{
    public class EjercicioAsociado
    {
        public int Id { get; set; }
        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }
        public Ejercicio EjercicioAlternativo { get; set; }
        public int EjercicioAlternativoId { get; set; }
    }
}

namespace ProgressusWebApi.Models.PlanEntrenamientoModels
{
    public class RegistroDesempeñoSerie
    {
        public int Id { get; set; }
        public EjercicioEnDiaDelPlan EjercicioEnDiaDelPlan { get; set; }
        public int EjercicioEnDiaDelPlanId { get; set; }
        public int RegistroDesempeñoDiaId { get; set; }
        public int RepeticionesConcretadas { get; set; }
        public int PesoDeRepeticion { get; set; }
        public int ResultadoRM {  get; set; }

        public DateTime FechaHora { get; set; }

    }
}

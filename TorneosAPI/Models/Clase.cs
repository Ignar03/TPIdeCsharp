namespace TorneosAPI.Models
{
    public class Torneo
    {
        public Torneo(int id, string nombre_torneo, int id_region, DateOnly fecha_inicio, DateOnly fecha_fin, bool torneo_activo, int minimo_medallas)
        {
            this.id = id;
            this.nombre_torneo = nombre_torneo;
            this.id_region = id_region;
            this.fecha_inicio = fecha_inicio;
            this.fecha_fin = fecha_fin;
            this.torneo_activo = torneo_activo;
            this.minimo_medallas = minimo_medallas;
        }

        public int id { get; set; }
        public string nombre_torneo { get; set; }
        public int id_region { get; set; }
        public DateOnly fecha_inicio { get; set; }
        public DateOnly fecha_fin {  get; set; }
        public bool torneo_activo { get; set; }
        public int minimo_medallas { get; set; }


    }
}

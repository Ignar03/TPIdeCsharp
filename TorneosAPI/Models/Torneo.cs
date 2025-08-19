using System.Data;

namespace TorneosAPI.Models
{
    public class Torneo
    {
        public int id { get; set; }
        public string nombre_torneo { get; set; }
        public int id_region { get; set; }
        public DateOnly fecha_inicio { get; set; }
        public DateOnly fecha_fin { get; set; }
        public bool torneo_activo { get; set; }
        public int minimo_medallas { get; set; }


    public Torneo(DataRow registro)
        {
            this.id = int.Parse(registro["id"].ToString() ?? "0");
            this.nombre_torneo = registro["nombre_torneo"].ToString() ?? "ERROR";
            this.id_region = int.Parse(registro["id_region"].ToString() ?? "0");
            this.fecha_inicio = DateOnly.Parse(registro["fecha_inicio"].ToString() ?? "00-00-0000");
            this.fecha_fin = DateOnly.Parse(registro["fecha_fin"].ToString() ?? "00-00-0000");
            this.torneo_activo = bool.Parse(registro["torneo_activo"].ToString() ?? "0");
            this.minimo_medallas = int.Parse(registro["minimo_medallas"].ToString() ?? "0");
        }
    }
}

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TorneosAPI.Models;

namespace TorneosAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TorneosController : ControllerBase
    {
        [HttpGet]
        [ActionName("getAllTorneos")]
        public ActionResult <List<Torneo>> getAllTorneos() //List es el resultado de la accion y Torneo es que es lo que contiene List
        {
            List<Torneo> listaMock = new();
            listaMock.Add(new Torneo(1, "La Docta", 4, new DateOnly(2025, 4, 8), new DateOnly(2025, 6, 9), false, 20));
            listaMock.Add(new Torneo(2, "La Docta", 7, new DateOnly(2025, 7, 12), new DateOnly(2025, 9, 11), true, 20));
            listaMock.Add(new Torneo(3, "La Docta", 3, new DateOnly(2026, 10, 17), new DateOnly(2025, 12, 28), false, 20));
            return Ok(listaMock);
        }
        [HttpGet]
        [ActionName("getTorneosActivos")]
        public ActionResult getTorneosActivos()
        {
            return Ok(new { mensaje = "Lista de torneos activos" });
        }
        [HttpGet("{id_torneo}")] // Agregamos el parametro
        [ActionName("getTorneoId")]// Nombre de la accion
        public ActionResult<List<Torneo>> getTorneoId(int id_torneo)
        {

            //Extracto del endpoint 

            DataTable respuesta = new();
            string consulta = "SELECT * FROM Pokemones";//Consulta SQL 

            using SqlConnection conexionBase = new SqlConnection("DataSource");
            using var comando = new SqlCommand(consulta, conexionBase);//Creamos el comando en la base con la conexion
            comando.CommandType = CommandType.Text;//Notificamos a la base que vamos a enviar
            SqlDataAdapter Adaptador = new(comando);
            conexionBase.Open(); //Abre la conexion (Puede fallar) 
            Adaptador.Fill(respuesta); //Contacta la base y ejecuta el comando 

            //Linea super importante para que no quede la conexion abierta 
            conexionBase.Close();
            //Resto del endpoint 
        }
        [HttpPost]
        [ActionName("postTorneo")]
        public ActionResult postTorneo([FromBody] object torneo)
        {
            return Ok(new {mensaje = "Torneo cargado con exito!",  datos = torneo });
        }
        [HttpPut]
        [ActionName("putTorneo")]
        public ActionResult putTorneo([FromBody] object torneo)
        { 
            return Ok(new { mensaje = "Torneo modificado con exito!", datos = torneo });
        }
        [HttpDelete]
        [ActionName("deleteTorneo")]
        public ActionResult deleteTorneo(int id_torneo)
        { 
            return Ok(new { mensaje = $"Torneo {id_torneo} borrado con exito." });
        }
    }
}

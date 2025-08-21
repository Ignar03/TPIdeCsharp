using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TorneosAPI.Adapters;
using TorneosAPI.Logs;
using TorneosAPI.Models;

namespace TorneosAPI.Controllers
{
    [EnableCors("CorsRules")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TorneosController : ControllerBase
    {
        private readonly GeneralAdapterSQL _adapter = new();

        [HttpGet]
        [ActionName("getTorneos")]
        public ActionResult<IEnumerable<Torneo>> getTorneos()
        {
            try
            {
                var respuesta = _adapter.EjecutarVista("SELECT * FROM Torneos");
                if (respuesta.Rows.Count > 0)
                {
                    if (respuesta.Columns.Contains("RESULTADO") && respuesta.Rows[0][0].ToString()?.Trim() == "ERROR")
                        return StatusCode(500, "Error al consultar la base de datos. Revisa la cadena de conexión y que la tabla exista.");
                    var listado = new List<Torneo>();
                    foreach (DataRow registro in respuesta.Rows)
                        listado.Add(new Torneo(registro));
                    return Ok(listado);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        [HttpGet]
        [ActionName("getTorneosActivos")]
        public ActionResult<IEnumerable<Torneo>> getTorneosActivos()
        {
            var respuesta = _adapter.EjecutarVista("SELECT * FROM Torneos WHERE torneo_activo = 1");
            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Columns.Contains("RESULTADO") && respuesta.Rows[0][0].ToString()?.Trim() == "ERROR")
                    return StatusCode(500, "Error al consultar la base de datos.");
                var listado = new List<Torneo>();
                foreach (DataRow registro in respuesta.Rows)
                    listado.Add(new Torneo(registro));
                return Ok(listado);
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        [ActionName("getTorneoId")]
        public ActionResult<Torneo> getTorneoId(int id)
        {
            var respuesta = _adapter.EjecutarVista($"SELECT * FROM Torneos WHERE id_torneo = {id}");
            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Columns.Contains("RESULTADO") && respuesta.Rows[0][0].ToString()?.Trim() == "ERROR")
                    return StatusCode(500, "Error al consultar la base de datos.");
                return Ok(new Torneo(respuesta.Rows[0]));
            }
            return NotFound();
        }

        [HttpPost]
        [ActionName("postTorneo")]
        public ActionResult postTorneo([FromBody] Torneo nuevoTorneo)
        {
            string comando = $@"
                INSERT INTO Torneos (id, nombre_torneo, id_region, fecha_inicio, fecha_fin, torneo_activo, minimo_medallas) 
                VALUES ('{nuevoTorneo.id}', '{nuevoTorneo.nombre_torneo}', {nuevoTorneo.id_region}, 
                        '{nuevoTorneo.fecha_inicio:yyyy-MM-dd}', 
                        '{nuevoTorneo.fecha_fin:yyyy-MM-dd}', 
                        {(nuevoTorneo.torneo_activo ? 1 : 0)}, {nuevoTorneo.minimo_medallas})";
            int resultado = _adapter.EjecutarComando(comando);
            if (resultado > 0)
                return StatusCode(201, "Torneo creado correctamente.");
            if (resultado == -1)
                return StatusCode(500, "Error al crear el torneo.");
            return BadRequest("No se pudo crear el torneo.");
        }

        [HttpPut("{id_torneo}")]
        [ActionName("putTorneo")]
        public ActionResult putTorneo(int id_torneo, [FromBody] Torneo modificarTorneo)
        {
            string comando = $@"
                UPDATE Torneos SET
                    id = '{modificarTorneo.id}',
                    nombre_torneo = '{modificarTorneo.nombre_torneo}',
                    id_region = {modificarTorneo.id_region},
                    fecha_inicio = '{modificarTorneo.fecha_inicio:yyyy-MM-dd}',
                    fecha_fin = '{modificarTorneo.fecha_fin:yyyy-MM-dd}',
                    torneo_activo = {(modificarTorneo.torneo_activo ? 1 : 0)},
                    minimo_medallas = {modificarTorneo.minimo_medallas}
                WHERE id_torneo = {id_torneo}";
            int resultado = _adapter.EjecutarComando(comando);
            if (resultado > 0)
                return Ok("Torneo actualizado correctamente.");
            if (resultado == -1)
                return StatusCode(500, "Error al actualizar el torneo.");
            return NotFound("No se encontró el torneo para actualizar.");
        }

        [HttpDelete("{id}")]
        [ActionName("deleteTorneo")]
        public ActionResult deleteTorneo(int id)
        {
            int resultado = _adapter.EjecutarComando($"DELETE FROM Torneos WHERE id_torneo = {id}");
            if (resultado > 0)
                return Ok("Torneo eliminado correctamente.");
            if (resultado == -1)
                return StatusCode(500, "Error al eliminar el torneo.");
            return NotFound("No se encontró el torneo para eliminar.");
        }
    }
}

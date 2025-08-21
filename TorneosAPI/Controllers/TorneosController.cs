using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using TorneosAPI.Models;
using TorneosAPI.Adapters;
using Microsoft.AspNetCore.Cors;
using TorneosAPI.Logs;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TorneosAPI.Controllers
{
    [EnableCors("CorsRules")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TorneosController : ControllerBase
    {
        private readonly GeneralAdapterSQL consultor = new();
        [HttpGet]
        [ActionName("getTorneos")]
        [ProducesResponseType(typeof(IEnumerable<Torneo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Torneo>> getTorneos()
        {
            try
            {
                DataTable respuesta = consultor.EjecutarVista("SELECT * FROM Torneos");

                if (respuesta.Columns.Contains("RESULTADO"))
                    return StatusCode(409, "Conflicto en la consulta");

                if (respuesta.Rows.Count == 0)
                    return NoContent();

                List<Torneo> lista = new();
                foreach (DataRow registro in respuesta.Rows)
                    lista.Add(new(registro));

                return Ok(lista);
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error en GET ObtenerTorneoCompleto");
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        [HttpGet]
        [ActionName("getTorneosActivos")]
        [ProducesResponseType(typeof(IEnumerable<Torneo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Torneo>> getTorneosActivos()
        {
            try
            {
                DataTable respuesta = consultor.EjecutarVista("SELECT * FROM Torneos WHERE torneo_activo = 1");

                if (respuesta.Columns.Contains("RESULTADO"))
                    return StatusCode(409, "Conflicto en la consulta");

                if (respuesta.Rows.Count == 0)
                    return NoContent();

                List<Torneo> lista = new();
                foreach (DataRow registro in respuesta.Rows)
                    lista.Add(new(registro));

                return Ok(lista);
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error en GET ObtenerTorneosActivo");
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }


        [HttpGet("${id}")]
        [ActionName("getTorneoId")]
        [ProducesResponseType(typeof(IEnumerable<Torneo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> getTorneoId(int id_torneo)
        {
            try
            {
                DataTable respuesta = consultor.EjecutarVista($"SELECT * FROM Torneos WHERE id_torneo = {id_torneo}");

                if (respuesta.Columns.Contains("RESULTADO"))
                    return StatusCode(409, "Conflicto en la consulta");

                if (respuesta.Rows.Count == 0)
                    return NotFound($"No se encontró el torneo con ID {id_torneo}");

                Torneo torneo = new(respuesta.Rows[0]);
                return Ok(torneo);
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error en GET ObtenerTorneoXid");
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }


        [HttpPost]
        [ActionName("postTorneo")]
        [ProducesResponseType(typeof(Torneo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> postTorneo([FromBody] Torneo nuevoTorneo)
        {
            try
            {
                if (nuevoTorneo == null)
                    return BadRequest("Body inválido");

                string sql = $@"
                    INSERT INTO Torneos (id, nombre_torneo, id_region, fecha_inicio, fecha_fin, torneo_activo, minimo_medallas)
                    VALUES ({nuevoTorneo.id}, '{nuevoTorneo.nombre_torneo}', {nuevoTorneo.id_region},
                            '{nuevoTorneo.fecha_inicio:yyyy-MM-dd}', '{nuevoTorneo.fecha_fin:yyyy-MM-dd}',
                            {(nuevoTorneo.torneo_activo ? 1 : 0)}, {nuevoTorneo.minimo_medallas})
                ";

                int filas = consultor.EjecutarComando(sql);

                if (filas == -1)
                    return StatusCode(500, "Error interno al crear torneo");

                if (filas == 0)
                    return Conflict("No se pudo insertar el torneo");

                return Created($"/Torneos/api/ObtenerTorneoXid/{nuevoTorneo.id}", nuevoTorneo);
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error en POST CargarTorneo");
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        [HttpPut("{id_torneo}")]
        [ActionName("putTorneo")]
        [ProducesResponseType(typeof(Torneo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> putTorneo(int id_torneo, [FromBody] Torneo modificado)
        {
            try
            {
                if (modificado == null)
                    return BadRequest("Body inválido");

                string sql = $@"
                    UPDATE Torneos
                    SET id = '{modificado.id}',
                        nombre_torneo = '{modificado.nombre_torneo}',
                        id_region = {modificado.id_region},
                        fecha_inicio = '{modificado.fecha_inicio:yyyy-MM-dd}',
                        fecha_fin = '{modificado.fecha_fin:yyyy-MM-dd}',
                        torneo_activo = {(modificado.torneo_activo ? 1 : 0)},
                        minimo_medallas = {modificado.minimo_medallas}
                    WHERE id_torneo = {modificado.id}
                ";

                int filas = consultor.EjecutarComando(sql);

                if (filas == -1)
                    return StatusCode(500, "Error interno al modificar torneo");

                if (filas == 0)
                    return NotFound($"No se encontró el torneo con ID {modificado.id}");

                return Ok("Torneo modificado con éxito");
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error en PUT ModificarTorneo");
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ActionName("deleteTorneo")]
        [ProducesResponseType(typeof(Torneo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> deleteTorneo(int id)
        {
            try
            {
                string sql = $"UPDATE Torneos SET torneo_activo = 0 WHERE id = {id}";

                int filas = consultor.EjecutarComando(sql);

                if (filas == -1)
                    return StatusCode(500, "Error interno al desactivar torneo");

                if (filas == 0)
                    return NotFound($"No se encontró el torneo con ID {id}");

                return Ok("Torneo desactivado con éxito");
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error en DELETE DesactivarTorneo");
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
    }
}   

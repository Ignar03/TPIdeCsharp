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
        [HttpGet]
        [ActionName("getTorneos")]
        [ProducesResponseType(typeof(IEnumerable<Torneo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Torneo>> getTorneos()
        {
            GeneralAdapterSQL consultor = new();

            DataTable respuesta = consultor.EjecutarVista("SELECT * FROM Torneos");

            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else
                {
                    List<Torneo> listadoCompleto = new();
                    try
                    {
                        foreach (DataRow registro in respuesta.Rows)
                        {
                            listadoCompleto.Add(new(registro));
                        }
                        return Ok(listadoCompleto);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Imposible creaar objeto, inconsistencia en los datos");
                        return Conflict("Error en la creación de los datos del objeto.");
                    }
                }
            }
            else return NoContent();
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
            GeneralAdapterSQL consultor = new();
            DataTable respuesta = consultor.EjecutarVista("SELECT * FROM Torneos WHERE torneo_activo = 1");
            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else
                {
                    try
                    {
                        List<Torneo> listadoActivos = new();
                        foreach (DataRow registro in respuesta.Rows)
                        {
                            listadoActivos.Add(new(registro));
                        }
                        return Ok(listadoActivos);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Imposible crear objeto, inconsistencia en los datos");
                        return Conflict("Ocurrio un error en la creacion de los datos");
                    }
                }
            }
            else return NoContent();
        }


        [HttpGet("${id}")]
        [ActionName("getTorneoId")]
        [ProducesResponseType(typeof(IEnumerable<Torneo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> getTorneoId(int id)
        {
            GeneralAdapterSQL consultor = new();
            DataTable respuesta = consultor.EjecutarVista($"SELECT * FROM Torneos WHERE id_torneo = {id}");

            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else
                {
                    try
                    {
                        Torneo busqueda = new(respuesta.Rows[0]);
                        return Ok(busqueda);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Error en la búsqueda del torneo");
                        return Conflict("Hubo un error en la creación de los datos");
                    }
                }
            }
            else return NoContent();
        }


        [HttpPost]
        [ActionName("postTorneo")]
        [ProducesResponseType(typeof(Torneo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> postTorneo([FromBody] Torneo nuevoTorneo)
        {
            GeneralAdapterSQL consultor = new();
            DataTable respuesta = consultor.EjecutarVista($@"
                INSERT INTO Torneos (id,nombre_torneo, id_region, fecha_inicio, fecha_fin, torneo_activo, minimo_medallas) 
                VALUES ('{nuevoTorneo.id}', '{nuevoTorneo.nombre_torneo}', {nuevoTorneo.id_region}, 
                        '{nuevoTorneo.fecha_inicio:yyyy-MM-dd}', 
                        '{nuevoTorneo.fecha_fin:yyyy-MM-dd}', 
                        {(nuevoTorneo.torneo_activo ? 1 : 0)}, {nuevoTorneo.minimo_medallas})");
            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else
                {
                    try
                    {
                        Torneo torneoCreado = new(respuesta.Rows[0]);
                        return Created("Torneo creado:", torneoCreado);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Error al crear el nuevo torneo.");
                        return Conflict("Error al crear los datos");
                    }
                }
            }
            else
            {
                ObjectResult resultado = new("Error")
                {
                    StatusCode = 418
                };
                return resultado;
            }
        }

        [HttpPut("{id_torneo}")]
        [ActionName("putTorneo")]
        [ProducesResponseType(typeof(Torneo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Torneo> putTorneo(int id_torneo, [FromBody] Torneo modificarTorneo)
        {
            GeneralAdapterSQL consultor = new();
            DataTable respuesta = consultor.EjecutarVista($"UPDATE Torneos SET" +
                $"id = '{modificarTorneo.id}', nombre_torneo = '{modificarTorneo.nombre_torneo}', " +
                $"id_region = '{modificarTorneo.id_region}', fecha_inicio = '{modificarTorneo.fecha_inicio:yyyy-MM-dd}', " +
                $"fecha_fin = '{modificarTorneo.fecha_fin:yyyy-MM-dd}', torneo_activo = '{(modificarTorneo.torneo_activo ? 1 : 0)}'" +
                $"minimo_medallas = '{modificarTorneo.minimo_medallas}' WHERE id_torneo = {id_torneo}");
            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else
                {
                    try
                    {
                        Torneo actualizado = new(respuesta.Rows[0]);
                        return Ok(actualizado);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Error al actualizar los datos del torneo.");
                        return Conflict("Error al actualizar los datos.");
                    }
                }
            }
            else return NoContent();
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
            GeneralAdapterSQL consultor = new();
            DataTable respuesta = consultor.EjecutarComando($"DELETE * FROM Torneos WHERE id_torneo = {id} ");
            if (respuesta.Rows.Count > 0)
            {
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else return BadRequest();
            }
            else return Ok("Torneo eliminado");
        }
    }
}   

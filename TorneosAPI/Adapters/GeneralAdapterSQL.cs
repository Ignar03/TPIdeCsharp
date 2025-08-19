using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using static Azure.Core.HttpHeader;
using TorneosAPI.Logs;

namespace TorneosAPI.Adapters
{
    public class GeneralAdapterSQL
    {
        public DataTable EjecutarVista(string vista)
        {
            //Es necesario ponerlo por fuera para poder usar el bloque finally 
            using SqlConnection conexionBase = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TorneosDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True");
            DataTable respuesta = new();
            try
            {
                //Con esto recupera la informacion de la vista 
                string consulta = "SELECT * FROM " + vista;
                using var comando = new SqlCommand(consulta, conexionBase);//Creamos el comando en la base con la conexion
                comando.CommandType = CommandType.Text;//Notificamos a la base que vamos a enviar
                
                SqlDataAdapter Adaptador = new(comando);
                conexionBase.Open(); //Abre la conexion (Puede fallar) 
                Adaptador.Fill(respuesta); //Contacta la base y ejecuta el comando 
            }
            catch (Exception ex)
            {
                //Registramos el error en nuestra carpeta 
                Logger.RegistrarERROR(ex, "Error al consultar la vista: " + vista);
                //Esta parte es para devolver un codigo de error al endpoint 
                respuesta.Columns.Add("RESULTADO");
                respuesta.Rows.Add("ERROR");
            }
            //Lo va a ejecutar no importa que parte del codigo realice 
            finally
            {
                //Limpia cualquier cadena de conexion que tengamos 
                SqlConnection.ClearAllPools();
                //Cierra la base de datos siempre 
                conexionBase.Close();
            }
            return respuesta;
        }
    }
}

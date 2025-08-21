using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TorneosAPI.Logs;

namespace TorneosAPI.Adapters
{
    public class GeneralAdapterSQL
    {
        private readonly string _connectionString;

        public GeneralAdapterSQL()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public DataTable EjecutarVista(string vista)
        {
            using SqlConnection conexionBase = new SqlConnection(_connectionString);
            DataTable respuesta = new();
            try
            {
                using var comando = new SqlCommand(vista, conexionBase);
                comando.CommandType = CommandType.Text;

                SqlDataAdapter Adaptador = new(comando);
                conexionBase.Open();
                Adaptador.Fill(respuesta);
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error al consultar la vista: " + vista);
                respuesta.Columns.Add("RESULTADO");
                respuesta.Rows.Add("ERROR");
            }
            finally
            {
                SqlConnection.ClearAllPools();
                conexionBase.Close();
            }
            return respuesta;
        }

        public int EjecutarComando(string com)
        {
            using SqlConnection conexionBase = new SqlConnection(_connectionString);
            try
            {
                using var comando = new SqlCommand(com, conexionBase);
                comando.CommandType = CommandType.Text;

                conexionBase.Open();
                return comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "Error al ejecutar comando:" + com);
                return -1;
            }
            finally
            {
                SqlConnection.ClearAllPools();
                conexionBase.Close();
            }
        }
    }
}

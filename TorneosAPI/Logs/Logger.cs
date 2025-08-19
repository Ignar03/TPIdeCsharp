using Microsoft.SqlServer.Server;

namespace TorneosAPI.Logs
{/// <summary> 
/// Esta clase esta creada con el proposito de almacenar mensajes en formato.txt dentro de la carpeta Logs
    /// </summary> 
    public class Logger
    {
        /// <summary> 
        /// Es el directorio donde vamos a almacenar la informacion. Recupera desde donde se esta ejecutando actualmente
        /// </summary> 
        private static string DirectoryPath = Environment.CurrentDirectory + "\\Logs";
        /// <summary> 
        /// Constructor vacio. 
        /// </summary> 
        public Logger() { }
        /// <summary> 
        /// Registra un error en la carpeta "Registros" 
        /// </summary> 
        /// <param name="ex">La excepcion que provoco el error</param> 
        /// <param name="error">El nombre del error</param> 
        public static void RegistrarERROR(Exception ex, string error)
        {
            //Prueba en Windows 
            DateTime actual = DateTime.Now;
            string fileName = DirectoryPath + "\\Registros\\" + actual.ToString("yyyy-MM-dd") + ".txt";
            //Sino existe crea la carpeta para registrar errores 
            if (!Directory.Exists(DirectoryPath + "\\Registros"))
                Directory.CreateDirectory(DirectoryPath + "\\Registros");

            using (var sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(actual + ", ERROR: " + error + ", MESSAGE EXCEPTION: " + ex.Message); 
            }
        }
        /// <summary> 
        /// Registra una Alerta ante situaciones inesperadas pero que noprovocan el fallo de la aplicacion
        /// </summary> 
        /// <param name="origen">Desde que parte del programa ocurrio</param> 
        /// <param name="comportamiento">Cual fue el comportamiento detectado</param>
        public static void RegistrarAnomalia(string origen, string comportamiento)
        {
            DateTime actual = DateTime.Now;
            string fileName = DirectoryPath + "\\Alertas\\" +
            actual.ToString("yyyy-MM-dd") + ".txt";
            using (var sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(actual + ", ANOMALIA: " + comportamiento + ", DESDE EL HOST: " + origen); 
            }
        }
    }
}


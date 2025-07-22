// Archivo: ConnectionConfig.cs
namespace CargaImagenes.Data
{
    /// <summary>
    /// Clase que representa la sección de configuración de conexión.
    /// </summary>
    public class ConnectionConfig
    {
        /// <summary>
        /// Cadena de conexión a la base de datos.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo máximo de espera para las operaciones de base de datos en segundos.
        /// </summary>
        public int CommandTimeout { get; set; } = 30;
    }
}

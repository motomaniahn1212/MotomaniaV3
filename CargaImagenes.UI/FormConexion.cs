using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace CargaImagenes.UI
{
    public partial class FormConexion : Form
    {
        public string ConnectionString { get; private set; } = string.Empty;

        private readonly string _configFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CargaImagenes",
            "dbconfig.json");

        public FormConexion()
        {
            InitializeComponent();
            CargarConfiguracionGuardada();
        }

        private void CargarConfiguracionGuardada()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    var configJson = File.ReadAllText(_configFilePath);
                    var config = JsonSerializer.Deserialize<DbConfig>(configJson);

                    if (config != null)
                    {
                        txtServidor.Text = config.Server;
                        txtBaseDatos.Text = config.Database;
                        txtUsuario.Text = config.User;
                        txtContrasena.Text = config.Password;
                        chkGuardarConfig.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar configuración: {ex.Message}");
            }
        }

        private bool ProbarConexion()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos
                if (string.IsNullOrWhiteSpace(txtServidor.Text) ||
                    string.IsNullOrWhiteSpace(txtBaseDatos.Text))
                {
                    MessageBox.Show("El servidor y la base de datos son obligatorios",
                        "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Construir y probar la cadena de conexión
                ConnectionString = ConstructConnectionString();
                if (!ProbarConexion())
                {
                    MessageBox.Show("No se pudo establecer la conexión con la base de datos",
                        "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Guardar configuración si está marcada la opción
                if (chkGuardarConfig.Checked)
                {
                    GuardarConfiguracion();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar conexión: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ConstructConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = txtServidor.Text,
                InitialCatalog = txtBaseDatos.Text,
                TrustServerCertificate = true
            };

            // Configurar la autenticación
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                builder.IntegratedSecurity = true; // Autenticación de Windows
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = txtUsuario.Text;
                builder.Password = txtContrasena.Text;
            }

            return builder.ConnectionString;
        }

        private void GuardarConfiguracion()
        {
            try
            {
                var config = new DbConfig
                {
                    Server = txtServidor.Text,
                    Database = txtBaseDatos.Text,
                    User = txtUsuario.Text,
                    Password = txtContrasena.Text
                };

                // Asegurar que el directorio existe
                var directorio = Path.GetDirectoryName(_configFilePath);
                if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                // Guardar como JSON
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                var configJson = JsonSerializer.Serialize(config, jsonOptions);
                File.WriteAllText(_configFilePath, configJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar configuración: {ex.Message}");
                // No mostramos mensaje al usuario, continuamos con la conexión
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    public class DbConfig
    {
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
using CargaImagenes.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CargaImagenes.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string? connectionString;
            using (var formConexion = new FormConexion())
            {
                if (formConexion.ShowDialog() != DialogResult.OK)
                    return;

                connectionString = formConexion.ConnectionString;
            }

            // 2) Construye el ServiceProvider
            var services = new ServiceCollection();

            // 2.1) Inyecta la configuración de conexión
            services.AddSingleton(new ConnectionConfig
            {
                ConnectionString = connectionString,
                CommandTimeout = 30
            });

            // 2.2) Registra tu DatabaseService
            services.AddTransient<IDatabaseService, DatabaseService>();

            // 2.3) Registra AppSettings
            var appSettings = new AppSettings
            {
                TempImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempImages"),
                DefaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images")
            };
            services.AddSingleton(appSettings);

            // 2.4) Registra tu formulario principal
            services.AddTransient<Form1>();

            var provider = services.BuildServiceProvider();

            // 3) Asegúrate de crear los directorios antes de arrancar el form
            Directory.CreateDirectory(appSettings.TempImagePath);
            Directory.CreateDirectory(appSettings.DefaultImagePath);

            // 4) Ejecuta el form con DI
            var mainForm = provider.GetRequiredService<Form1>();
            Application.Run(mainForm);
        }
    }
}
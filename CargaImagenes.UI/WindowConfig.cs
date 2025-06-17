using System.IO;
using System.Text.Json;

namespace CargaImagenes.UI
{
    public class WindowConfig
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CargaImagenes",
            "windowconfig.json");

        public static WindowConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    var cfg = JsonSerializer.Deserialize<WindowConfig>(json);
                    if (cfg != null)
                        return cfg;
                }
            }
            catch
            {
                // Ignorar errores de lectura
            }
            return new WindowConfig();
        }

        public static void Save(int width, int height)
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var cfg = new WindowConfig { Width = width, Height = height };
                var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch
            {
                // Ignorar errores de escritura
            }
        }
    }
}

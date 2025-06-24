using System.IO;
using Newtonsoft.Json;

namespace ThemeForge.Utilities
{
    public class SettingsManager
    {
        private const string FileName = "settings.json";
        public Settings Load(string dir)
        {
            var path = Path.Combine(dir, FileName);
            return File.Exists(path)
                ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path)) ?? new Settings()
                : new Settings();
        }
        public void Save(string dir, Settings settings)
        {
            var path = Path.Combine(dir, FileName);
            File.WriteAllText(path, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}

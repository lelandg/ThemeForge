using System;
using System.IO;
using System.Windows;
using ThemeForge.Utilities;
using Application = System.Windows.Application;

namespace ThemeForge
{
    public partial class App
    {
        private static SettingsManager _settingsManager;
        private static Settings _settings;

        public static SettingsManager SettingsManager => _settingsManager;
        public static Settings Settings => _settings;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize settings
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ThemeForge");

            // Ensure directory exists
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _settingsManager = new SettingsManager();
            _settings = _settingsManager.Load(appDataPath);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Save settings on application exit
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ThemeForge");

            _settingsManager.Save(appDataPath, _settings);

            base.OnExit(e);
        }
    }
}

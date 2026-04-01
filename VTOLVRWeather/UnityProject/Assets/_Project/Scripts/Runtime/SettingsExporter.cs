using System;
using System.IO;
using Enviro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DaftAppleGames.VTOLWeatherMod
{
    public class SettingsExporter : MonoBehaviour
    {
        public EnviroConfiguration settings;

        private void Awake()
        {
            ExportConfig();
        }
    
        private void ExportConfig()
        {
            ExportSettings(settings.timeModule);
            ExportSettings(settings.lightingModule);
            ExportSettings(settings.reflectionsModule);
            ExportSettings(settings.Sky);
            ExportSettings(settings.fogModule);
            ExportSettings(settings.volumetricCloudModule);
            ExportSettings(settings.flatCloudModule);
            ExportSettings(settings.Weather);
            ExportSettings(settings.Aurora);
            ExportSettings(settings.Audio);
            ExportSettings(settings.Effects);
            ExportSettings(settings.Lightning);
            ExportSettings(settings.Quality);

            foreach (EnviroQuality enviroQuality in settings.Quality.Settings.Qualities)
            {
                ExportSettings(enviroQuality, enviroQuality.name);
            }
            
            ExportSettings(settings.Environment);
        }

        private void ExportSettings(Object enviroModule, string postFix = "")
        {
            if (enviroModule == null)
            {
                Debug.LogWarning("Module is null - skipping.");
                return;
            }
            string moduleName = enviroModule.GetType().Name;
            string settingsFileName = moduleName + (String.IsNullOrEmpty(postFix) ? "" : postFix) + "Settings.json";
            var json = JsonUtility.ToJson(enviroModule);
            string settingsFilePath = Path.Combine("Assets/_Project/Settings/", settingsFileName);
            Debug.Log($"Exporting {moduleName} to {settingsFilePath}...");
            File.WriteAllText(settingsFilePath, json);
            Debug.Log($"Exporting {moduleName} complete.");
        }
    }
}

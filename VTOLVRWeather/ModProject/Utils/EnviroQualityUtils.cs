using Enviro;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public enum Quality
{
    Low,
    Medium,
    High,
    Ultra,
    Insane
}

public static class EnviroQualityUtils
{
    public static void Configure(EnviroManager enviroManager, Quality quality, string assetBundleName)
    {
        if (enviroManager.Quality == null)
        {
            Log("Quality module is null, skipping configuration.");
            return;
        }

        // Create a new ScriptableObject instance to receive the JSON data
        EnviroQuality enviroQuality = ScriptableObject.CreateInstance<EnviroQuality>();
        enviroQuality.auroraOverride = new EnviroAuroraQualitySettings();
        enviroQuality.effectsOverride = new EnviroEffectsQualitySettings();
        enviroQuality.flatCloudsOverride = new EnviroFlatCloudsQualitySettings();
        enviroQuality.skyOverride = new EnviroSkyQualitySettings();
        enviroQuality.volumetricCloudsOverride = new EnviroVolumetricCloudsQualitySettings();
        enviroQuality.fogOverride = new EnviroFogQualitySettings();
        
        switch (quality)
        {
            case Quality.Low:
            default:
                TextAsset lowJsonAsset =
                    ModUtils.LoadFromAssetBundle<TextAsset>(assetBundleName, "EnviroQualityLowSettings.json");
                string lowQualityJsonText = lowJsonAsset.text;
                JsonUtility.FromJsonOverwrite(lowQualityJsonText, enviroQuality);
                break;

            case Quality.Medium:
                TextAsset mediumJsonAsset =
                    ModUtils.LoadFromAssetBundle<TextAsset>(assetBundleName, "EnviroQualityMediumSettings.json");
                string mediumQualityJsonText = mediumJsonAsset.text;
                JsonUtility.FromJsonOverwrite(mediumQualityJsonText, enviroQuality);
                break;

            case Quality.High:
                TextAsset highJonAsset =
                    ModUtils.LoadFromAssetBundle<TextAsset>(assetBundleName, "EnviroQualityHighSettings.json");
                string highQualityJsonText = highJonAsset.text;
                JsonUtility.FromJsonOverwrite(highQualityJsonText, enviroQuality);
                break;
        }

        enviroManager.Quality.Settings.defaultQuality = enviroQuality;
        enviroManager.Quality.Settings.Qualities.Clear();
        enviroManager.Quality.Settings.Qualities.Add(enviroQuality);
    }
}
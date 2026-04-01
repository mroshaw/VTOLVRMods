using Enviro;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public static class EnviroVolumetricCloudUtils
{
    public static void Configure(EnviroManager enviroManager, string assetBundleName)
    {
        enviroManager.VolumetricClouds.settingsVolume = new EnviroCloudLayerSettings();
        enviroManager.VolumetricClouds.settingsGlobal = new EnviroCloudGlobalSettings();
        enviroManager.VolumetricClouds.settingsQuality = new EnviroVolumetricCloudsQuality();

        enviroManager.VolumetricClouds.settingsGlobal.sunLightColorGradient = new Gradient();
        enviroManager.VolumetricClouds.settingsGlobal.moonLightColorGradient = new Gradient();
        enviroManager.VolumetricClouds.settingsGlobal.ambientColorGradient = new Gradient();

        enviroManager.VolumetricClouds.settingsGlobal.noise =
            ModUtils.LoadFromAssetBundle<Texture3D>(assetBundleName, "enviro_clouds_base");
        enviroManager.VolumetricClouds.settingsGlobal.detailNoise =
            ModUtils.LoadFromAssetBundle<Texture3D>(assetBundleName, "enviro_clouds_detail");

        enviroManager.VolumetricClouds.settingsGlobal.curlTex =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_curl_new");
        enviroManager.VolumetricClouds.settingsGlobal.bottomsOffsetNoise =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_bottoms");
        enviroManager.VolumetricClouds.settingsGlobal.blueNoise =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_blueNoise");

        enviroManager.VolumetricClouds.settingsGlobal.customWeatherMap = null;
    }
}
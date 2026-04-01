using Enviro;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public static class EnviroFogUtils
{
    public static void Configure(EnviroManager enviroManager, string assetBundleName)
    {
        enviroManager.Fog.Settings = new EnviroFogSettings();
        
        enviroManager.Fog.Settings.scatteringMultiplier = new AnimationCurve();
        
        enviroManager.Fog.Settings.ambientColorGradient = new Gradient();
        
        // enviroManager.Fog.Settings.noise = ModUtils.LoadFromAssetBundle<Texture3D>(assetBundleName, "tex_enviro_noise");
        enviroManager.Fog.Settings.ditheringTex =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_dithering");
        enviroManager.Fog.Settings.unityFogColor = new Gradient();
    }
}
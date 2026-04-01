using Enviro;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public static class EnviroSkyUtils
{
    public static void Configure(EnviroManager enviroManager, string assetBundleName)
    {
        enviroManager.Sky.Settings = new EnviroSky();
        enviroManager.Sky.Settings.backColorGradient0 = new Gradient();
        enviroManager.Sky.Settings.backColorGradient1 = new Gradient();
        enviroManager.Sky.Settings.backColorGradient2 = new Gradient();
        enviroManager.Sky.Settings.backColorGradient3 = new Gradient();
        enviroManager.Sky.Settings.backColorGradient4 = new Gradient();
        enviroManager.Sky.Settings.backColorGradient5 = new Gradient();
        enviroManager.Sky.Settings.frontColorGradient0 = new Gradient();
        enviroManager.Sky.Settings.frontColorGradient1 = new Gradient();
        enviroManager.Sky.Settings.frontColorGradient2 = new Gradient();
        enviroManager.Sky.Settings.frontColorGradient3 = new Gradient();
        enviroManager.Sky.Settings.frontColorGradient4 = new Gradient();
        enviroManager.Sky.Settings.frontColorGradient5 = new Gradient();
        enviroManager.Sky.Settings.sunDiscColorGradient = new Gradient();
        enviroManager.Sky.Settings.moonColorGradient = new Gradient();
        enviroManager.Sky.Settings.moonGlowColorGradient = new Gradient();

        enviroManager.Sky.Settings.mieScatteringIntensityCurve = new AnimationCurve();
        enviroManager.Sky.Settings.moonGlowIntensityCurve = new AnimationCurve();
        enviroManager.Sky.Settings.starIntensityCurve = new AnimationCurve();
        enviroManager.Sky.Settings.galaxyIntensityCurve = new AnimationCurve();

        enviroManager.Sky.Settings.starsTex =
            ModUtils.LoadFromAssetBundle<Cubemap>(assetBundleName, "cube_enviro_stars");
        enviroManager.Sky.Settings.starsTwinklingTex =
            ModUtils.LoadFromAssetBundle<Cubemap>(assetBundleName, "cube_enviro_starsNoise");
        enviroManager.Sky.Settings.galaxyTex =
            ModUtils.LoadFromAssetBundle<Cubemap>(assetBundleName, "cube_enviro_galaxy");

        enviroManager.Sky.Settings.sunTex =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "Enviro_Sun_Tex");
        enviroManager.Sky.Settings.moonTex =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_moon_standard");
        enviroManager.Sky.Settings.moonGlowTex =
            ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_moonGlowNew");
    }
}
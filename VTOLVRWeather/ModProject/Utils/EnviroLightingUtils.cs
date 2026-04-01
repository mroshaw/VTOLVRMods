using Enviro;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public static class EnviroLightingUtils
{
    public static void Configure(EnviroManager enviroManager)
    {
        enviroManager.Lighting.Settings = new EnviroLighting();

        enviroManager.Lighting.Settings.sunIntensityCurve = new AnimationCurve();
        enviroManager.Lighting.Settings.moonIntensityCurve = new AnimationCurve();

        enviroManager.Lighting.Settings.sunIntensityCurveHDRP = new AnimationCurve();
        enviroManager.Lighting.Settings.moonIntensityCurveHDRP = new AnimationCurve();
        enviroManager.Lighting.Settings.lightColorTemperatureHDRP = new AnimationCurve();

        enviroManager.Lighting.Settings.sceneExposure = new AnimationCurve();
        enviroManager.Lighting.Settings.diffuseIndirectIntensity = new AnimationCurve();
        enviroManager.Lighting.Settings.reflectionIndirectIntensity = new AnimationCurve();
        enviroManager.Lighting.Settings.ambientIntensityCurve = new AnimationCurve();

        enviroManager.Lighting.Settings.sunColorGradient = new Gradient();
        enviroManager.Lighting.Settings.sunColorGradient = new Gradient();
        enviroManager.Lighting.Settings.sunColorGradient = new Gradient();

        enviroManager.Lighting.Settings.ambientSkyColorGradient = new Gradient();
        enviroManager.Lighting.Settings.ambientEquatorColorGradient = new Gradient();
        enviroManager.Lighting.Settings.ambientGroundColorGradient = new Gradient();
    }
}
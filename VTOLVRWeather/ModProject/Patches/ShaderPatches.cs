global using static DaftAppleGames.WeatherMod.Logger;
using System.Collections.Generic;
using DaftAppleGames.WeatherMod.Utils;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Patches;

[HarmonyPatch(typeof(Shader))] public static class ShaderPatches
{
    // Where we know the shader requested has a different file name, we can pick it up via a mapping
    private static Dictionary<string, string> _shaderMapping = new Dictionary<string, string>
    {
        { "Enviro3/Standard/WeatherTexture", "EnviroWeatherMap" },
        { "Hidden/EnviroBlur", "EnviroBilateralBlur"},
        { "Hidden/EnviroCloudsRaymarch", "EnviroVolumetricCloudsRaymarch" }
    };
    
    [HarmonyPatch(nameof(Shader.Find))]
    [HarmonyPrefix]
    static bool Find_Prefix(string name, ref Shader __result)
    {
        Log($"In ShaderFind, intercepted request for {name}...");

        // See if there's a lookup mapping
        if (_shaderMapping.TryGetValue(name, out var value))
        {
            Log($"Found explicit mapping of {name} to {value}...");
            name = value;
        }
        
        // Try loading based on shader name
        string shaderName = name.Substring(name.LastIndexOf('/') + 1);
        Shader shader = ModUtils.LoadFromAssetBundle<Shader>(WeatherMod.AssetBundleName, shaderName);

        // If this fails, try again with an 'Enviro' prefix
        if (shader == null)
        {
            string enviroShaderName = "Enviro" + shaderName;
            Log($"Shader not found as {shaderName}. Try again as {enviroShaderName}...");
            shader = ModUtils.LoadFromAssetBundle<Shader>(WeatherMod.AssetBundleName, enviroShaderName);
        }

        if (shader != null)
        {
            Log($"Loaded shader {name} from AssetBundle!");
            __result = shader;
            return false;
        }

        // Log($"Failed to load shader {name} from AssetBundle, falling through to original.");
        return true;
    }
}
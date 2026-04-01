global using static DaftAppleGames.WeatherMod.Logger;
using System;
using DaftAppleGames.WeatherMod.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DaftAppleGames.WeatherMod.Patches;

/// <summary>
/// Intercept Resources.Load() and retarget to our AssetBundle
/// </summary>
[HarmonyPatch] public static class ResourcesPatches
{
    static System.Reflection.MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(Resources), nameof(Resources.Load),
            new Type[] { typeof(string), typeof(Type) });
    }

    static bool Prefix(string path, Type systemTypeInstance, ref Object __result)
    {
        string assetName = System.IO.Path.GetFileName(path);
        Log($"In ResourcesLoad, intercepted request for {path}...");
        Log($"Loading {assetName} from AssetBundle...");
        Object asset = ModUtils.LoadFromAssetBundle<Object>(WeatherMod.AssetBundleName, assetName);

        if (asset != null)
        {
            Log($"Loaded {assetName} from AssetBundle!");
            __result = asset;
            return false;
        }

        // Log($"Failed to load {assetName} from AssetBundle!");
        return true;
    }
}
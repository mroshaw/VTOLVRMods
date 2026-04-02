global using static DaftAppleGames.WeatherMod.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public static class ModUtils
{
    private static readonly Dictionary<String, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();
    internal static readonly string ModFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Explicitly loads DLL
    /// </summary>
    public static void LoadAssembly(string assemblyName)
    {
        string assemblyPath = Path.Combine(ModFolder, assemblyName);
        if (File.Exists(assemblyPath))
        {
            Assembly.LoadFrom(assemblyPath);
            Log($"{assemblyName} assembly loaded");
        }
        else
        {
            LogError($"{assemblyName} not found at: {assemblyPath}");
        }
    }

    /// <summary>
    /// Load an asset bundle and cache it in the dictionary
    /// </summary>
    private static void LoadAssetBundle(string assetBundleName)
    {
        Log("Attempting to load asset bundle: " + assetBundleName);
        // Check asset file exists
        String assetBundleFilePath = Path.Combine(ModFolder, assetBundleName);
        if (!File.Exists(assetBundleFilePath))
        {
            LogError($"AssetBundle not found at: {assetBundleFilePath}");
            return;
        }

        Log("Found asset bundle: " + assetBundleFilePath);
        AssetBundle bundle = AssetBundle.LoadFromFile(assetBundleFilePath);
        LoadedBundles.Add(assetBundleName, bundle);
        Log("Loaded asset bundle: " + assetBundleName);
    }

    /// <summary>
    /// Loads an asset of type T from the named asset bundle. The bundle is loaded if it's not already found in the
    /// bundle cache
    /// </summary>
    public static T LoadFromAssetBundle<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
    {
        Log($"Attempting to load asset {assetName} from {assetBundleName}...");
        if (!LoadedBundles.ContainsKey(assetBundleName))
        {
            LoadAssetBundle(assetBundleName);
        }

        AssetBundle assetBundle = LoadedBundles[assetBundleName];
        if (!assetBundle)
        {
            LogError($"AssetBundle: {assetBundle} could not be loaded!");
        }

        T asset = assetBundle.LoadAsset<T>(assetName);
        if (!asset)
        {
            LogWarn($"Could not load asset: {assetName}");
            return null;
        }

        Log($"Loaded asset: {assetName} from {assetBundleName}");
        return asset;
    }
}
global using static DaftAppleGames.WeatherMod.Logger;
using DaftAppleGames.WeatherMod.Utils;
using Enviro;
using HarmonyLib;
using ModLoader.Framework;
using ModLoader.Framework.Attributes;
using OC;
using UnityEngine;

namespace DaftAppleGames.WeatherMod;

[ItemId("DaftAppleGames.WeatherMod")] public class WeatherMod : VtolMod
{
    private const string EnviroAssemblyName = "Enviro3.Runtime.dll";
    internal const string AssetBundleName = "enviroassetbundle";
    private const string SkyPrefabName = "EnviroSky.prefab";

    private GameObject _skyInstance;

    /// <summary>
    /// Init AssetBundles and prepare sky for spawning
    /// </summary>
    private void Awake()
    {
        Log($"Awake at {ModUtils.ModFolder}");
        ModUtils.LoadAssembly(EnviroAssemblyName);

        Harmony harmony = new Harmony("com.DaftAppleGames.VtolWeatherMod");
        harmony.PatchAll();

        Log($"Getting Sky prefab instance from: {AssetBundleName}");
        GameObject skyPrefab = ModUtils.LoadFromAssetBundle<GameObject>(AssetBundleName, SkyPrefabName);
        skyPrefab.SetActive(false);

        if (!skyPrefab)
        {
            LogError("Failed to load Sky Prefab! Aborting!");
            return;
        }

        // Init sky
        Log("Instantiating Sky instance...");
        _skyInstance = Instantiate(skyPrefab, Vector3.zero, Quaternion.identity);
        Log("Setting Sky Instance to active...");
        if (ConfigureSky())
        {
            // _skyInstance.SetActive(true);
            Log("Sky setup complete");
        }
        else
        {
            LogError("Failed to setup sky! Prefab instance will not be activated!");
        }
    }

    /// <summary>
    /// Configure the new Sky, replacing the OverCloud sky
    /// </summary>
    /// <returns></returns>
    private bool ConfigureSky()
    {
        Log("Configuring Sky...");
        EnviroManager enviroManager = _skyInstance.GetComponent<EnviroManager>();
        if (!enviroManager)
        {
            LogError("EnviroManager not found!");
            return false;
        }

        enviroManager.Camera = Camera.main;

        OverCloud overCloud = FindFirstObjectByType<OverCloud>();
        if (!overCloud)
        {
            LogError("OverCloud not found!");
            return false;
        }

        GameObject sun = overCloud.transform.Find("Sun").gameObject;
        GameObject moon = overCloud.transform.Find("Moon").gameObject;

        enviroManager.Objects.directionalLight = sun.GetComponent<Light>();
        enviroManager.Objects.additionalDirectionalLight = moon.GetComponent<Light>();
        EnviroReflectionProbe reflectionProbe = _skyInstance.GetComponentInChildren<EnviroReflectionProbe>(true);
        enviroManager.Objects.globalReflectionProbe = reflectionProbe;

        if (!ConfigureEnviroSettings())
        {
            return false;
        }

        Log("Sky configured!");
        return true;
    }

    /// <summary>
    /// Loads serialized JSON settings from AssetBundle. Required as the settings ScriptableObject instance assets
    /// from Enviro do not persist properly in AssetBundles
    /// </summary>
    private bool ConfigureEnviroSettings()
    {
        EnviroManager enviroManager = _skyInstance.GetComponent<EnviroManager>();

        enviroManager.Events = new EnviroEvents();
        enviroManager.configuration = null;

        LoadSettings(enviroManager, typeof(EnviroAudioModule));
        LoadSettings(enviroManager, typeof(EnviroAuroraModule));
        LoadSettings(enviroManager, typeof(EnviroEffectsModule));

        LoadSettings(enviroManager, typeof(EnviroEnvironmentModule));
        LoadSettings(enviroManager, typeof(EnviroFlatCloudsModule));
        LoadSettings(enviroManager, typeof(EnviroFogModule));
        LoadSettings(enviroManager, typeof(EnviroLightingModule));
        LoadSettings(enviroManager, typeof(EnviroLightningModule));
        LoadSettings(enviroManager, typeof(EnviroQualityModule));

        LoadSettings(enviroManager, typeof(EnviroReflectionsModule));
        LoadSettings(enviroManager, typeof(EnviroSkyModule));
        LoadSettings(enviroManager, typeof(EnviroTimeModule));
        LoadSettings(enviroManager, typeof(EnviroVolumetricCloudsModule));
        LoadSettings(enviroManager, typeof(EnviroWeatherModule));

        Log("Loaded Enviro preset settings successfully!");
        return true;
    }

    /// <summary>
    /// Loads a module settings instance from a JSON file supplied in the AssetBundle
    /// </summary>
    private void LoadSettings(EnviroManager enviroManager, System.Type moduleType)
    {
        if (!enviroManager)
        {
            LogError("EnviroManager is NULL!");
            return;
        }

        // Derive settings file from module type
        string moduleName = moduleType.ToString().Replace("Enviro.", "");
        string settingsFileName = moduleName + "Settings.json";

        Log($"Loading {moduleName} settings from: {settingsFileName}");
        TextAsset jsonAsset = ModUtils.LoadFromAssetBundle<TextAsset>(AssetBundleName, settingsFileName);
        Log($"Successfully loaded JSON. Deserializing {moduleName}");

        // Strip asset references to avoid NREs on load
        string settingsText = StripAssetReferences(jsonAsset.text);

        switch (moduleName)
        {
            case "EnviroAudioModule":
                if (!enviroManager.Audio)
                {
                    Log("Audio module is null. Skipping settings import");
                    return;
                }

                enviroManager.Audio.Settings = new EnviroAudio();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Audio);
                break;

            case "EnviroAuroraModule":
                if (!enviroManager.Aurora)
                {
                    Log("Aurora module is null. Skipping settings import");
                    return;
                }

                enviroManager.Aurora.Settings = new EnviroAurora();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Aurora);
                break;

            case "EnviroEffectsModule":
                if (!enviroManager.Effects)
                {
                    Log("Effects module is null. Skipping settings import");
                    return;
                }

                enviroManager.Effects.Settings = new EnviroEffects();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Effects);
                break;

            case "EnviroEnvironmentModule":
                if (!enviroManager.Environment)
                {
                    Log("Environment module is null. Skipping settings import");
                    return;
                }

                enviroManager.Environment.Settings = new EnviroEnvironment();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Environment);
                break;

            case "EnviroFlatCloudsModule":
                if (!enviroManager.FlatClouds)
                {
                    Log("FlatClouds module is null. Skipping settings import");
                    return;
                }

                enviroManager.FlatClouds.settings = new EnviroFlatClouds();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.FlatClouds);
                break;

            case "EnviroFogModule":
                if (!enviroManager.Fog)
                {
                    Log("Fog module is null. Skipping settings import");
                    return;
                }

                EnviroFogUtils.Configure(enviroManager, AssetBundleName);
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Fog);
                break;

            case "EnviroLightingModule":
                if (!enviroManager.Lighting)
                {
                    Log("Lighting module is null. Skipping settings import");
                    return;
                }

                EnviroLightingUtils.Configure(enviroManager);
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Lighting);
                break;

            case "EnviroLightningModule":
                if (!enviroManager.Lightning)
                {
                    Log("Lightning module is null. Skipping settings import");
                    return;
                }

                enviroManager.Lightning.Settings = new EnviroLightning();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Lightning);
                break;

            case "EnviroQualityModule":
                if (!enviroManager.Quality)
                {
                    Log("Quality module is null. Skipping settings import");
                    return;
                }

                enviroManager.Quality.Settings = new EnviroQualities();
                EnviroQualityUtils.Configure(enviroManager, Quality.Low, AssetBundleName);
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Quality);
                break;

            case "EnviroReflectionsModule":
                if (!enviroManager.Reflections)
                {
                    Log("Reflections module is null. Skipping settings import");
                    return;
                }

                enviroManager.Reflections.Settings = new EnviroReflections();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Reflections);
                break;

            case "EnviroSkyModule":
                if (!enviroManager.Sky)
                {
                    Log("Sky module is null. Skipping settings import");
                    return;
                }

                EnviroSkyUtils.Configure(enviroManager, AssetBundleName);
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Sky);
                break;

            case "EnviroTimeModule":
                if (!enviroManager.Time)
                {
                    Log("Time module is null. Skipping settings import");
                    return;
                }

                enviroManager.Time.Settings = new EnviroTime();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Time);
                Log($"Successfully set Time Settings. e.g. {enviroManager.Time.Settings.calenderType}");
                break;

            case "EnviroVolumetricCloudsModule":
                if (!enviroManager.VolumetricClouds)
                {
                    Log("Volumetric clouds module is null. Skipping settings import");
                    return;
                }

                EnviroVolumetricCloudUtils.Configure(enviroManager, AssetBundleName);
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.VolumetricClouds);
                break;
            case "EnviroWeatherModule":
                if (!enviroManager.Weather)
                {
                    Log("Weather module is null. Skipping settings import");
                    return;
                }

                enviroManager.Weather.Settings = new EnviroWeather();
                JsonUtility.FromJsonOverwrite(settingsText, enviroManager.Weather);
                break;
            default:
                LogError($"Unknown module name: {moduleName}");
                break;
        }
    }

    private string StripAssetReferences(string json)
    {
        // Replace {"instanceID":XXXXXX} with {"instanceID":0} to null out asset references
        return System.Text.RegularExpressions.Regex.Replace(
            json,
            @"\{""instanceID"":-?\d+\}",
            @"{""instanceID"":0}"
        );
    }

    /// <summary>
    /// Clean up when mod is unloaded
    /// </summary>
    public override void UnLoad()
    {
        if (_skyInstance != null)
        {
            Destroy(_skyInstance);
            _skyInstance = null;
        }
    }
}
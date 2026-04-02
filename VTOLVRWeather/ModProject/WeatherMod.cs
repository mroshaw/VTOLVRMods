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

    internal static WeatherMod Instance { get; private set; }

    private GameObject _skyInstance;
    private EnviroManager _enviroManager;

    private OverCloud _overCloud;
    private EnvironmentManager _environmentManager;
    private Light _overCloudSunLight;
    private Light _overCloudMoonLight;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        Log($"Awake at {ModUtils.ModFolder}");
        ModUtils.LoadAssembly(EnviroAssemblyName);

        Harmony harmony = new Harmony("com.DaftAppleGames.VtolWeatherMod");
        harmony.PatchAll();

        Log("Getting references...");
        if (!FindReferences())
        {
            LogError("Could not find OverCloud or EnvironmentManager. Aborting!");
            return;
        }

        Log($"Getting Sky prefab instance from: {AssetBundleName}");
        GameObject skyPrefab = ModUtils.LoadFromAssetBundle<GameObject>(AssetBundleName, SkyPrefabName);

        if (!skyPrefab)
        {
            LogError("Failed to load Sky Prefab! Aborting!");
            return;
        }

        skyPrefab.SetActive(false);

        Log("Instantiating Sky instance...");
        _skyInstance = Instantiate(skyPrefab, Vector3.zero, Quaternion.identity);
        _enviroManager = _skyInstance.GetComponent<EnviroManager>();

        if (!_enviroManager)
        {
            LogError("EnviroManager not found! Aborting!");
            return;
        }

        if (!ConfigureSky())
        {
            LogError("Failed to configure sky! Aborting!");
            return;
        }

        if (!ConfigureModules())
        {
            LogError("Failed to initially configure modules!");
            return;
        }

        Log("Disabling OverCloud...");
        if (!DisableOverCloud())
        {
            LogError("Failed to disable OverCloud! Aborting!");
            return;
        }

        Log("Configuring Main Camera");
        if (!ConfigureCamera(Camera.main))
        {
            LogError("Failed to configure camera! Aborting!");
            return;
        }

        Log("Setting Sky Instance to active...");
        _skyInstance.SetActive(true);
        Log("Sky setup complete");
    }

    /// <summary>
    /// Find and set existing sky related references
    /// </summary>
    /// <returns></returns>
    private bool FindReferences()
    {
        _overCloud = FindFirstObjectByType<OverCloud>();
        if (!_overCloud)
        {
            LogError("OverCloud not found!");
            return false;
        }

        _environmentManager = FindFirstObjectByType<EnvironmentManager>();
        if (!_environmentManager)
        {
            LogError("EnvironmentManager not found!");
            return false;
        }

        _overCloudSunLight = _overCloud.m_OverCloudSun._light;
        if (!_overCloudSunLight)
        {
            LogError("Could not find overCloud sunlight!");
            return false;
        }

        _overCloudMoonLight = _overCloud.m_OverCloudMoon._light;
        if (!_overCloudMoonLight)
        {
            LogWarn("Could not find overCloudMoonlight!");
        }

        return true;
    }

    private bool DisableOverCloud()
    {
        Log("Disabling OverCloud...");
        _overCloud.gameObject.SetActive(false);

        Log("Disabling EnvironmentManager...");
        _environmentManager.gameObject.SetActive(false);

        Log("Done disabling OverCloud!");
        return true;
    }

    private bool ConfigureCamera(Camera camera)
    {
        if (!camera)
        {
            LogError("Camera is null!");
            return false;
        }

        Log($"Configuring Camera: {camera.gameObject.name}");
        SetupOCOnCamera ocSetup = camera.GetComponent<SetupOCOnCamera>();
        if (ocSetup)
        {
            Log("Disabling SetupOCOnCamera...");
            ocSetup.enabled = false;
        }

        OverCloudCamera ocCamera = camera.GetComponent<OverCloudCamera>();
        if (ocCamera)
        {
            Log("Disabling OverCloudCamera...");
            ocCamera.enabled = false;
        }

        CameraFogSettings fogSettings = camera.GetComponent<CameraFogSettings>();
        if (fogSettings)
        {
            Log("Disabling CameraFogSettings....");
            fogSettings.enabled = false;
        }

        if (!camera.TryGetComponent<EnviroRenderer>(out EnviroRenderer enviroRenderer))
        {
            enviroRenderer = camera.gameObject.AddComponent<EnviroRenderer>();
            Log("Added EnviroRenderer to camera");
        }

        if (!enviroRenderer)
        {
            LogError("Failed to add EnviroRenderer to camera!");
            return false;
        }

        Log("Camera configuration complete!");

        return true;
    }

    private bool ConfigureSky()
    {
        Log("Configuring Sky...");

        if (!ConfigureGeneralSettings())
            return false;

        Log("Sky configured successfully!");
        return true;
    }

    /// <summary>
    /// Sets high level EnviroManager parameters
    /// </summary>
    private bool ConfigureGeneralSettings()
    {
        Log("Configuring general settings...");

        Log("Setting Configuration...");
        _enviroManager.configuration =
            ModUtils.LoadFromAssetBundle<EnviroConfiguration>(AssetBundleName, "EnviroSettings.asset");

        _enviroManager.Camera = Camera.main;
        _enviroManager.Events = new EnviroEvents();
        _enviroManager.configuration = null;

        Log("Setting Sun, Moon and Stars...");
        GameObject sun = _enviroManager.transform.Find("Sun").gameObject;
        GameObject moon = _enviroManager.transform.Find("Moon").gameObject;
        GameObject stars = _enviroManager.transform.Find("Stars").gameObject;

        if (sun == null || moon == null || stars == null)
        {
            LogError($"Could not find Sun {sun}, Moon {moon}, or Stars {stars}!");
            return false;
        }

        _enviroManager.Objects.sun = sun;
        _enviroManager.Objects.moon = moon;
        _enviroManager.Objects.stars = stars;

        Log("Setting Directional Light...");
        _enviroManager.Objects.directionalLight = _enviroManager.GetComponentInChildren<Light>(true);
        _enviroManager.Objects.directionalLight.cullingMask = _overCloudSunLight.cullingMask;
        _enviroManager.Objects.directionalLight.renderingLayerMask = _overCloudSunLight.cullingMask;

        Log("Setting Reflection Probe...");
        _enviroManager.Objects.globalReflectionProbe = _skyInstance.GetComponentInChildren<EnviroReflectionProbe>(true);

        return true;
    }

    internal bool ConfigureModules()
    {
        Log("Configuring modules...");

        return
            ConfigureAudioModule() &&
            ConfigureAuroraModule() &&
            ConfigureEffectsModule() &&
            ConfigureEnvironmentModule() &&
            ConfigureFlatCloudsModule() &&
            ConfigureFogModule() &&
            ConfigureLightingModule() &&
            ConfigureLightningModule() &&
            ConfigureQualityModule() &&
            ConfigureReflectionsModule() &&
            ConfigureSkyModule() &&
            ConfigureTimeModule() &&
            ConfigureVolumetricCloudsModule() &&
            ConfigureWeatherModule();
    }

    private bool ConfigureAudioModule()
    {
        if (_enviroManager.Audio == null)
            _enviroManager.Audio = ScriptableObject.CreateInstance<EnviroAudioModule>();

        _enviroManager.Audio.Settings = new EnviroAudio();
        return ApplyJsonSettings(_enviroManager.Audio, "EnviroAudioModuleSettings.json");
    }

    private bool ConfigureAuroraModule()
    {
        if (_enviroManager.Aurora == null)
            _enviroManager.Aurora = ScriptableObject.CreateInstance<EnviroAuroraModule>();

        _enviroManager.Aurora.Settings = new EnviroAurora();
        return ApplyJsonSettings(_enviroManager.Aurora, "EnviroAuroraModuleSettings.json");
    }

    private bool ConfigureEffectsModule()
    {
        if (_enviroManager.Effects == null)
            _enviroManager.Effects = ScriptableObject.CreateInstance<EnviroEffectsModule>();

        _enviroManager.Effects.Settings = new EnviroEffects();
        return ApplyJsonSettings(_enviroManager.Effects, "EnviroEffectsModuleSettings.json");
    }

    private bool ConfigureEnvironmentModule()
    {
        if (_enviroManager.Environment == null)
            _enviroManager.Environment = ScriptableObject.CreateInstance<EnviroEnvironmentModule>();

        _enviroManager.Environment.Settings = new EnviroEnvironment();
        return ApplyJsonSettings(_enviroManager.Environment, "EnviroEnvironmentModuleSettings.json");
    }

    private bool ConfigureFlatCloudsModule()
    {
        if (_enviroManager.FlatClouds == null)
            _enviroManager.FlatClouds = ScriptableObject.CreateInstance<EnviroFlatCloudsModule>();

        _enviroManager.FlatClouds.settings = new EnviroFlatClouds();
        EnviroFlatCloudUtils.Configure(_enviroManager, AssetBundleName);
        return ApplyJsonSettings(_enviroManager.FlatClouds, "EnviroFlatCloudsModuleSettings.json");
    }

    private bool ConfigureFogModule()
    {
        if (_enviroManager.Fog == null)
            _enviroManager.Fog = ScriptableObject.CreateInstance<EnviroFogModule>();

        EnviroFogUtils.Configure(_enviroManager, AssetBundleName);
        return ApplyJsonSettings(_enviroManager.Fog, "EnviroFogModuleSettings.json");
    }

    private bool ConfigureLightingModule()
    {
        if (_enviroManager.Lighting == null)
            _enviroManager.Lighting = ScriptableObject.CreateInstance<EnviroLightingModule>();

        EnviroLightingUtils.Configure(_enviroManager);
        return ApplyJsonSettings(_enviroManager.Lighting, "EnviroLightingModuleSettings.json");
    }

    private bool ConfigureLightningModule()
    {
        if (_enviroManager.Lightning == null)
            _enviroManager.Lightning = ScriptableObject.CreateInstance<EnviroLightningModule>();

        _enviroManager.Lightning.Settings = new EnviroLightning();
        return ApplyJsonSettings(_enviroManager.Lightning, "EnviroLightningModuleSettings.json");
    }

    private bool ConfigureQualityModule()
    {
        if (_enviroManager.Quality == null)
            _enviroManager.Quality = ScriptableObject.CreateInstance<EnviroQualityModule>();

        _enviroManager.Quality.Settings = new EnviroQualities();
        EnviroQualityUtils.Configure(_enviroManager, Quality.Low, AssetBundleName);
        return ApplyJsonSettings(_enviroManager.Quality, "EnviroQualityModuleSettings.json");
    }

    private bool ConfigureReflectionsModule()
    {
        if (_enviroManager.Reflections == null)
            _enviroManager.Reflections = ScriptableObject.CreateInstance<EnviroReflectionsModule>();

        _enviroManager.Reflections.Settings = new EnviroReflections();
        return ApplyJsonSettings(_enviroManager.Reflections, "EnviroReflectionsModuleSettings.json");
    }

    private bool ConfigureSkyModule()
    {
        if (_enviroManager.Sky == null)
            _enviroManager.Sky = ScriptableObject.CreateInstance<EnviroSkyModule>();

        EnviroSkyUtils.Configure(_enviroManager, AssetBundleName);
        if (!ApplyJsonSettings(_enviroManager.Sky, "EnviroSkyModuleSettings.json"))
        {
            return false;
        }

        EnviroSkyUtils.PostConfigure(_enviroManager);
        return true;
    }

    private bool ConfigureTimeModule()
    {
        if (_enviroManager.Time == null)
            _enviroManager.Time = ScriptableObject.CreateInstance<EnviroTimeModule>();

        EnviroTimeUtils.Configure(_enviroManager);
        if (!ApplyJsonSettings(_enviroManager.Time, "EnviroTimeModuleSettings.json"))
        {
            return false;
        }

        EnviroTimeUtils.PostConfigure(_enviroManager);
        return true;
    }

    private bool ConfigureVolumetricCloudsModule()
    {
        if (_enviroManager.VolumetricClouds == null)

            _enviroManager.VolumetricClouds = ScriptableObject.CreateInstance<EnviroVolumetricCloudsModule>();

        EnviroVolumetricCloudUtils.Configure(_enviroManager, AssetBundleName);
        return ApplyJsonSettings(_enviroManager.VolumetricClouds, "EnviroVolumetricCloudsModuleSettings.json");
    }

    private bool ConfigureWeatherModule()
    {
        if (_enviroManager.Weather == null)
            _enviroManager.Weather = ScriptableObject.CreateInstance<EnviroWeatherModule>();

        _enviroManager.Weather.Settings = new EnviroWeather();
        return ApplyJsonSettings(_enviroManager.Weather, "EnviroWeatherModuleSettings.json");
    }

    /// <summary>
    /// Checks that a module is not null
    /// </summary>
    private bool CheckModule(object module, string moduleName)
    {
        if (module == null)
        {
            LogError($"{moduleName} module is null!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads JSON settings from AssetBundle and applies them to the given module
    /// </summary>
    private bool ApplyJsonSettings(object module, string settingsFileName)
    {
        Log($"Loading settings from: {settingsFileName}");
        TextAsset jsonAsset = ModUtils.LoadFromAssetBundle<TextAsset>(AssetBundleName, settingsFileName);

        if (!jsonAsset)
        {
            LogError($"Failed to load {settingsFileName} from AssetBundle!");
            return false;
        }

        string settingsText = StripAssetReferences(jsonAsset.text);
        JsonUtility.FromJsonOverwrite(settingsText, module);
        Log($"Successfully applied settings from {settingsFileName}");
        return true;
    }

    private string StripAssetReferences(string json)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            json,
            @"\{""instanceID"":-?\d+\}",
            @"{""instanceID"":0}"
        );
    }

    public override void UnLoad()
    {
        if (_skyInstance != null)
        {
            Destroy(_skyInstance);
            _skyInstance = null;
        }
    }
}
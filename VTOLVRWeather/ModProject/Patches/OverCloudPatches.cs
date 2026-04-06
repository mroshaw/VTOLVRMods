using HarmonyLib;
using OC;

namespace DaftAppleGames.WeatherMod.Patches;

[HarmonyPatch(typeof(OverCloud))] public class OverCloudPatches
{
    [HarmonyPatch(nameof(OverCloud.Awake))]
    [HarmonyPrefix]
    static bool Awake_Prefix(OverCloud __instance)
    {
        // Disable component
        __instance.enabled = false;
        return false;
    }

    [HarmonyPatch(nameof(OverCloud.Render))]
    [HarmonyPrefix]
    static bool Render_Prefix(OverCloud __instance)
    {
        // Intercept any call to Render and block
        return false;
    }
}
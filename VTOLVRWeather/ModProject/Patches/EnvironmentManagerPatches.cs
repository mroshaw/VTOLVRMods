using HarmonyLib;

namespace DaftAppleGames.WeatherMod.Patches;

[HarmonyPatch(typeof(EnvironmentManager))] public class EnvironmentManagerPatches
{
    [HarmonyPatch(nameof(EnvironmentManager.Awake))]
    [HarmonyPrefix]
    static bool Awake_Prefix(EnvironmentManager __instance)
    {
        // Disable component
        __instance.enabled = false;
        return false;
    }
}
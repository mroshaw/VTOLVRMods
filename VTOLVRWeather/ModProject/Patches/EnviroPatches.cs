using Enviro;
using HarmonyLib;

namespace DaftAppleGames.WeatherMod.Patches;

[HarmonyPatch(typeof(EnviroManagerBase))] public class EnviroPatches
{
    [HarmonyPatch(nameof(EnviroManagerBase.StartModules))]
    [HarmonyPrefix]
    static bool StartModules_Prefix(EnviroManagerBase __instance)
    {
        Log("Blocking Enviro.StartModules");
        return false;
    }
}
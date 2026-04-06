using Enviro;
using HarmonyLib;

namespace DaftAppleGames.WeatherMod.Patches;

[HarmonyPatch(typeof(EnviroManagerBase))]
public class EnviroPatches
{
    [HarmonyPatch(nameof(EnviroManagerBase.StartModules))]
    [HarmonyPrefix]
    static bool StartModules_Prefix(EnviroManagerBase __instance)
    {
        Log("Started EnviroManagerBase.StartModules");
        Log($"EnviroManagerBase Settings is null? {__instance.Time.Settings == null}");
        Log($"EnviroManagerBase Time settings is null? {__instance.Time.Settings == null}");
        return true;
    }
    
    [HarmonyPatch(nameof(EnviroManagerBase.StartModules))]
    [HarmonyPostfix]
    static void StartModules_Postfix(EnviroManagerBase __instance)
    {
        Log("StartModules postfix - applying module settings...");
        
        WeatherMod.Instance.ConfigureModules();
        
        Log("Finished EnviroManagerBase.StartModules");
        Log($"EnviroManagerBase Time settings is null? {__instance.Time.Settings == null}");
    }
}
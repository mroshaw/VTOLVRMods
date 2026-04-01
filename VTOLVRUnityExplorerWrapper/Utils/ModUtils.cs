global using static DaftAppleGames.UnityExplorerWrapper.Logger;

using System.IO;
using System.Reflection;

namespace DaftAppleGames.UnityExplorerWrapper.Utils;

public static class ModUtils
{
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
}
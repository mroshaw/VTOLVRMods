global using static DaftAppleGames.UnityExplorerWrapper.Logger;

using DaftAppleGames.UnityExplorerWrapper.Utils;
using ModLoader.Framework;
using ModLoader.Framework.Attributes;
using UnityExplorer;

namespace DaftAppleGames.UnityExplorerWrapper;

[ItemId("DaftAppleGames.UnityExplorerWrapper")]
public class UnityExplorerWrapper : VtolMod
{
    private static readonly string UnityExplorerAssembly = "UnityExplorer.STANDALONE.Mono.dll";
    
    private void Awake()
    {
        Log($"Awake at {ModUtils.ModFolder}");
        
        // Load the Unity Explorer DLL
        Log("Loading Unity Explorer DLL...");
        ModUtils.LoadAssembly(UnityExplorerAssembly);
        
        // Initialize Unity Explorer
        Log("Creating Unity Explorer instance...");
        ExplorerStandalone.CreateInstance();
        Log("Done!");
    }

    public override void UnLoad()
    {
    }
}
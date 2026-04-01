using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/_Project/AssetBundles", BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64);

        // Deploy bundles
        File.Copy("Assets/_Project/AssetBundles/unistormassetbundle",
            "E:/Dev/DAG/VTOLVRMods/VTOLVRWeather/ModProject/lib/unistormassetbundle", true);
        File.Copy("Assets/_Project/AssetBundles/enviroassetbundle",
            "E:/Dev/DAG/VTOLVRMods/VTOLVRWeather/ModProject/lib/enviroassetbundle", true);

        Debug.Log("Asset bundles successfully built and deployed");
    }
}
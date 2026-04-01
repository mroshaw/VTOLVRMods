using Enviro;
using UnityEngine;

namespace DaftAppleGames.WeatherMod.Utils;

public static class EnviroFlatCloudUtils
{
    public static void Configure(EnviroManager enviroManager, string assetBundleName)
    {
        enviroManager.FlatClouds.settings = new EnviroFlatClouds();
        enviroManager.FlatClouds.settings.cirrusCloudsTex = ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_cirrus.png");
        enviroManager.FlatClouds.settings.flatCloudsBaseTex = ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_flat_base.png");
        enviroManager.FlatClouds.settings.flatCloudsDetailTex = ModUtils.LoadFromAssetBundle<Texture2D>(assetBundleName, "tex_enviro_flat_detail.png");
        
        enviroManager.FlatClouds.settings.cirrusCloudsColor = new Gradient();
        enviroManager.FlatClouds.settings.flatCloudsLightColor = new Gradient();
        enviroManager.FlatClouds.settings.flatCloudsAmbientColor = new Gradient();
    }
}
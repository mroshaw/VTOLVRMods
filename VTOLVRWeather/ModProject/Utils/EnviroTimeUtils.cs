using Enviro;

namespace DaftAppleGames.WeatherMod.Utils;

public static class EnviroTimeUtils
{
    internal static void Configure(EnviroManager enviroManager)
    {
        enviroManager.Time.Settings = new EnviroTime();
    }

    internal static void PostConfigure(EnviroManager enviroManager)
    {
        enviroManager.Time.Settings.cycleLengthInMinutes = 1;
        enviroManager.Time.Settings.simulate = true;
    }
}
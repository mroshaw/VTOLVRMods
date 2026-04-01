# VTOL VR Mods
This repository contains my collection of mods for the amazing game [VTOL VR](https://store.steampowered.com/app/667970/?snr=1_5_9__205).

## Mods

- [VTOLVRUnityExplorerWrapper](https://github.com/mroshaw/VTOLVRMods/tree/main/VTOLVRUnityExplorerWrapper) - wraps the standalone DLL version of the amazing Unity Explorer tool, so it can be loaded as a mod.
- VTOLVRWeather - replaces the "OverCloud" Sky with "Enviro", to provide enhanced volumetric clouds and weather effects.

## Building

All mods were created using [JetBrains Rider Community Editio](https://www.jetbrains.com/rider/download/?section=windows)n (2026.1 at the time of writing). Building the mods requires:

- The [VTOL VR game](https://store.steampowered.com/app/667970/?snr=1_5_9__205) files (at time of writing, version 1.12.6f1)
- [VTOL VR Mod Loader](https://store.steampowered.com/app/3018410/?snr=1_5_9__205) (at time of writing, version 6.8.10)

Do a search/replace on `UnityExplorerWrapper.csproj`, in Notepad of similar, to set the references hint path. For example:

- Find: `E:\Games\Steam\steamapps\common\VTOL VR`
- Replace with: `<Install location of your game`

For mods with Unity Asset Bundles, you'll need to install:

- [Unity](https://unity.com/releases/editor/archive) (at time of writing, 2020.3.49f1)


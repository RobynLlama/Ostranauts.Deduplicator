using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Deduplicator;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
  internal static ManualLogSource Log = null!;

  private void Awake()
  {

    Log = Logger;

    // Log our awake here so we can see it in LogOutput.txt file
    Log.LogInfo($"Plugin {LCMPluginInfo.PLUGIN_NAME} version {LCMPluginInfo.PLUGIN_VERSION} is loaded!");

    try
    {
      CrewSim.OnGameFinishedLoading.AddListener(Callbacks.OnGameFinishedLoadingCallback);
      Log.LogInfo("Registering callbacks with CrewSim");
    }
    catch (Exception ex)
    {
      Log.LogError($"Error while initializing:\n{ex}\n");
    }
  }

}

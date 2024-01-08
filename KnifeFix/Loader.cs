using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Diagnostics;

namespace KnifeFix
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.0.0")]
    internal sealed class Loader : BasePlugin
    {
        public const string MODNAME = "KnifeFix";

#if DEBUG
        private static ManualLogSource Logger;
#endif

        [Conditional("DEBUG")]
        public static void DebugLog(object data)
        {
#if DEBUG
            Logger.LogMessage(data);
#endif
        }

        public override void Load()
        {
#if DEBUG
            Logger = Log;
#endif
            Log.LogMessage("Loading " + MODNAME);

            new Harmony(MODNAME).PatchAll(typeof(KnifePatch));

            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}
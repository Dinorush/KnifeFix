using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace KnifeFix
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.2.1")]
    internal sealed class Loader : BasePlugin
    {
        public const string MODNAME = "KnifeFix";

        public static ManualLogSource Logger;

        public override void Load()
        {
            Logger = Log;

            Log.LogMessage("Loading " + MODNAME);

            new Harmony(MODNAME).PatchAll(typeof(KnifePatch));

            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}
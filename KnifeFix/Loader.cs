using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Diagnostics;

namespace KnifeFix
{
    [BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.1.0")]
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
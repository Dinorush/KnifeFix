using BepInEx.Unity.IL2CPP;

namespace KnifeFix
{
    internal static class MWCWrapper
    {
        public const string GUID = "Dinorush.MeleeSwingCustomization";

        public static bool HasMWC { get; }

        static MWCWrapper()
        {
            HasMWC = IL2CPPChainloader.Instance.Plugins.ContainsKey(GUID);
        }
    }
}

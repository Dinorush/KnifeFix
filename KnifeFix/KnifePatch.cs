using HarmonyLib;
using GameData;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using Il2CppSystem.Collections.Generic;

namespace KnifeFix
{
    internal static class KnifePatch
    {
        private const float KNIFE_SPHERE_MIN = .15f;
        private const float KNIFE_SPHERE_MOD = -.1f;
        private const float KNIFE_LENGTH_MOD = .25f;

        private static List<uint> modifiedAnims = new();

        [HarmonyPatch(typeof(GameDataInit), nameof(GameDataInit.Initialize))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void EditKnifeDatablocks()
        {
            Il2CppArrayBase<MeleeArchetypeDataBlock> archs = MeleeArchetypeDataBlock.GetAllBlocks();
            foreach (MeleeArchetypeDataBlock archBlock in archs)
            {
                MeleeAnimationSetDataBlock? animBlock = MeleeAnimationSetDataBlock.GetBlock(archBlock.MeleeAnimationSet);
                if (animBlock == null)
                    continue;

                if (IsKnifeAnim(animBlock))
                {
                    Loader.Logger.LogMessage("Modified " + archBlock.PublicName + " archetype block.");
                    OverwriteKnifeArch(archBlock);
                    if (!modifiedAnims.Contains(animBlock.persistentID))
                    {
                        Loader.Logger.LogMessage("Modified " + animBlock.name + " animation block.");
                        OverwriteKnifeAnim(animBlock);
                        modifiedAnims.Add(animBlock.persistentID);
                    }
                }
            }
        }

        private static bool IsKnifeAnim(MeleeAnimationSetDataBlock animBlock)
        {
            if (modifiedAnims.Contains(animBlock.persistentID))
                return true;

            // Since custom rundowns can rename knife or have custom animations of their own,
            // this seemed like the safest way to reduce false positives (albeit, it's not pretty).

            if (!animBlock.FPAttackMissRight.Anim.Name.Equals("Knife_Hit"))
                return false;

            if (animBlock.FPAttackMissRight.AttackHitFrameTime < 5)
                return false;

            if (!animBlock.FPAttackMissLeft.Anim.Name.Equals("Knife_Hit2"))
                return false;

            if (animBlock.FPAttackMissLeft.AttackHitFrameTime < 5)
                return false;

            if (!animBlock.FPAttackChargeUpReleaseRight.Anim.Name.Equals("Knife_ChargeupRelease"))
                return false;

            if (animBlock.FPAttackChargeUpReleaseRight.AttackHitFrameTime < 5)
                return false;

            if (animBlock.FPAttackChargeUpReleaseRight.DamageStartTime > 0)
                return false;

            if (!animBlock.FPAttackChargeUpReleaseLeft.Anim.Name.Equals("Knife_ChargeupRelease2"))
                return false;

            if (animBlock.FPAttackChargeUpReleaseLeft.AttackHitFrameTime < 5)
                return false;

            if (animBlock.FPAttackChargeUpReleaseLeft.DamageStartTime > 0)
                return false;

            return true;
        }

        private static void OverwriteKnifeArch(MeleeArchetypeDataBlock archBlock)
        {
            // If only a portion of sphere radius applies (e.g. modded knife sphere), only compensate with a portion of the length 
            float fracSphere = Math.Min(1, Math.Max(0, (archBlock.AttackSphereRadius + KNIFE_SPHERE_MOD) / KNIFE_SPHERE_MIN));
            float newLength = archBlock.CameraDamageRayLength + fracSphere * KNIFE_LENGTH_MOD;
            archBlock.CameraDamageRayLength = newLength;
            archBlock.AttackSphereRadius += fracSphere * KNIFE_SPHERE_MOD;
        }

        private static void OverwriteKnifeAnim(MeleeAnimationSetDataBlock animBlock)
        {
            // To minimize the amount of hard coding, just overwriting the fields that matter.
            // Frame times don't seem to do anything from testing, but editing those too JFS.
            MeleeAnimationSetDataBlock.MeleeAttackData data = animBlock.FPAttackMissRight;
            data.AttackHitTime = 0.1167f;
            data.AttackHitFrameTime = 3.5f;
            data.AttackCamFwdHitTime = 0.1167f;
            data.AttackCamFwdHitFrameTime = 3.5f;
            data.ComboEarlyTime = 0.4667f;
            data.ComboEarlyFrameTime = 14.0f;

            data = animBlock.FPAttackMissLeft;
            data.AttackHitTime = 0.1167f;
            data.AttackHitFrameTime = 3.5f;
            data.AttackCamFwdHitTime = 0.1167f;
            data.AttackCamFwdHitFrameTime = 3.5f;
            data.ComboEarlyTime = 0.4667f;
            data.ComboEarlyFrameTime = 14.0f;

            data = animBlock.FPAttackHitRight;
            data.AttackHitTime = 0.1167f;
            data.AttackHitFrameTime = 3.5f;
            data.ComboEarlyTime = 0.3333f;
            data.ComboEarlyFrameTime = 10.0f;

            data = animBlock.FPAttackHitLeft;
            data.AttackHitTime = 0.1167f;
            data.AttackHitFrameTime = 3.5f;
            data.ComboEarlyTime = 0.3333f;
            data.ComboEarlyFrameTime = 10.0f;

            data = animBlock.FPAttackChargeUpReleaseRight;
            data.AttackHitTime = 0.1f;
            data.AttackHitFrameTime = 3.0f;
            data.DamageStartTime = 0.08f;
            data.DamageStartFrameTime = 2.4f;
            data.AttackCamFwdHitTime = 0.1f;
            data.AttackCamFwdHitFrameTime = 3.0f;

            data = animBlock.FPAttackChargeUpReleaseLeft;
            data.AttackHitTime = 0.1f;
            data.AttackHitFrameTime = 3.0f;
            data.DamageStartTime = 0.0667f;
            data.DamageStartFrameTime = 2.0f;
            data.AttackCamFwdHitTime = 0.1f;
            data.AttackCamFwdHitFrameTime = 3.0f;

            data = animBlock.FPAttackChargeUpHitRight;
            data.ComboEarlyTime = 0.3333f;
            data.ComboEarlyFrameTime = 10.0f;

            data = animBlock.FPAttackChargeUpHitLeft;
            data.ComboEarlyTime = 0.3333f;
            data.ComboEarlyFrameTime = 10.0f;
        }
    }
}

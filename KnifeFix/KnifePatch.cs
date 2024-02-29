using HarmonyLib;
using GameData;
using System;
using Gear;

namespace KnifeFix
{
    internal static class KnifePatch
    {
        private const float KNIFE_SPHERE_MIN = .15f;
        private const float KNIFE_SPHERE_MOD = -.1f;
        private const float KNIFE_LENGTH_MOD = .25f;

        [HarmonyPatch(typeof(MeleeWeaponFirstPerson), nameof(MeleeWeaponFirstPerson.SetupMeleeAnimations))]
        [HarmonyWrapSafe]
        [HarmonyPostfix]
        private static void EditKnifeDatablock(MeleeWeaponFirstPerson __instance, MeleeAnimationSetDataBlock data)
        {
            if (!IsKnife(__instance)) return;

            Loader.Logger.LogMessage("Modifying " + __instance.PublicName + " archetype block.");
            // Archetype is referenced directly, so forced to edit DBs.
            OverwriteKnifeArchBlock(__instance.MeleeArchetypeData);
            Loader.Logger.LogMessage("Modifying " + data.name + " animations.");
            OverwriteKnifeAnim(__instance);
        }

        private static bool IsKnife(MeleeWeaponFirstPerson melee)
        {
            // Since custom rundowns can rename knife or have custom animations of their own,
            // this seemed like the safest way to reduce false positives (albeit, it's not pretty).

            if (!melee.m_states[4].m_data.m_name.Equals("Knife_Hit")) // Right light
                return false;

            if (melee.m_states[4].m_data.m_attackHitTime < .16f)
                return false;

            if (!melee.m_states[3].m_data.m_name.Equals("Knife_Hit2")) // Left light
                return false;

            if (melee.m_states[3].m_data.m_attackHitTime < .16f)
                return false;

            if (!melee.m_states[10].m_data.m_name.Equals("Knife_ChargeupRelease")) // Right charge
                return false;

            if (melee.m_states[10].m_data.m_attackHitTime < .16f)
                return false;

            if (melee.m_states[10].m_data.m_damageStartTime > 0)
                return false;

            if (!melee.m_states[8].m_data.m_name.Equals("Knife_ChargeupRelease2")) // Left charge
                return false;

            if (melee.m_states[8].m_data.m_attackHitTime < .16f)
                return false;

            if (melee.m_states[8].m_data.m_damageStartTime > 0)
                return false;

            return true;
        }

        private static void OverwriteKnifeArchBlock(MeleeArchetypeDataBlock archBlock)
        {
            // If only a portion of sphere radius applies (e.g. modded knife sphere), only compensate with a portion of the length 
            float fracSphere = Math.Min(1, Math.Max(0, (archBlock.AttackSphereRadius + KNIFE_SPHERE_MOD) / KNIFE_SPHERE_MIN));
            archBlock.CameraDamageRayLength += fracSphere * KNIFE_LENGTH_MOD;
            archBlock.AttackSphereRadius += fracSphere * KNIFE_SPHERE_MOD;
        }

        private static void OverwriteKnifeAnim(MeleeWeaponFirstPerson melee)
        {
            MeleeAttackData data = melee.m_states[4].m_data; // Right light
            data.m_attackHitTime = 0.1167f;
            data.m_attackCamFwdHitTime = 0.1167f;
            data.m_comboEarlyTime = 0.4667f;

            data = melee.m_states[3].m_data; // Left light
            data.m_attackHitTime = 0.1167f;
            data.m_attackCamFwdHitTime = 0.1167f;
            data.m_comboEarlyTime = 0.4667f;

            data = melee.m_states[6].m_data; // Right light hit
            data.m_attackHitTime = 0.1167f;
            data.m_comboEarlyTime = 0.3333f;

            data = melee.m_states[5].m_data; // Left light hit
            data.m_attackHitTime = 0.1167f;
            data.m_comboEarlyTime = 0.3333f;

            data = melee.m_states[10].m_data; // Right charge
            data.m_attackHitTime = 0.1f;
            data.m_damageStartTime = 0.08f;
            data.m_attackCamFwdHitTime = 0.1f;

            data = melee.m_states[8].m_data; // Left charge
            data.m_attackHitTime = 0.1f;
            data.m_damageStartTime = 0.0667f;
            data.m_attackCamFwdHitTime = 0.1f;

            data = melee.m_states[12].m_data; // Right charge hit
            data.m_comboEarlyTime = 0.3333f;

            data = melee.m_states[11].m_data; // Left charge hit
            data.m_comboEarlyTime = 0.3333f;
        }
    }
}

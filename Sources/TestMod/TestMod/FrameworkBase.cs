// CultivationFramework.cs
// RimWorld cultivation framework skeleton (updated)
// Author: (your name)
// License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using TestMod;
using UnityEngine;

namespace CultivationFramework
{
    #region Enumerations

    public enum CultivationRealm
    {
        Mortal,
        QiRefinement,
        Foundation,
        CoreFormation,
        NascentSoul,
        SoulDivine
    }

    public enum CultivationPathType
    {
        Spiritual,
        Body,
        Alchemy,
        Demonic
    }

    #endregion





   

    #region Harmony bootstrap & example patches

    [StaticConstructorOnStartup]
    public static class CultivationFrameworkStartup
    {
        static CultivationFrameworkStartup()
        {
            var harmony = new Harmony("com.mrpenguinchik.cultivationframework");
            harmony.PatchAll();
        }
    }

    // Simple patch example: give Qi while using vanilla meditation job (Royalty DLC).
    [HarmonyPatch(typeof(JobDriver_Meditate), "MeditationTick")]
    public static class Patch_MeditationTick_Cultivation
    {
        static void Postfix(JobDriver_Meditate __instance)
        {
            Pawn pawn = __instance.pawn;
            var comp = pawn.TryGetComp<CompCultivator>();
            if (comp != null) comp.GainQi(0.1f);
        }
    }

    #endregion
}

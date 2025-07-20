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



    public enum CultivationPathType
    {
        Spiritual,
        Body,
        Alchemy,
        Demonic
    }
    public enum QiType
    {
        BaseQi,
        CoreQi,
        NascentQi,
        ImmortalQi,
        BloodQi,
        SoulEnergy//TODO psyfocus compatbility
    }

    public enum QiElement
    {
        None,
        Fire,
        Water,
        Earth,
        Metal,
        Wood
    }


    public static class QiElementUtility
    {
        // Element that nourishes the key one in the generating cycle
        private static readonly Dictionary<QiElement, QiElement> feeds = new Dictionary<QiElement, QiElement>
        {
            { QiElement.Wood, QiElement.Fire },
            { QiElement.Fire, QiElement.Earth },
            { QiElement.Earth, QiElement.Metal },
            { QiElement.Metal, QiElement.Water },
            { QiElement.Water, QiElement.Wood }
        };

        // Element that controls the key one in the overcoming cycle
        private static readonly Dictionary<QiElement, QiElement> suppresses = new Dictionary<QiElement, QiElement>
        {
            { QiElement.Wood, QiElement.Earth },
            { QiElement.Earth, QiElement.Water },
            { QiElement.Water, QiElement.Fire },
            { QiElement.Fire, QiElement.Metal },
            { QiElement.Metal, QiElement.Wood }
        };

        public static bool Feeds(this QiElement source, QiElement target)
        {
            return feeds.TryGetValue(source, out var nourished) && nourished == target;
        }

        public static bool Suppresses(this QiElement source, QiElement target)
        {
            return suppresses.TryGetValue(source, out var suppressed) && suppressed == target;
        }
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

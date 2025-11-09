using Duckov.Endowment;
using HarmonyLib;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using SodaCraft.Localizations;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PorterEnhanced
{
    internal class EndowmentPorterPatch
    {
        private static EndowmentEntry.ModifierDescription[] extraModifiers = [
            new() { statKey ="WalkSpeed", value = 0.5f, type = ModifierType.Add },
            new() { statKey ="RunSpeed", value = 0.2f, type = ModifierType.Add },
            new() { statKey ="Stamina", value = 8, type = ModifierType.Add },
            new() { statKey ="StaminaRecoverRate", value = 0.05f, type = ModifierType.PercentageAdd },
        ];

        [HarmonyPatch(declaringType: typeof(EndowmentEntry))]
        [HarmonyPatch(methodName: nameof(EndowmentEntry.ApplyModifiers))]
        private class _1
        {
            static void Postfix(EndowmentEntry __instance)
            {
                if (__instance.Index == EndowmentIndex.Porter && CharacterMainControl.Main?.CharacterItem is Item characterItem)
                {
                    foreach (EndowmentEntry.ModifierDescription modifierDescription in extraModifiers)
                    {
                        characterItem.AddModifier(modifierDescription.statKey, new Modifier(modifierDescription.type, modifierDescription.value, __instance));
                        Debug.Log($"[{nameof(PorterEnhanced)}] Added modifier(type: {Enum.GetName(typeof(ModifierType), modifierDescription.type)}"
                            + $", value: {modifierDescription.value})"
                            + $" for stat \"{modifierDescription.statKey}\" of {characterItem}");
                    }
                }
            }
        }

        [HarmonyPatch(declaringType: typeof(EndowmentEntry))]
        [HarmonyPatch(methodName: nameof(EndowmentEntry.DescriptionAndEffects))]
        [HarmonyPatch(methodType: MethodType.Getter)]
        private class _2
        {
            static void Postfix(EndowmentEntry __instance, ref string __result)
            {
                if (__instance.Index == EndowmentIndex.Porter)
                {
                    __result += "- " + string.Join("; ", extraModifiers.Select(modifier => modifier.DescriptionText));
                    __result += "\n- " + "PorterEnhanced_EffectDescription.FastRun".LocalizeToPlainTextWithVariables(
                        ("[active_threshold]", EndowmentPorterFastRun.FastRunActiveThreshold.ToString("0.##%")),
                        ("[stat_name]", "Stat_RunSpeed".LocalizeToPlainText()),
                        ("[stat_value]", EndowmentPorterFastRun.FastRunModification.ToString("+0.##;-0.##;0")));
                }
            }
        }
    }
}

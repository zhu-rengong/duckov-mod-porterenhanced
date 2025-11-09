using Duckov.Buffs;
using Duckov.Endowment;
using Duckov.Quests;
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
                    __result += "\n- " + "PorterEnhanced_EffectDescription.FeelHappy".LocalizeToPlainTextWithVariables(
                        ("[required_water]", FeelHappyRequiredWater.ToString("0")),
                        ("[buff_duration]", FeelHappyBuffDuration.ToString("0")));
                }
            }
        }

        public static float FeelHappyRequiredWater = 5.0f;
        public static float FeelHappyBuffDuration = 2.0f;

        [HarmonyPatch(declaringType: typeof(CharacterMainControl))]
        [HarmonyPatch(methodName: nameof(CharacterMainControl.AddWater))]
        private class _3
        {
            static void Postfix(CharacterMainControl __instance, float waterValue)
            {
                if (waterValue <= 0) { return; }

                if (__instance == CharacterMainControl.Main
                    && EndowmentManager.Current is { Index: EndowmentIndex.Porter } endowmentEntry
                    && QuestManager.IsQuestFinished(UserDeclaredGlobal.QUEST_SENIOR_COURIER_ID))
                {
                    bool buffDurationMustBeSet = false;

                    if (!__instance.HasBuff(UserDeclaredGlobal.BUFF_HAPPY_ID))
                    {
                        __instance.AddBuff(UserDeclaredGlobal.BUFF_HAPPY_PREFAB, __instance);
                        buffDurationMustBeSet = true;
                    }

                    if (__instance.GetBuffManager().Buffs.FirstOrDefault(buff => buff.ID == UserDeclaredGlobal.BUFF_HAPPY_ID) is Buff happyBuff)
                    {
                        float timeRecalculated = Mathf.Min(Mathf.Floor(waterValue / FeelHappyRequiredWater) * FeelHappyBuffDuration, happyBuff.TotalLifeTime);
                        if (!buffDurationMustBeSet && timeRecalculated < happyBuff.RemainingTime) { return; }
                        happyBuff.timeWhenStarted = Time.time + timeRecalculated - happyBuff.TotalLifeTime;
                    }
                }
            }
        }
    }
}

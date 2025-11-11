using Duckov;
using Duckov.Buffs;
using Duckov.Endowment;
using Duckov.ItemUsage;
using Duckov.Quests;
using HarmonyLib;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PorterEnhanced
{
    public class EndowmentPorterPatch : MonoBehaviour
    {
        public static List<EndowmentEntry.ModifierDescription> BaseModifierDescriptions = [
            new() { statKey ="WalkSpeed", value = 0.2f, type = ModifierType.Add },
            new() { statKey ="RunSpeed", value = 0.2f, type = ModifierType.Add },
            new() { statKey ="Stamina", value = 5, type = ModifierType.Add },
            new() { statKey ="StaminaRecoverRate", value = 0.1f, type = ModifierType.PercentageAdd },
        ];

        public static float SeniorCourierFastRunActiveThreshold = 0.75f;
        public static float SeniorCourierFastRunModification = 0.3f;
        public static bool IsSeniorCourierFastRunModifierApplied;
        public static Modifier SeniorCourierFastRunModifier = new(ModifierType.Add, SeniorCourierFastRunModification, UserDeclaredGlobal.PORTER_ENHANCED_MODIFIER_SOURCE);

        public static List<EndowmentEntry.ModifierDescription> ExpertCourierIIModifierDescriptions = [
            new() { statKey = "WalkSpeed", value = 0.3f, type = ModifierType.Add },
            new() { statKey = "PetCapcity", value = 1, type = ModifierType.Add },
        ];

        public static bool AreExpertCourierIIModifiersApplied;

        private void Awake()
        {
            Quest.onQuestCompleted += OnQuestCompleted;
            Debug.Log($"[{nameof(PorterEnhanced)}] Added an event handler to listen for Quest.onQuestCompleted.");
        }

        private void OnDestroy()
        {
            Quest.onQuestCompleted -= OnQuestCompleted;
            Debug.Log($"[{nameof(PorterEnhanced)}] Removed the event handler for Quest.onQuestCompleted.");
        }

        private static void OnQuestCompleted(Quest quest)
        {
            if (quest.ID == UserDeclaredGlobal.EXPERT_COURIER_II_QUEST_ID
                && TryGetMainCharacterMainControl(out var characterMainControl)
                && TryGetCharacterItem(characterMainControl, out var characterItem))
            {
                ApplyExpertCourierIIModifiers(characterItem);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetMainCharacterMainControl([NotNullWhen(true)] out CharacterMainControl? characterMainControl)
        {
            characterMainControl = null;

            if (CharacterMainControl.Main != null)
            {
                characterMainControl = CharacterMainControl.Main;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetCharacterItem(CharacterMainControl characterMainControl, [NotNullWhen(true)] out Item? characterItem)
        {
            characterItem = null;

            if (characterMainControl.CharacterItem != null)
            {
                characterItem = characterMainControl.CharacterItem;
                return true;
            }

            return false;
        }

        private void Update()
        {
            if (LevelManager.LevelInited
                && TryGetMainCharacterMainControl(out var characterMainControl)
                && TryGetCharacterItem(characterMainControl, out var characterItem))
            {
                if (EndowmentManager.Current is { Index: EndowmentIndex.Porter }
                    && characterMainControl.CurrentStamina / characterMainControl.MaxStamina > SeniorCourierFastRunActiveThreshold
                    && QuestManager.IsQuestFinished(UserDeclaredGlobal.SENIOR_COURIER_QUEST_ID))
                {
                    if (!IsSeniorCourierFastRunModifierApplied)
                    {
                        SeniorCourierFastRunModifier.RemoveFromTarget();
                        characterItem.Stats[UserDeclaredGlobal.RUN_SPEED_STRING_HASH].AddModifier(SeniorCourierFastRunModifier);
                        Debug.Log($"[{nameof(PorterEnhanced)}] Ability to fast run.");
                        IsSeniorCourierFastRunModifierApplied = true;
                    }
                }
                else if (IsSeniorCourierFastRunModifierApplied)
                {
                    characterItem.Stats[UserDeclaredGlobal.RUN_SPEED_STRING_HASH].RemoveModifier(SeniorCourierFastRunModifier);
                    Debug.Log($"[{nameof(PorterEnhanced)}] Inability to fast run.");
                    IsSeniorCourierFastRunModifierApplied = false;
                }
            }
        }

        private static void ApplyExpertCourierIIModifiers(Item characterItem)
        {
            if (!AreExpertCourierIIModifiersApplied)
            {
                foreach (EndowmentEntry.ModifierDescription modifierDescription in ExpertCourierIIModifierDescriptions)
                {
                    characterItem.AddModifier(modifierDescription.statKey, new Modifier(modifierDescription.type, modifierDescription.value, UserDeclaredGlobal.PORTER_ENHANCED_MODIFIER_SOURCE));
                }
                AreExpertCourierIIModifiersApplied = true;
                Debug.Log($"[{nameof(PorterEnhanced)}] Applied Expert Courier II modifiers.");
            }
        }

        [HarmonyPatch(declaringType: typeof(EndowmentEntry))]
        [HarmonyPatch(methodName: nameof(EndowmentEntry.Activate))]
        private class Activate
        {
            static void Postfix(EndowmentEntry __instance)
            {
                if (__instance.Index == EndowmentIndex.Porter
                    && TryGetMainCharacterMainControl(out var characterMainControl)
                    && TryGetCharacterItem(characterMainControl, out var characterItem))
                {
                    foreach (EndowmentEntry.ModifierDescription modifierDescription in BaseModifierDescriptions)
                    {
                        characterItem.AddModifier(modifierDescription.statKey, new Modifier(modifierDescription.type, modifierDescription.value, UserDeclaredGlobal.PORTER_ENHANCED_MODIFIER_SOURCE));
                    }
                    Debug.Log($"[{nameof(PorterEnhanced)}] Applied Base Modifiers.");

                    if (QuestManager.IsQuestFinished(UserDeclaredGlobal.EXPERT_COURIER_II_QUEST_ID))
                    {
                        ApplyExpertCourierIIModifiers(characterItem);
                    }
                }

            }
        }

        [HarmonyPatch(declaringType: typeof(EndowmentEntry))]
        [HarmonyPatch(methodName: nameof(EndowmentEntry.Deactivate))]
        private class Deactivate
        {
            static void Postfix(EndowmentEntry __instance)
            {
                if (__instance.Index == EndowmentIndex.Porter
                    && TryGetMainCharacterMainControl(out var characterMainControl)
                    && TryGetCharacterItem(characterMainControl, out var characterItem))
                {
                    int totalRemovals = characterItem.RemoveAllModifiersFrom(UserDeclaredGlobal.PORTER_ENHANCED_MODIFIER_SOURCE);
                    IsSeniorCourierFastRunModifierApplied = false;
                    AreExpertCourierIIModifiersApplied = false;

                    Debug.Log($"[{nameof(PorterEnhanced)}] Removed a total of {totalRemovals} modifiers.");
                }
            }
        }

        [HarmonyPatch(declaringType: typeof(EndowmentEntry))]
        [HarmonyPatch(methodName: nameof(EndowmentEntry.DescriptionAndEffects))]
        [HarmonyPatch(methodType: MethodType.Getter)]
        private class AppendEndowmentDescription
        {
            private static string ConstructConditionalDescription(string conditionals, bool isActive, string description)
            {
                string result = "";

                result +=
                    (isActive ? "" : "<color=#696969>")
                    + (isActive ? "<color=#696969>" : "<color=#808080>") + conditionals + "</color>"
                    + (isActive
                        ? $" (<color=#00FF00>{"PorterEnhanced_EffectActive".LocalizeToPlainText()}</color>) "
                        : $" (<color=#CC0000>{"PorterEnhanced_EffectInactive".LocalizeToPlainText()}</color>) ")
                    + description
                    + (isActive ? "" : "</color>");

                return result;
            }

            static void Postfix(EndowmentEntry __instance, ref string __result)
            {
                if (__instance.Index == EndowmentIndex.Porter)
                {
                    StringBuilder stringBuilder = new(__result);

                    stringBuilder.AppendLine("- " + string.Join("; ", BaseModifierDescriptions.Select(modifier => modifier.DescriptionText)));

                    stringBuilder.AppendLine("- " + ConstructConditionalDescription(
                        conditionals: "PorterEnhanced_EndowmentEffectSeniorCourierCondtionals".LocalizeToPlainTextWithVariables(
                            ("[quest_name]", "Quest_884".LocalizeToPlainText())),

                        isActive: QuestManager.IsQuestFinished(UserDeclaredGlobal.SENIOR_COURIER_QUEST_ID),

                        description: "PorterEnhanced_EndowmentEffectSeniorCourierDescription".LocalizeToPlainTextWithVariables(
                            ("[active_threshold]", SeniorCourierFastRunActiveThreshold.ToString("0.##%")),
                            ("[stat_name]", "Stat_RunSpeed".LocalizeToPlainText()),
                            ("[stat_value]", SeniorCourierFastRunModification.ToString("+0.##;-0.##;0")))
                    ));

                    stringBuilder.AppendLine("- " + ConstructConditionalDescription(
                        conditionals: "PorterEnhanced_EndowmentEffectExpertCourierIICondtionals".LocalizeToPlainTextWithVariables(
                            ("[quest_name]", "Quest_887".LocalizeToPlainText())),

                        isActive: QuestManager.IsQuestFinished(UserDeclaredGlobal.EXPERT_COURIER_II_QUEST_ID),

                        description: string.Join("; ", ExpertCourierIIModifierDescriptions.Select(modifier => modifier.DescriptionText))
                    ));

                    stringBuilder.AppendLine("- " + ConstructConditionalDescription(
                        conditionals: "PorterEnhanced_EndowmentEffectEatDrinkCondtionals\r\n".LocalizeToPlainTextWithVariables(
                            ("[level]", UserDeclaredGlobal.EAT_DRINK_EFFECT_REQUIRED_LEVEL)),

                        isActive: EXPManager.Level >= UserDeclaredGlobal.EAT_DRINK_EFFECT_REQUIRED_LEVEL,

                        description: "PorterEnhanced_EndowmentEffectEatDrinkDescription\r\n".LocalizeToPlainTextWithVariables(
                            ("[required_amount]", FeelHappyRequiredEnergyOrWater.ToString("0.#")),
                            ("[buff_duration]", FeelHappyBuffDuration.ToString("0.#")))
                    ));

                    __result = stringBuilder.ToString();
                }
            }
        }

        public static float FeelHappyRequiredEnergyOrWater = 5.0f;
        public static float FeelHappyBuffDuration = 9.0f;

        private static float cumulatedEnergyAndWater;
        private static bool startCumulatingEnergyAndWater;

        [HarmonyPatch(declaringType: typeof(FoodDrink))]
        [HarmonyPatch(methodName: nameof(FoodDrink.Eat))]
        private class EatDrink
        {
            static void Prefix(CharacterMainControl character)
            {
                startCumulatingEnergyAndWater = EndowmentManager.Current is { Index: EndowmentIndex.Porter }
                    && TryGetMainCharacterMainControl(out var characterMainControl)
                    && character == characterMainControl
                    && EXPManager.Level >= UserDeclaredGlobal.EAT_DRINK_EFFECT_REQUIRED_LEVEL;
            }

            static void Postfix(CharacterMainControl character)
            {
                if (startCumulatingEnergyAndWater)
                {
                    if (cumulatedEnergyAndWater > 0)
                    {
                        Debug.Log($"[{nameof(PorterEnhanced)}] {character.characterPreset?.DisplayName} has supplemented a total of {cumulatedEnergyAndWater} in energy and water.");

                        bool ignoreCheckToStack = false;

                        if (!character.HasBuff(UserDeclaredGlobal.HAPPY_BUFF_ID))
                        {
                            character.AddBuff(UserDeclaredGlobal.HAPPY_BUFF_PREFAB, character);
                            ignoreCheckToStack = true;
                        }

                        if (character.GetBuffManager().Buffs.FirstOrDefault(buff => buff.ID == UserDeclaredGlobal.HAPPY_BUFF_ID) is Buff happyBuff)
                        {
                            float duration = Mathf.Min(Mathf.Floor(cumulatedEnergyAndWater / FeelHappyRequiredEnergyOrWater) * FeelHappyBuffDuration, happyBuff.TotalLifeTime);
                            if (!ignoreCheckToStack && duration < happyBuff.RemainingTime) { return; }
                            happyBuff.timeWhenStarted = Time.time + duration - happyBuff.TotalLifeTime;
                        }

                        cumulatedEnergyAndWater = 0;
                    }

                    startCumulatingEnergyAndWater = false;
                }
            }
        }

        [HarmonyPatch(declaringType: typeof(CharacterMainControl))]
        [HarmonyPatch(methodName: nameof(CharacterMainControl.AddEnergy))]
        private class CumulateEnergy
        {
            static void Prefix(CharacterMainControl __instance, out float? __state)
            {
                __state = startCumulatingEnergyAndWater ? __instance.CurrentEnergy : null;
            }

            static void Postfix(CharacterMainControl __instance, float? __state)
            {
                if (!__state.HasValue) { return; }

                cumulatedEnergyAndWater += Mathf.Max(__instance.CurrentEnergy - __state.Value, 0);
            }
        }

        [HarmonyPatch(declaringType: typeof(CharacterMainControl))]
        [HarmonyPatch(methodName: nameof(CharacterMainControl.AddWater))]
        private class CumulateWater
        {
            static void Prefix(CharacterMainControl __instance, out float? __state)
            {
                __state = startCumulatingEnergyAndWater ? __instance.CurrentWater : null;
            }

            static void Postfix(CharacterMainControl __instance, float? __state)
            {
                if (!__state.HasValue) { return; }

                cumulatedEnergyAndWater += Mathf.Max(__instance.CurrentWater - __state.Value, 0);
            }
        }
    }
}

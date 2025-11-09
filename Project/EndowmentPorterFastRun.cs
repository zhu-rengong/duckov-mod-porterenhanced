using Duckov.Endowment;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PorterEnhanced
{
    public class EndowmentPorterFastRun : MonoBehaviour
    {
        public static float FastRunActiveThreshold = 0.75f;
        public static float FastRunModification = 0.3f;

        private bool modified;

#pragma warning disable CS8618
        private Modifier runSpeedModifier;
#pragma warning restore CS8618

        private void Start()
        {
            runSpeedModifier = new(ModifierType.Add, FastRunModification, this);
        }

        private void Update()
        {
            if (LevelManager.LevelInited
                && CharacterMainControl.Main is not null and var control
                && control.CharacterItem is not null and var characterItem)
            {
                if (EndowmentManager.Current is { Index: EndowmentIndex.Porter } endowmentEntry
                    && control.CurrentStamina / control.MaxStamina > FastRunActiveThreshold)
                {
                    if (!modified)
                    {
                        runSpeedModifier.RemoveFromTarget();
                        characterItem.AddModifier("RunSpeed", runSpeedModifier);
                        Debug.Log($"[{nameof(PorterEnhanced)}] Ability to fast run.");
                        modified = true;
                    }
                }
                else if (modified)
                {
                    characterItem.Stats["RunSpeed"].RemoveModifier(runSpeedModifier);
                    Debug.Log($"[{nameof(PorterEnhanced)}] Inability to fast run.");
                    modified = false;
                }
            }
        }

        private void Awake()
        {
            LevelManager.OnAfterLevelInitialized += OnAfterLevelInitialized;
            Debug.Log($"[{nameof(PorterEnhanced)}] Added an event handler to listen for LevelManager.OnAfterLevelInitialized.");
        }

        private void OnDestroy()
        {
            LevelManager.OnAfterLevelInitialized -= OnAfterLevelInitialized;
            Debug.Log($"[{nameof(PorterEnhanced)}] Removed the event handler for LevelManager.OnAfterLevelInitialized.");
        }

        private void OnAfterLevelInitialized()
        {
            modified = false;
            Debug.Log($"[{nameof(PorterEnhanced)}] Reset state of fast running.");
        }
    }
}

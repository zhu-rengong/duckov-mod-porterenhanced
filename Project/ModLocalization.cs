using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PorterEnhanced
{
    public class ModLocalization : MonoBehaviour
    {
#pragma warning disable CS8618
        private Dictionary<SystemLanguage, CSVFileLocalizor> AvailableModLocalizors;
#pragma warning restore CS8618

        private void OverrideTexts(SystemLanguage language)
        {
            if (AvailableModLocalizors.TryGetValue(language, out var provider))
            {
                foreach ((string key, MiniLocalizor.DataEntry entry) in provider.dic)
                {
                    LocalizationManager.SetOverrideText(key, entry.value);
                }
                Debug.Log($"[{nameof(PorterEnhanced)}] Successfully overrode texts for language \"{Enum.GetName(typeof(SystemLanguage), language)}\"");
            }
            else
            {
                Debug.LogWarning($"[{nameof(PorterEnhanced)}] No localization provider found for language \"{Enum.GetName(typeof(SystemLanguage), language)}\"");
            }

        }

        private void RemoveOverriddenTexts()
        {
            foreach (CSVFileLocalizor provider in AvailableModLocalizors.Values)
            {
                Debug.Log($"[{nameof(PorterEnhanced)}] Removing overrides for language \"{Enum.GetName(typeof(SystemLanguage), provider.Language)}\"");
                foreach ((string key, MiniLocalizor.DataEntry entry) in provider.dic)
                {
                    LocalizationManager.RemoveOverrideText(key);
                }
            }
        }

        private void Awake()
        {
            string textFilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TextFiles");
            AvailableModLocalizors = Directory.GetFiles(textFilesPath)
                .Where(file => Path.GetExtension(file).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                .Select(file => new CSVFileLocalizor(file))
                .ToDictionary(provider => provider.Language);

            OverrideTexts(language: LocalizationManager.CurrentLanguage);
            LocalizationManager.OnSetLanguage += OnSetLanguage;
        }

        private void OnDestroy()
        {
            RemoveOverriddenTexts();
            LocalizationManager.OnSetLanguage -= OnSetLanguage;
        }

        private void OnSetLanguage(SystemLanguage language)
        {
            OverrideTexts(language);
        }
    }
}

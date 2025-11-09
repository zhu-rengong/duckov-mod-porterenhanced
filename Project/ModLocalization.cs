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

        private void Tamper(SystemLanguage? language = null, bool restore = false)
        {
            LocalizationDatabase.Instance.Entries.ForEach(languageSettings =>
            {
                if ((language is null || languageSettings.Language == language)
                    && languageSettings.Provider is CSVFileLocalizor originalProvider
                    && AvailableModLocalizors.TryGetValue(languageSettings.Language, out var incomingProvider))
                {
                    foreach ((string key, MiniLocalizor.DataEntry entry) in incomingProvider.dic)
                    {
                        if (restore)
                        {
                            originalProvider.dic.Remove(key);
                        }
                        else
                        {
                            originalProvider.dic.TryAdd(key, entry);
                        }
                    }
                }
            });

            Debug.Log($"[{nameof(PorterEnhanced)}]"
                + (restore ? " Restored localizor for" : " Injected localized texts to")
                + (language is null ? " all languages" : $" language \"{Enum.GetName(typeof(SystemLanguage), language)}\"")
                + " by tampering with the original.");
        }

        private void Awake()
        {
            string modFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TextFiles");
            AvailableModLocalizors = Directory.GetFiles(modFolder)
                .Where(file => Path.GetExtension(file).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                .Select(file => new CSVFileLocalizor(file))
                .ToDictionary(provider => provider.Language);

            Tamper(language: LocalizationManager.CurrentLanguage);
            LocalizationManager.OnSetLanguage += OnSetLanguage;
        }

        private void OnDestroy()
        {
            Tamper(restore: true);
            LocalizationManager.OnSetLanguage -= OnSetLanguage;
        }

        private void OnSetLanguage(SystemLanguage language)
        {
            Tamper(language);
        }
    }
}

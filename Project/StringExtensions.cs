using SodaCraft.Localizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PorterEnhanced
{
    public static class StringExtensions
    {
        public static string LocalizeToPlainText(this string str) => str.ToPlainText();

        public static string LocalizeToPlainTextWithVariables(this string str, params (string Key, object Value)[] replacements)
        {
            string cachedValue = str.LocalizeToPlainText();
            foreach (var (key, value) in replacements)
            {
                // https://github.com/mono/mono/pull/20960
                // Using InvariantCulture as StringComparison will throw NotImplementedException, which has been addressed in the latest mscorlib version.
                cachedValue = cachedValue.Replace(key, value.ToString());
            }
            return cachedValue;
        }
    }
}

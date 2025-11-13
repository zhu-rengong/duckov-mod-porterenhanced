using HarmonyLib;
using PorterEnhanced.Buffs;
using System;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace PorterEnhanced
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? harmony;

        public static readonly string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public override void OnAfterSetup()
        {
            Debug.Log($"[{nameof(PorterEnhanced)}] Setup finished!");

            if (harmony is not null)
            {
                harmony.UnpatchAll(UserDeclaredGlobal.HARMONY_ID);
            }

            var currentAssembly = Assembly.GetExecutingAssembly();

            Debug.Log($"[{nameof(PorterEnhanced)}] Start patching in ASM {currentAssembly.FullName}...");
            harmony = new Harmony(UserDeclaredGlobal.HARMONY_ID);
            harmony.PatchAll(currentAssembly);
            foreach (var method in harmony.GetPatchedMethods())
            {
                var patchInfo = Harmony.GetPatchInfo(method);

                foreach (var prefix in patchInfo.Prefixes)
                {
                    if (prefix.PatchMethod.DeclaringType is Type type && type.Assembly == currentAssembly)
                    {
                        Debug.Log($"[{nameof(PorterEnhanced)}] A prefix method {type.FullName} {prefix.PatchMethod} has been patched onto method {method.DeclaringType?.FullName} {method}");
                    }
                }

                foreach (var postfix in patchInfo.Postfixes)
                {
                    if (postfix.PatchMethod.DeclaringType is Type type && type.Assembly == currentAssembly)
                    {
                        Debug.Log($"[{nameof(PorterEnhanced)}] A postfix method {type.FullName} {postfix.PatchMethod} has been patched onto method {method.DeclaringType?.FullName} {method}");
                    }
                }
            }
            Debug.Log($"[{nameof(PorterEnhanced)}] Patching finished!");

            this.AddComponent<ModLocalization>();
            this.AddComponent<EndowmentPorterPatch>();

            Debug.Log($"[{nameof(PorterEnhanced)}] Loading textures...");
            SpriteLoader.LoadTexture(UserDeclaredGlobal.SPRITES_BUFFS_PATH);
        }

        public override void OnBeforeDeactivate()
        {
            Debug.Log($"[{nameof(PorterEnhanced)}] Start deactivating...");

            if (!PorterPotentialUnleashedBuff.IsPrefabNull && !PorterPotentialUnleashedBuff.Prefab.IsDestroyed())
            {
                Destroy(PorterPotentialUnleashedBuff.Prefab.gameObject);
            }

            SpriteLoader.ClearCache();

            if (harmony is not null)
            {
                Debug.Log($"[{nameof(PorterEnhanced)}] Start unpatching...");
                harmony.UnpatchAll(UserDeclaredGlobal.HARMONY_ID);
                harmony = null;
                Debug.Log($"[{nameof(PorterEnhanced)}] Unpatching finished!");
            }
        }
    }
}

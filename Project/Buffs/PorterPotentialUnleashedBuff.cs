using Duckov.Buffs;
using Unity.VisualScripting;
using UnityEngine;

namespace PorterEnhanced.Buffs
{
    public class PorterPotentialUnleashedBuff : Buff
    {
        public static bool IsPrefabNull => _prefab is null;
        private static PorterPotentialUnleashedBuff? _prefab;
        public static PorterPotentialUnleashedBuff Prefab
        {
            get
            {
                if (_prefab is null || _prefab.IsDestroyed())
                {
                    GameObject gameObject = new GameObject(typeof(PorterPotentialUnleashedBuff).FullName);
                    gameObject.SetActive(false);
                    DontDestroyOnLoad(gameObject);
                    _prefab = gameObject.AddComponent<PorterPotentialUnleashedBuff>();
                    _prefab.id = UserDeclaredGlobal.PORTER_POTENTIAL_UNLEASHED_BUFF_ID;
                    _prefab.displayName = $"{nameof(PorterEnhanced)}_{nameof(PorterPotentialUnleashedBuff)}Name";
                    _prefab.description = $"{nameof(PorterEnhanced)}_{nameof(PorterPotentialUnleashedBuff)}Description";
                    _prefab.limitedLifeTime = true;
                    _prefab.totalLifeTime = UserDeclaredGlobal.PORTER_POTENTIAL_UNLEASHED_BUFF_DURATION;
                    _prefab.icon = SpriteLoader.CreateSprite(UserDeclaredGlobal.SPRITES_BUFFS_PATH, new(0, 0, 128, 128));
                }
                return _prefab;
            }
        }

        public override void OnSetup()
        {
            
        }
    }
}

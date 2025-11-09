using Duckov.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PorterEnhanced
{
    public static class UserDeclaredGlobal
    {
        public const string HARMONY_ID = "com.whosyourdaddy.porterenhacned";
        public const int QUEST_SENIOR_COURIER_ID = 884;
        public const int BUFF_HAPPY_ID = 1101;
        public static Buff? BUFF_HAPPY_PREFAB;

        static UserDeclaredGlobal()
        {
            foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.GetComponent<Buff>() is { ID: BUFF_HAPPY_ID } buff)
                {
                    BUFF_HAPPY_PREFAB = buff;
                }
            }
        }
    }
}

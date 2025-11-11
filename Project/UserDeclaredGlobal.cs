using Duckov.Buffs;
using Duckov.Quests;
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

        public static int RUN_SPEED_STRING_HASH = "RunSpeed".GetHashCode();

        public const int SENIOR_COURIER_QUEST_ID = 884;
        public const int PROMOTION_TO_EXPERT_COURIER_QUEST_ID = 885;
        public const int EXPERT_COURIER_II_QUEST_ID = 887;

        public static int EAT_DRINK_EFFECT_REQUIRED_LEVEL = -1;

        public const int HAPPY_BUFF_ID = 1101;
        public static Buff? HAPPY_BUFF_PREFAB;

        public static object PORTER_ENHANCED_MODIFIER_SOURCE = new object();

        static UserDeclaredGlobal()
        {
            foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (HAPPY_BUFF_PREFAB is null && gameObject.GetComponent<Buff>() is { ID: HAPPY_BUFF_ID } buff)
                {
                    HAPPY_BUFF_PREFAB = buff;
                }

                if (gameObject.GetComponent<Quest>() is { ID: PROMOTION_TO_EXPERT_COURIER_QUEST_ID } quest)
                {
                    EAT_DRINK_EFFECT_REQUIRED_LEVEL = quest.RequireLevel;
                }
            }
        }
    }
}

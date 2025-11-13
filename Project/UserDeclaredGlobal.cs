using Duckov.Buffs;
using Duckov.Quests;
using PorterEnhanced.Buffs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PorterEnhanced
{
    public static class UserDeclaredGlobal
    {
        public const string HARMONY_ID = "com.whosyourdaddy.porterenhacned";

        public static readonly string SPRITES_BUFFS_PATH = Path.Combine(ModBehaviour.Location, "Sprites", "Buffs.png");

        public static int RUN_SPEED_STRING_HASH = "RunSpeed".GetHashCode();
        public static int MAX_WEIGHT_STRING_HASH = "MaxWeight".GetHashCode();

        public const int SENIOR_COURIER_QUEST_ID = 884;
        public const int PROMOTION_TO_EXPERT_COURIER_QUEST_ID = 885;
        public const int EXPERT_COURIER_II_QUEST_ID = 887;

        public static int EAT_DRINK_EFFECT_REQUIRED_LEVEL = -1;

        public const int HAPPY_BUFF_ID = 1101;
        public static Buff? HAPPY_BUFF_PREFAB;

        public const int WEIGHT_OVERWEIGHT_BUFF_ID = 1024;
        public const int WEIGHT_SUPERHEAVY_BUFF_ID = 1023;

        public static readonly int MID = 367678838;
        public static readonly int PORTER_POTENTIAL_UNLEASHED_BUFF_ID = MID + 1;
        public static readonly float PORTER_POTENTIAL_UNLEASHED_BUFF_DURATION = 60.0f;

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

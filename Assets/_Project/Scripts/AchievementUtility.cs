using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AchievementSystem
{
    public class AchievementUtility
    {
        public static Achievement GetAchievementFromEnum(AchievementID achievementID,CurrentAchievements currentAchievements)
        {
            Achievement achievement = null;

            foreach (var ach in currentAchievements.achievements)
            {
                if(ach.id == achievementID.ToString())
                {
                    achievement = ach;
                    break;
                }
            }

            return achievement;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
            if (achievement == null) Debug.LogError("cannot find achievement");
            return achievement;
        }


        public static string AddSingleQuotes(string value)
        {
            return "\'" + value + "\'";
        }

        /// <summary>
        /// Most of the characters start from E,A,r,i,o,t,n,s,l
        /// </summary>
        /// <returns></returns>
        public static char GetCharacter()
        {
            char[] commonStartingChar = new char[] { 'e', 'a', 'r', 'i', 'o', 't', 'n', 's', 'l' };
            return char.ToUpper(commonStartingChar[Random.Range(0, commonStartingChar.Length)]);
        }

        public static int GetRandomWordCount()
        {
            return Random.Range(4, 6);
        }

    }
}
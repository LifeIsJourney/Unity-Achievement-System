using Random = UnityEngine.Random;
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

        

        public static List<Achievement> GetLoadedAchievements(AchievementDatabase achievementDatabase,
            SavedAchievements savedAchievements, System.Action<string> onAchievementCompleted)
        {
            List<Achievement> loadedAchievements = new List<Achievement>();

            if (savedAchievements != null)
            {
                foreach (var achievement in savedAchievements.achievements)
                {

                    int userLevel;
                    int[] changingValue;
                    char charValue;
                    int progress;
                    GetDataFromSavedValue(achievement.userValue, out userLevel,
                        out changingValue, out charValue, out progress);

                    Achievement loadedAchievement =
                        AchievementFactory.CreateAchievement(achievement.achievementID, achievementDatabase, userLevel,
                        changingValue, charValue);

                    loadedAchievement.userLevel = userLevel;
                    loadedAchievement.changingValue = changingValue;
                    loadedAchievement.charValue = charValue;
                    loadedAchievement.progress = progress;
                    loadedAchievement.onAchievementCompleted = onAchievementCompleted;

                    if (progress >= changingValue[0] - 1) loadedAchievement.isCompleted = true;

                    loadedAchievements.Add(loadedAchievement);

                }
            }

            return loadedAchievements;
        }

        private static void GetDataFromSavedValue(string savedValue, out int userLevel,
            out int[] changingValue, out char charValue, out int progress)
        {
            string[] split = savedValue.Split('@');

            int.TryParse(split[0], out userLevel);

            string[] _changingValueSplit = split[1].Split(',');
            changingValue = new int[_changingValueSplit.Length];

            for (int i = 0; i < changingValue.Length; i++)
            {
                int.TryParse(_changingValueSplit[i], out changingValue[i]);
            }

            char.TryParse(split[2], out charValue);

            int.TryParse(split[3], out progress);
        }


        static char splitChar = '@';
        public static string ParseUserValue(Achievement achievement)
        {
            return ParseUserValue(achievement.userLevel, achievement.changingValue,
                achievement.charValue, achievement.progress);
        }
        public static string ParseUserValue(int userLevel, int[] changingValue, char charValue, int progress = 0)
        {
            string userValue = string.Empty;

            userValue += userLevel;
            userValue += splitChar;
            bool removeFirstComma = true;
            foreach (var intValue in changingValue)
            {
                if (removeFirstComma)
                {
                    removeFirstComma = false;
                    userValue += intValue.ToString();
                }
                else
                    userValue += ',' + intValue.ToString();
            }
            userValue += splitChar;
            userValue += charValue;
            userValue += splitChar;
            userValue += progress;

            return userValue;
        }

        public static string ParseUserValue(int userLevel, int[] changingValue)
        {
            return ParseUserValue(userLevel, changingValue, AchievementManager.achievementDefaultCharacter, 0);
        }


    }
}
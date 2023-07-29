using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using Rand = UnityEngine.Random;

namespace AchievementSystem
{
    /// <summary>
    /// Make and save achievement
    /// </summary>
    public class AchievementsMaker : MonoBehaviour
    {
        [SerializeField] AchievementDatabase achievementDatabase;

        [SerializeField] AchievementManager achievementManager;
        
       
        string achievementString;
       
        public void LoadOrMakeAchievements()
        {
            achievementString = PlayerPrefs.GetString(AchievementManager.PrefsKey,string.Empty);
            achievementManager.currentAchievements = new CurrentAchievements();
            achievementManager.currentAchievements.achievements = new List<Achievement>();
            List<AchievementID> achievementIDs = new List<AchievementID>();
            List<string> achievementValues = new List<string>();
            // if(user level > AchievementManager.UnlockLevel)
            if (true)
            {
                if (!string.IsNullOrEmpty(achievementString))
                {
                    achievementManager.savedAchievements = JsonUtility.FromJson<SavedAchievements>(achievementString);

                    foreach (var achievement in achievementManager.savedAchievements.achievements)
                    {
                        achievementIDs.Add(achievement.achievementID);
                        achievementValues.Add(achievement.userValue);
                    }
                }
                else
                {
                    List<AchievementID> achievementList = new List<AchievementID>();

                    foreach (AchievementID val in Enum.GetValues(typeof(AchievementID)))
                    {
                        achievementList.Add(val);
                    }

                   achievementIDs = GetRandomUniqueCombinations(achievementList,
                        AchievementManager.MaxConcurrentAchievementCount);

                    //Fill achievement value
                    achievementValues = CreateNewAchievements(achievementIDs);

                    // Save achievements
                    SaveNewAchievements(achievementIDs, achievementValues);
                }

                ShowAchievements(achievementIDs, achievementValues);
            }
        }

       
        public List<AchievementID> GetRandomUniqueCombinations(List<AchievementID> allValues, int count)
        {
            System.Random random = new System.Random();
            int n = allValues.Count;

            // Use Fisher-Yates (Knuth) shuffle algorithm
            for (int i = 0; i < n; i++)
            {
                int randIndex = i + random.Next(n - i);
                AchievementID temp = allValues[randIndex];
                allValues[randIndex] = allValues[i];
                allValues[i] = temp;
            }

            return allValues.GetRange(0, Math.Min(n, count));
        }

        private List<string> CreateNewAchievements(List<AchievementID> randomCombinations)
        {
            List<string> achievementValues = new List<string>();

            foreach (var achievement in randomCombinations)
            {
                if (achievement == AchievementID.BeatLevel || achievement == AchievementID.UseHint
                    || achievement == AchievementID.EarnCoin || achievement == AchievementID.SpendCoins
                    || achievement == AchievementID.ExtraWords || achievement == AchievementID.MakeWords
                    || achievement == AchievementID.MakeVovelsWords)
                {
                  achievementValues.Add(MakeSingleUnknownValueAchievement(achievement));
                }
                else if (achievement == AchievementID.PlayWord
                    || achievement == AchievementID.MakeLetterWords)
                {
                    achievementValues.Add(MakeTwoUnknownValueAchievement(achievement));
                }
            }
            return achievementValues;
        }

        private string MakeTwoUnknownValueAchievement(AchievementID achievement)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());
            
            int userLevel = 1;
            int[] changingValue = new int[] { tmp.initialValue * userLevel, Rand.Range(4, 6) };
            char charValue = GetCharacter();
            return ParseUserValue(userLevel, changingValue,charValue);
        }

        private string MakeSingleUnknownValueAchievement(AchievementID achievement)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());
           
            //necessary value
            int userLevel = 1; int[] changingValue = new int[] { tmp.initialValue * userLevel };

            return ParseUserValue(userLevel, changingValue);
        }

        private void SaveNewAchievements(List<AchievementID> randomCombinations, List<string> achievementValues)
        {
            achievementManager.savedAchievements = new SavedAchievements()
            {
                achievements = new AchievementWithValue[randomCombinations.Count]
            };

            for (int i = 0; i < randomCombinations.Count; i++)
            {
                achievementManager.savedAchievements.achievements[i] = new AchievementWithValue()
                {
                    achievementID = randomCombinations[i],
                    userValue = achievementValues[i]
                };
            }

            achievementString = JsonUtility.ToJson(achievementManager.savedAchievements);
            PlayerPrefs.SetString(AchievementManager.PrefsKey, achievementString);
        }

        private void ShowAchievements(List<AchievementID> achievementIds, List<string> achievementValues)
        {
            for (int i = 0; i < achievementIds.Count; i++)
            {
                achievementManager.currentAchievements.achievements.Add(ShowAchievement(achievementIds[i],
                    achievementValues[i]));
            }
        }

        Achievement ShowAchievement(AchievementID achievement, string savedValue)
        {
            int userLevel; int[] changingValue;char charValue;
            GetDataFromSavedValue(savedValue, out userLevel, out changingValue ,out charValue);

            if (achievement == AchievementID.BeatLevel || achievement == AchievementID.UseHint
            || achievement == AchievementID.EarnCoin || achievement == AchievementID.SpendCoins
            || achievement == AchievementID.ExtraWords || achievement == AchievementID.MakeWords
                || achievement == AchievementID.MakeVovelsWords)
            {
              return LoadSingleUnknownValueAchievement(achievement,userLevel,changingValue);
            }
            else if (achievement == AchievementID.PlayWord
                || achievement == AchievementID.MakeLetterWords)
            {
               return LoadTwoUnknownValueAchievement(achievement, userLevel, changingValue, charValue);
            }
            return null;
        }

        Achievement LoadSingleUnknownValueAchievement(AchievementID achievement,
            int userStage, int[] changingValue)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());
            Achievement achievement1 = new Achievement(tmp.id, tmp.title, tmp.description, 
                userStage, changingValue);

            achievement1.description = string.Format(achievement1.description, achievement1.changingValue[0]);
           
            return achievement1;
        }

        Achievement LoadTwoUnknownValueAchievement(AchievementID achievement,
              int userStage, int[] changingValue,char charValue)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());

            Achievement achievement1 = new Achievement(tmp.id, tmp.title, tmp.description, userStage, 
               changingValue, charValue);

            if (achievement == AchievementID.PlayWord)
                achievement1.title = string.Format(tmp.title, achievement1.charValue);

            if (achievement == AchievementID.PlayWord)
                achievement1.description =
                 string.Format(achievement1.description,
                 achievement1.changingValue[0], AddSingleQuotes(achievement1.charValue.ToString()));
            if (achievement == AchievementID.MakeLetterWords)
                achievement1.description =
                string.Format(achievement1.description,
                achievement1.changingValue[0], AddSingleQuotes(achievement1.changingValue[1].ToString()));

            return achievement1;
        }

        char splitChar = '@';
        string ParseUserValue(Achievement achievement)
        {
            string userValue = string.Empty;

            userValue += achievement.userLevel;
            userValue += splitChar;
            bool removeFirstComma = true;
            foreach (var intValue in achievement.changingValue)
            {
                if (removeFirstComma)
                {
                    removeFirstComma = false;
                    userValue += intValue.ToString();
                }else
                    userValue += ','+intValue.ToString();
            }
            userValue += splitChar;
            userValue += achievement.charValue;

            return userValue;
        }
        string ParseUserValue(int userLevel, int[] changingValue,char charValue)
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

            return userValue;
        }

        string ParseUserValue(int userLevel, int[] changingValue)
        {
           return ParseUserValue(userLevel, changingValue, '!');
        }
        void GetDataFromSavedValue(string savedValue,out int userLevel,out int[] changingValue,out char charValue)
        {
           string[] split = savedValue.Split(splitChar);

            int.TryParse(split[0], out userLevel);

            string[] _changingValueSplit = split[1].Split(',');
            changingValue = new int[_changingValueSplit.Length];

            for (int i = 0; i < changingValue.Length; i++)
            {
                int.TryParse(_changingValueSplit[i], out changingValue[i]);
            }

            char.TryParse(split[2], out charValue);
        }

        /// <summary>
        /// Most of the characters start from E,A,r,i,o,t,n,s,l
        /// </summary>
        /// <returns></returns>
        char[] commonStartingChar = new char[] { 'e', 'a', 'r', 'i', 'o', 't', 'n', 's', 'l' };
        char GetCharacter()
        {
            return char.ToUpper(commonStartingChar[Rand.Range(0,commonStartingChar.Length)]);
        }

        public string AddSingleQuotes(string value)
        {
            return "\'" + value + "\'";
        }
    }
    [System.Serializable]
    public class CurrentAchievements
    {
        public List<Achievement> achievements;
    }
    [System.Serializable]
    public class SavedAchievements
    {
        public AchievementWithValue[] achievements;
    }
    [System.Serializable]
    public class AchievementWithValue
    {
        public AchievementID achievementID;
        //userValue string is in this format
        //userLevel-changing int values []-changing char value
        //changing int value is array [] - values will be like - 1,2,3,4
        public string userValue;
    }
}

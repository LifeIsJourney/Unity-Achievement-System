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
            achievementManager.LoadAchievements();
            //achievementManager.SaveAchievements(); // Call SaveAchievements here whenever needed

            List<Achievement> loadedAchievements = achievementManager.GetLoadedAchievements(achievementDatabase);
            List<string> achievementValues = new List<string>();
            List<AchievementID> achievementIDs = new List<AchievementID>();

            if (loadedAchievements.Count == 0)
            {
                List<AchievementID> achievementList = new List<AchievementID>();

                foreach (AchievementID val in Enum.GetValues(typeof(AchievementID)))
                {
                    achievementList.Add(val);
                }

                AchievementCombinationsGenerator generator = new AchievementCombinationsGenerator();
                achievementIDs = generator.GetRandomUniqueCombinations(achievementList, AchievementManager.MaxConcurrentAchievementCount);

                achievementValues = CreateNewAchievements(achievementIDs);

                SaveNewAchievements(achievementIDs, achievementValues);

                loadedAchievements = achievementManager.GetLoadedAchievements(achievementDatabase);
            }

            // ShowAchievements(achievementIDs, achievementValues);
            achievementManager.currentAchievements.achievements = loadedAchievements;
        }

        private List<string> CreateNewAchievements(List<AchievementID> randomCombinations)
        {
            List<string> achievementValues = new List<string>();

            foreach (var achievement in randomCombinations)
            {
                switch (achievement)
                {
                    case AchievementID.BeatLevel:
                    case AchievementID.UseHint:
                    case AchievementID.EarnCoin:
                    case AchievementID.SpendCoins:
                    case AchievementID.ExtraWords:
                    case AchievementID.MakeWords:
                    case AchievementID.MakeVovelsWords:
                            achievementValues.Add(MakeSingleUnknownValueAchievement(achievement));
                        break;

                    case AchievementID.PlayWord:
                    case AchievementID.MakeLetterWords:
                        achievementValues.Add(MakeTwoUnknownValueAchievement(achievement));
                        break;

                    default:
                        Debug.LogWarning($"Unsupported achievement ID: {achievement}");
                        break;
                }
            }
            return achievementValues;
        }

        private string MakeTwoUnknownValueAchievement(AchievementID achievement)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());

            int userLevel = 1;
            int[] changingValue = new int[] { tmp.initialValue * userLevel, AchievementUtility.GetRandomWordCount() };
            char charValue = AchievementUtility.GetCharacter();
            return ParseUserValue(userLevel, changingValue, charValue);
        }

        private string MakeSingleUnknownValueAchievement(AchievementID achievement)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());
           
            //necessary value
            int userLevel = 1; int[] changingValue = new int[] { tmp.initialValue * userLevel };

            return ParseUserValue(userLevel, changingValue);
        }

        private void SaveNewAchievements(List<AchievementID> SelectedAchievementIds, List<string> achievementValues)
        {
            achievementManager.savedAchievements = new SavedAchievements()
            {
                achievements = new AchievementWithValue[SelectedAchievementIds.Count]
            };

            for (int i = 0; i < SelectedAchievementIds.Count; i++)
            {
                achievementManager.savedAchievements.achievements[i] = new AchievementWithValue()
                {
                    achievementID = SelectedAchievementIds[i],
                    userValue = achievementValues[i]
                };
            }

            achievementManager.SaveAchievements();
        }

        public void SaveNewAchievements()
        {
            List<AchievementID> SelectedAchievementIds = new List<AchievementID>();
            List< string > achievementValues = new List<string>();

            foreach (var achievement in achievementManager.currentAchievements.achievements)
            {
                AchievementID achievementId = (AchievementID)Enum.Parse(typeof(AchievementID), achievement.id);
                SelectedAchievementIds.Add(achievementId);
                achievementValues.Add(ParseUserValue(achievement));
               
            }
            SaveNewAchievements(SelectedAchievementIds, achievementValues);
        }

        char splitChar = '@';
        string ParseUserValue(Achievement achievement)
        {
            return ParseUserValue(achievement.userLevel, achievement.changingValue, 
                achievement.charValue, achievement.progress);
        }
        string ParseUserValue(int userLevel, int[] changingValue,char charValue,int progress = 0)
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

        string ParseUserValue(int userLevel, int[] changingValue)
        {
           return ParseUserValue(userLevel, changingValue, AchievementManager.achievementDefaultCharacter,0);
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

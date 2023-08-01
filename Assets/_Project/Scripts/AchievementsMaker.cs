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
       
        public List<Achievement> LoadOrMakeAchievements(string PrefsKey,out SavedAchievements savedAchievements,
            List<AchievementID> achievementIDs,Action<string> onAchievementCompleted)
        {
            AchievementPP.LoadAchievements(PrefsKey,out savedAchievements);
            //achievementManager.SaveAchievements(); // Call SaveAchievements here whenever needed

            List<Achievement> loadedAchievements = AchievementUtility.GetLoadedAchievements(achievementDatabase,
                savedAchievements,onAchievementCompleted);
           
            if (loadedAchievements.Count == 0)
            {
                List<string> achievementValues = CreateNewAchievements(achievementIDs);

                AchievementPP.SaveNewAchievements(PrefsKey,achievementIDs, achievementValues,
                   out savedAchievements);

                loadedAchievements = AchievementUtility.GetLoadedAchievements(achievementDatabase,
                    savedAchievements, onAchievementCompleted);
            }

            // ShowAchievements(achievementIDs, achievementValues);
            return loadedAchievements;
        }

        public List<string> CreateNewAchievements(List<AchievementID> randomCombinations)
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
            return AchievementUtility.ParseUserValue(userLevel, changingValue, charValue);
        }

        private string MakeSingleUnknownValueAchievement(AchievementID achievement)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());
           
            //necessary value
            int userLevel = 1; int[] changingValue = new int[] { tmp.initialValue * userLevel };

            return AchievementUtility.ParseUserValue(userLevel, changingValue);
        }

      
    }
    [System.Serializable]
    public class CurrentAchievements
    {
        public List<Achievement> achievements;

        public CurrentAchievements()
        {
            achievements = new List<Achievement>();
        }
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

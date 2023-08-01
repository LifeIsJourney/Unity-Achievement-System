using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AchievementSystem
{
    /// <summary>
    /// Handle Achievement Player pref related task
    /// </summary>
    public class AchievementPP
    {
        public static void SaveNewAchievements(string PrefsKey, List<AchievementID> SelectedAchievementIds,
               List<string> achievementValues, out SavedAchievements savedAchievements)
        {
            savedAchievements = new SavedAchievements()
            {
                achievements = new AchievementWithValue[SelectedAchievementIds.Count]
            };

            for (int i = 0; i < SelectedAchievementIds.Count; i++)
            {
                savedAchievements.achievements[i] = new AchievementWithValue()
                {
                    achievementID = SelectedAchievementIds[i],
                    userValue = achievementValues[i]
                };
            }

            SaveAchievements(PrefsKey, savedAchievements);
        }
        public static void LoadAchievements(string PrefsKey, out SavedAchievements savedAchievements)
        {
            string achievementString = PlayerPrefs.GetString(PrefsKey, string.Empty);
            savedAchievements = JsonUtility.FromJson<SavedAchievements>(achievementString);
        }

        public static void SaveAchievements(string PrefsKey, SavedAchievements savedAchievements)
        {
            string achievementString = JsonUtility.ToJson(savedAchievements);
            PlayerPrefs.SetString(PrefsKey, achievementString);
        }
        public static void SaveNewAchievements(string key, List<Achievement> achievements, out SavedAchievements savedAchievements)
        {
            List<AchievementID> SelectedAchievementIds = new List<AchievementID>();
            List<string> achievementValues = new List<string>();

            foreach (var achievement in achievements)
            {
                AchievementID achievementId = (AchievementID)Enum.Parse(typeof(AchievementID), achievement.id);
                SelectedAchievementIds.Add(achievementId);
                achievementValues.Add(AchievementUtility.ParseUserValue(achievement));

            }
            SaveNewAchievements(key, SelectedAchievementIds, achievementValues,
               out savedAchievements);
        }

    }
}
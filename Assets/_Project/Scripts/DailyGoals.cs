using DateTime = System.DateTime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace AchievementSystem
{
    public class DailyGoals : MonoBehaviour
    {
        public List<List<AchievementID>> AchievementIDsGroup = new List<List<AchievementID>>
        {
            new List<AchievementID>()
            {
                AchievementID.BeatLevel, AchievementID.EarnCoin, AchievementID.MakeWords
            },
            new List<AchievementID>()
            {
                AchievementID.BeatLevel, AchievementID.UseHint, AchievementID.ExtraWords
            },
            new List<AchievementID>()
            {
                AchievementID.BeatLevel, AchievementID.SpendCoins, AchievementID.MakeLetterWords
            },
        };
        public CurrentAchievements dailyGoalAchievements;
        public SavedAchievements savedAchievements;

        [SerializeField] AchievementManager achievementManager;
        [SerializeField] AchievementsMaker achievementMaker;
        [SerializeField] Transform achievementItemParentTransform;

        private const string PrefsKey = "DailyGoalData";
        private const int achievementMaxCount = 3;

        DateTime lastResetTime; // The last time the daily goals were reset
        List<AchievementItemController> achievementItems;

        // Method to initialize daily goal achievements and reset if needed
        public void InitializeDailyGoals(AchievementDatabase achievementDatabase)
        {
            achievementItems = new List<AchievementItemController>();
            dailyGoalAchievements = new CurrentAchievements();
            dailyGoalAchievements.achievements = new List<Achievement>();
            // Check if daily goals need to be reset
            if (DateTime.Now.Subtract(lastResetTime).TotalHours >= 24 
                && dailyGoalAchievements != null && dailyGoalAchievements.achievements.Count != 0)
            {
                // Reset daily goals
                ResetDailyGoals();
            }
            else 
            {
                // Load or create new daily goals
                LoadOrCreateDailyGoals(achievementDatabase);
            }
           //save and load values using parse data same as other achievements
            achievementItems = achievementManager.LoadAchievementsTable(achievementItems,
                dailyGoalAchievements.achievements, achievementItemParentTransform);
        }

        public void CheckIfDailyGoalAchievementHit(string achievementId)
        {
            Achievement achievement = dailyGoalAchievements.achievements.Find(ach => ach.id == achievementId);

            if (achievement != null)
            {
                achievement.HitByUser();
                achievementManager.RefreshAchievement(achievementItems);
            }
        }

        public void AchievementCompleted(string achievement)
        {
            AchievementItemController item = null;

            foreach (var achievementItem in achievementItems)
            {
                if (achievementItem.achievement.id == achievement)
                { item = achievementItem; break; }
            }

            if (item == null) { Debug.LogError("Unable to find returning..."); return; }
            //get int value and time from saved value
            if (item.unlocked)
                return;

            achievementManager.ShowNotification(item.achievement);
            item.RefreshView();
        }
      
        private void LoadOrCreateDailyGoals(AchievementDatabase achievementDatabase)
        {
            // Load daily goal data from PlayerPrefs or create new daily goals
            AchievementUtility.LoadAchievements(PrefsKey, out savedAchievements);
            dailyGoalAchievements.achievements = AchievementUtility.GetLoadedAchievements(achievementDatabase,
               savedAchievements);

            if (dailyGoalAchievements.achievements.Count == 0)
            { 
               dailyGoalAchievements.achievements = CreateNewDailyGoals(achievementDatabase, 
                   dailyGoalAchievements.achievements);
            }
            
        }

        private void ContinueDailyGoals()
        {
            // Continue from saved progress (no need to do anything here as the achievements are already loaded)
        }
        private void SaveDailyGoals()
        {
            // Save daily goal data to PlayerPrefs
            AchievementUtility.SaveAchievements(PrefsKey,savedAchievements);
        }
        private List<Achievement> CreateNewDailyGoals(AchievementDatabase achievementDatabase,
            List<Achievement> loadedAchievements)
        {
            List<string> achievementValues = new List<string>();
            List<AchievementID> achievementIDs = new List<AchievementID>();
            // Randomly choose 3 daily goal achievements from the pool of available achievements
            List<AchievementID> allAchievementIDs = GetAchievementIDs();

            achievementValues = achievementMaker.CreateNewAchievements(allAchievementIDs);

            AchievementUtility.SaveNewAchievements(PrefsKey,allAchievementIDs, achievementValues,
                out savedAchievements);

            loadedAchievements = AchievementUtility.GetLoadedAchievements(achievementDatabase,
                savedAchievements);
            return loadedAchievements;
        }

        List<AchievementID> GetAchievementIDs()
        {
            List<AchievementID> achievementIDs = new List<AchievementID>();

            WeightedRandomSelector<int> weightedRandom = new WeightedRandomSelector<int>();
            weightedRandom.Add(0, achievementManager.levelCompleted < 50 ? 0.8f : 0.4f);
            weightedRandom.Add(1, achievementManager.levelCompleted < 50 ? 0.1f : 0.4f);
            weightedRandom.Add(2, achievementManager.levelCompleted < 50 ? 0.1f : 0.2f);

            achievementIDs = AchievementIDsGroup[weightedRandom.Select()];
            return achievementIDs;
        }

        // Method to reset daily goals
        private void ResetDailyGoals()
        {
            foreach (Achievement dailyGoal in dailyGoalAchievements.achievements)
            {
                // Reset the progress and last completion time
                dailyGoal.progress = 0;
            }

            // Update the last reset time
            lastResetTime = DateTime.Now;

            // Save the updated daily goal data to PlayerPrefs
            SaveDailyGoals();
        }

        private void OnDisable()
        {
            achievementMaker.SaveNewAchievements(PrefsKey,dailyGoalAchievements.achievements,out savedAchievements);
        }
    }
}
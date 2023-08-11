using DateTime = System.DateTime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public CurrentAchievements currentAchievements;
        public SavedAchievements savedAchievements;

        [SerializeField] AchievementManager achievementManager;
        [SerializeField] AchievementsMaker achievementMaker;
        [SerializeField] Transform achievementItemParentTransform;
        [SerializeField] AchievementItemController achievementItemPrefab;

        private const string PrefsKey = "DailyGoalData";
        private const string ResetTimePrefsKey = "DailyGoalResetTime";
        private const int achievementMaxCount = 3;

        DateTime lastResetTime; // The last time the daily goals were reset
        public List<AchievementItemController> achievementItems;

        // Method to initialize daily goal achievements and reset if needed
        public void InitializeDailyGoals(AchievementDatabase achievementDatabase)
        {
            achievementItems = new List<AchievementItemController>();
            currentAchievements = new CurrentAchievements();
            currentAchievements.achievements = new List<Achievement>();
            GetLastResetTime();

            // Load or create new daily goals
            LoadOrCreateDailyGoals(achievementDatabase);

            // Check if daily goals need to be reset
            if (DateTime.Now.Subtract(lastResetTime).TotalHours >= 24)
            {
                // Reset daily goals
                ResetDailyGoals();
            }

            //save and load values using parse data same as other achievements
            achievementItems = achievementManager.LoadAchievementsTable(achievementItemPrefab, achievementItems,
                currentAchievements.achievements, achievementItemParentTransform);

        }

        private void GetLastResetTime()
        {
            if (!PlayerPrefs.HasKey(ResetTimePrefsKey))
            {
                UpdateLastResetTime();
            }
            else
                DateTime.TryParse(PlayerPrefs.GetString(ResetTimePrefsKey), out lastResetTime);
        }

        private void UpdateLastResetTime()
        {
            lastResetTime = DateTime.Now;
            PlayerPrefs.SetString(ResetTimePrefsKey, lastResetTime.ToString());
        }

        public void CheckIfDailyGoalAchievementHit(string achievementId)
        {
            Achievement achievement = currentAchievements.achievements.Find(ach => ach.id == achievementId);

            if (achievement != null)
            {
                achievement.HitByUser();
                achievementManager.RefreshAchievement(achievementItems);
            }
        }

        private void LoadOrCreateDailyGoals(AchievementDatabase achievementDatabase)
        {
            // Randomly choose 3 daily goal achievements from the pool of available achievements
            List<AchievementID> achievementIDs = GetAchievementIDs();

            currentAchievements.achievements = achievementMaker.LoadOrMakeAchievements(PrefsKey,
                out savedAchievements, achievementIDs,AchievementCompleted);

        }

        private void ContinueDailyGoals()
        {
            // Continue from saved progress (no need to do anything here as the achievements are already loaded)
        }
        private void SaveDailyGoals()
        {
            // Save daily goal data to PlayerPrefs
            AchievementPP.SaveAchievements(PrefsKey,savedAchievements);
        }

        void AchievementCompleted(string achievementId)
        {
            achievementManager.UnlockAchievement(achievementId,achievementItems);
        }
      
        List<AchievementID> GetAchievementIDs()
        {
            WeightedRandomSelector<int> weightedRandom = new WeightedRandomSelector<int>();
            weightedRandom.Add(0, achievementManager.levelCompleted < 50 ? 0.8f : 0.4f);
            weightedRandom.Add(1, achievementManager.levelCompleted < 50 ? 0.1f : 0.4f);
            weightedRandom.Add(2, achievementManager.levelCompleted < 50 ? 0.1f : 0.2f);

            List<AchievementID> achievementIDs = AchievementIDsGroup[weightedRandom.Select()];
            return achievementIDs;
        }

        // Method to reset daily goals
        private void ResetDailyGoals()
        {
            foreach (Achievement dailyGoal in currentAchievements.achievements)
            {
                // Reset the progress and last completion time
                dailyGoal.progress = 0;
            }

            UpdateLastResetTime();

            // Save the updated daily goal data to PlayerPrefs
            SaveDailyGoals();
        }

        public void SaveAchievements()
        {
            AchievementPP.SaveNewAchievements(PrefsKey,currentAchievements.achievements,out savedAchievements);
        }

        public void Restart()
        {
            PlayerPrefs.DeleteKey(PrefsKey);

        }
    }
}
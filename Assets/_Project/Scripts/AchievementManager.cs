using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace AchievementSystem
{
    /// <summary>
    /// Achievement Lock and unlock using this class
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {

        [SerializeField] AchievementDatabase database;

        [SerializeField] AchievementNotificationController achievementNotificationController;

        [SerializeField] AchievementDropdownController achievementDropdownController;

        [SerializeField] AchievementsMaker achievementsMaker;
        [SerializeField] DailyGoals dailyGoals;
        [SerializeField] LongTermGoals longTermGoals;
        
        public AchievementID achievementToShow;

        public const int UnlockLevel = 17;
        public const int MaxConcurrentAchievementCount = 5;
        public const char achievementDefaultCharacter = '!';

        public int levelCompleted = 20;

        private void Start()
        {
            achievementDropdownController.onValueChanged += HandleAchievementDropdownValueChanged;
            dailyGoals.InitializeDailyGoals(database);
            longTermGoals.Initialize(database);
        }

        private void OnDisable()
        {
           longTermGoals.SaveAchievements();
            dailyGoals.SaveAchievements();
        }

        /// <summary>
        /// Call this from other class when achievent event hit by user
        /// </summary>
        /// <param name="achievementId"></param>
        public void AchievementHit(AchievementID achievementId)
        {
            foreach (var achievement in longTermGoals.currentAchievements.achievements)
            {
                if (achievement.id == achievementId.ToString()) achievement.HitByUser();
            }
            RefreshAchievement(longTermGoals.achievementItems);

            dailyGoals.CheckIfDailyGoalAchievementHit(achievementId.ToString());
        }


        public void ShowNotification(Achievement achievement)
        {
           achievementNotificationController.ShowNotification(achievement);
        }

        public void Restart()
        {
            dailyGoals.Restart();
            longTermGoals.Restart();
           achievementDropdownController.onValueChanged -= HandleAchievementDropdownValueChanged;
           
        }

        private void HandleAchievementDropdownValueChanged(AchievementID achievement)
        {
            achievementToShow = achievement;
        }

        [ContextMenu("LoadAchievementsTable()")]
        public List<AchievementItemController> LoadAchievementsTable(AchievementItemController prefab,
            List<AchievementItemController> achievementItems, List<Achievement> achievements,
            Transform parentTransform)
        {
            if (achievementItems.Count != 0)
            {
                foreach (AchievementItemController item in achievementItems)
                {
                    DestroyImmediate(item.gameObject);
                }
                achievementItems.Clear();
            }
            foreach (Achievement achievement in achievements)
            {
                GameObject obj = Instantiate(prefab.gameObject, parentTransform);
                AchievementItemController item = obj.GetComponent<AchievementItemController>();
                bool unlocked = PlayerPrefs.GetInt(achievement.id, 0) == 1;
                item.unlocked = unlocked;
                item.achievement = achievement;
                item.RefreshView();
                achievementItems.Add(item);
            }
            return achievementItems;
        }

        public void RefreshAchievement(List<AchievementItemController> achievementItems)
        {
            foreach (var achievement in achievementItems)
            {
                achievement.RefreshView();
            }
        }

      
        public void UnlockAchievement(string achievement,List<AchievementItemController> achievementItems)
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

            ShowNotification(item.achievement);
            item.unlocked = true;
            item.RefreshView();
        }

        public void LockAllAchievements()
        {
            foreach (Achievement achievement in database.achievements)
            {
                PlayerPrefs.DeleteKey(achievement.id);
            }
            foreach (AchievementItemController item in longTermGoals.achievementItems)
            {
                item.unlocked = false;
                item.RefreshView();
            }
        }

     
        //Debug
        public void D_ShowNotificaion()
        {
            ShowNotification(longTermGoals.currentAchievements.achievements[0]);
        }

        public void D_UnlockAchievement()
        {
            D_UnlockAchievement(achievementToShow);
        }

        public void D_UnlockAchievement(AchievementID achievement)
        {
            D_AchievementUnlocked(achievement.ToString());
        }
        public void D_AchievementUnlocked(string achievement)
        {
            UnlockAchievement(achievement.ToString(), dailyGoals.achievementItems);
            UnlockAchievement(achievement.ToString(), longTermGoals.achievementItems);
        }
    }
}
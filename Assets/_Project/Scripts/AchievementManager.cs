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
        [SerializeField] Transform achievementItemParentTransform;
        [SerializeField] AchievementItemController achievementItemPrefab;
      
        public AchievementID achievementToShow;

        public const int UnlockLevel = 17;
        public const string PrefsKey = "AchievementsData";
        public const int MaxConcurrentAchievementCount = 5;
        public const char achievementDefaultCharacter = '!';

        public int levelCompleted = 20;

        //later convert this into property
        //[HideInInspector]
        //[NonSerialized]
        public CurrentAchievements currentAchievements;
        //[HideInInspector]
        //[NonSerialized]
        public SavedAchievements savedAchievements;

        private List<AchievementItemController> achievementItems;

        private void Start()
        {
            achievementsMaker.LoadOrMakeAchievements();
            achievementDropdownController.onValueChanged += HandleAchievementDropdownValueChanged;
            achievementItems = new List<AchievementItemController>();
            dailyGoals.InitializeDailyGoals(database);
            achievementItems = LoadAchievementsTable(achievementItemPrefab,achievementItems,
                currentAchievements.achievements, achievementItemParentTransform);
            AddOnAchievementComplete();
        }

        private void OnDisable()
        {
           achievementsMaker.SaveNewAchievements(PrefsKey,currentAchievements.achievements,out savedAchievements);
        }

        /// <summary>
        /// Call this from other class when achievent event hit by user
        /// </summary>
        /// <param name="achievementId"></param>
        public void AchievementHit(string achievementId)
        {
            foreach (var achievement in currentAchievements.achievements)
            {
                if (achievement.id == achievementId) achievement.HitByUser();
            }
            RefreshAchievement(achievementItems);

            dailyGoals.CheckIfDailyGoalAchievementHit(achievementId);
        }


        public void ShowNotification(Achievement achievement)
        {
           achievementNotificationController.ShowNotification(achievement);
        }

        public void Restart()
        {
            PlayerPrefs.DeleteKey(PrefsKey);
            achievementsMaker.LoadOrMakeAchievements();
            achievementDropdownController.onValueChanged -= HandleAchievementDropdownValueChanged;
            achievementItems = LoadAchievementsTable(achievementItemPrefab,achievementItems, 
                currentAchievements.achievements, achievementItemParentTransform);
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

        void AddOnAchievementComplete()
        {
            foreach (var achievement in currentAchievements.achievements)
            {
                achievement.onAchievementCompleted -= AchievementCompleted;
                achievement.onAchievementCompleted += AchievementCompleted;
            }
        }

        void AchievementCompleted(string achievementId)
        {
            UnlockAchievement(achievementId);
            dailyGoals.AchievementCompleted(achievementId);
        }

        public void UnlockAchievement()
        {
            UnlockAchievement(achievementToShow);
        }

        public void UnlockAchievement(AchievementID achievement)
        {
            UnlockAchievement(achievement.ToString());
        }
        void UnlockAchievement(string achievement)
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
            PlayerPrefs.SetInt(item.achievement.id, 1);
            item.unlocked = true;
            item.RefreshView();
        }

        public void LockAllAchievements()
        {
            foreach (Achievement achievement in database.achievements)
            {
                PlayerPrefs.DeleteKey(achievement.id);
            }
            foreach (AchievementItemController item in achievementItems)
            {
                item.unlocked = false;
                item.RefreshView();
            }
        }

     
        //Debug
        public void D_ShowNotificaion()
        {
            ShowNotification(currentAchievements.achievements[0]);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AchievementSystem
{
    /// <summary>
    /// Achievement Lock and unlock using this class
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {

        public AchievementDatabase database;

        public AchievementNotificationController achievementNotificationController;

        public AchievementDropdownController achievementDropdownController;

        [SerializeField] AchievementsMaker achievementsMaker;

        public GameObject achievementItemPrefab;
        public Transform scrollViewContent;

        public AchievementID achievementToShow;

        public const int UnlockLevel = 17;
        public const string PrefsKey = "AchievementsData";
        public const int MaxConcurrentAchievementCount = 3;
        public int userStage = 20;

        //later convert this into property
        //[HideInInspector]
        //[NonSerialized]
        public CurrentAchievements currentAchievements;
        //[HideInInspector]
        //[NonSerialized]
        public SavedAchievements savedAchievements;

        [SerializeField]
        [HideInInspector]
        private List<AchievementItemController> achievementItems;

        private void Start()
        {
            achievementsMaker.LoadOrMakeAchievements();
            achievementDropdownController.onValueChanged += HandleAchievementDropdownValueChanged;
            LoadAchievementsTable();
        }

        public void ShowNotification()
        {
            Achievement achievement = 
                AchievementUtility.GetAchievementFromEnum(achievementToShow,currentAchievements);
            achievementNotificationController.ShowNotification(achievement);
        }

        private void HandleAchievementDropdownValueChanged(AchievementID achievement)
        {
            achievementToShow = achievement;
        }

        [ContextMenu("LoadAchievementsTable()")]
        private void LoadAchievementsTable()
        {
            foreach (AchievementItemController item in achievementItems)
            {
                DestroyImmediate(item.gameObject);
            }
            achievementItems.Clear();
            foreach (Achievement achievement in currentAchievements.achievements)
            {
                GameObject obj = Instantiate(achievementItemPrefab, scrollViewContent);
                AchievementItemController item = obj.GetComponent<AchievementItemController>();
                bool unlocked = PlayerPrefs.GetInt(achievement.id, 0) == 1;
                item.unlocked = unlocked;
                item.achievement = achievement;
                item.RefreshView();
                achievementItems.Add(item);
            }
        }

        public void UnlockAchievement()
        {
            UnlockAchievement(achievementToShow);
        }

        public void UnlockAchievement(AchievementID achievement)
        {

            AchievementItemController item = null;

            foreach (var achievementItem in achievementItems)
            {
                if (achievementItem.achievement.id == achievement.ToString())
                { item = achievementItem; break; }
            }
            
            if(item == null) { Debug.LogError("Unable to find returning...");return; }
            //get int value and time from saved value
            if (item.unlocked)
                return;

            ShowNotification();
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

    }
}
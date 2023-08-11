using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] GameObject achievementGo;
        
        public AchievementID achievementToShow;

        public const int UnlockLevel = 17;
        public const int MaxConcurrentAchievementCount = 5;
        public const char achievementDefaultCharacter = '!';

        public int levelCompleted = 20;

        public static AchievementManager Instance;
        void Awake()
        {
            Instance = this;
        }
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

        public void ShowAchievementUI()
        {
            achievementGo.SetActive(true);
            dailyGoals.ShowingAchievementUI();
            longTermGoals.ShowingAchievementUI();
        }
        public void HideAchievementUI()
        {
            achievementGo.SetActive(false);
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
                item.completed = unlocked;
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

      
        public void UnlockAchievement(string achievement,List<Achievement> achievementItems)
        {
            Achievement item = null;

            foreach (var achievementItem in achievementItems)
            {
                if (achievementItem.id == achievement)
                { item = achievementItem; break; }
            }

            if (item == null) { Debug.LogError("Unable to find returning..."); return; }
          
            ShowNotification(item);

            if (dailyGoals.CheckIfAchievementPresent(item)) { 
                Debug.Log("Reward user");
            }

        }

        public void LockAllAchievements()
        {
            foreach (Achievement achievement in database.achievements)
            {
                PlayerPrefs.DeleteKey(achievement.id);
            }
            foreach (AchievementItemController item in longTermGoals.achievementItems)
            {
                item.completed = false;
                item.RefreshView();
            }
        }

        /// <summary>
        /// Call this from other class when achievent event hit by user except word achievements
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
        public void CheckForWordAchievements(string word)
        {
            CheckAchievement(AchievementID.PlayWord, word[0]);
            CheckAchievement(AchievementID.MakeLetterWords, word.Length);
            CheckAchievement(AchievementID.MakeVovelsWords, IsContainVovels(word));
        }

        private void CheckAchievement(AchievementID achievementId, object value)
        {
            Achievement wordAchievement = GetAchievement(longTermGoals.currentAchievements.achievements, achievementId);

            if (wordAchievement != null)
            {
                switch (achievementId)
                {
                    case AchievementID.PlayWord:
                        if ((char)value == wordAchievement.charValue) wordAchievement.HitByUser();
                        break;

                    case AchievementID.MakeLetterWords:
                        if ((int)value == wordAchievement.changingValue[1]) wordAchievement.HitByUser();
                        break;

                    case AchievementID.MakeVovelsWords:
                        if ((bool)value) wordAchievement.HitByUser();
                        break;
                }
            }

            wordAchievement = GetAchievement(dailyGoals.currentAchievements.achievements, achievementId);

            if (wordAchievement != null)
            {
                switch (achievementId)
                {
                    case AchievementID.PlayWord:
                        if ((char)value == wordAchievement.charValue) wordAchievement.HitByUser();
                        break;

                    case AchievementID.MakeLetterWords:
                        if ((int)value == wordAchievement.changingValue[1]) wordAchievement.HitByUser();
                        break;

                    case AchievementID.MakeVovelsWords:
                        if ((bool)value) wordAchievement.HitByUser();
                        break;
                }
            }
        }

        // Create a list of vowels
        char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        bool IsContainVovels(string word)
        {
            // Check if the word contains any vowels
            bool containsVowels = false;
            for (int i = 0; i < word.Length; i++)
            {
                if (vowels.Contains(word[i]))
                {
                    containsVowels = true;
                    break;
                }
            }
            return containsVowels;
        }
        Achievement GetAchievement(List<Achievement> achievements,AchievementID achievementID)
        {
            return achievements.Find(ach => ach.id == achievementID.ToString());
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
            UnlockAchievement(achievement.ToString(), dailyGoals.currentAchievements.achievements);
            UnlockAchievement(achievement.ToString(), longTermGoals.currentAchievements.achievements);
        }
    }
}
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

        public AchievementDatabase database;

        public AchievementNotificationController achievementNotificationController;

        public AchievementDropdownController achievementDropdownController;

        [SerializeField] AchievementsMaker achievementsMaker;

        public GameObject achievementItemPrefab;
        public Transform scrollViewContent;

        public AchievementID achievementToShow;

        public const int UnlockLevel = 17;
        public const string PrefsKey = "AchievementsData";
        public const int MaxConcurrentAchievementCount = 5;
        public const char achievementDefaultCharacter = '!';

        public int userStage = 20;

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
            LoadAchievementsTable();
            AddOnAchievementComplete();
        }

        private void OnDisable()
        {
           achievementsMaker.SaveNewAchievements();
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
            RefreshAchievement();
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
            LoadAchievementsTable();
        }

        private void HandleAchievementDropdownValueChanged(AchievementID achievement)
        {
            achievementToShow = achievement;
        }

        [ContextMenu("LoadAchievementsTable()")]
        private void LoadAchievementsTable()
        {
            if (achievementItems.Count != 0)
            {
                foreach (AchievementItemController item in achievementItems)
                {
                    DestroyImmediate(item.gameObject);
                }
                achievementItems.Clear();
            }
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

        void RefreshAchievement()
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

        public void LoadAchievements()
        {
            string achievementString = PlayerPrefs.GetString(PrefsKey, string.Empty);
            savedAchievements = JsonUtility.FromJson<SavedAchievements>(achievementString);
        }

        public void SaveAchievements()
        {
            string achievementString = JsonUtility.ToJson(savedAchievements);
            PlayerPrefs.SetString(PrefsKey, achievementString);
        }

        public List<Achievement> GetLoadedAchievements(AchievementDatabase achievementDatabase)
        {
            List<Achievement> loadedAchievements = new List<Achievement>();

            if (savedAchievements != null)
            {
                foreach (var achievement in savedAchievements.achievements)
                {

                    int userLevel;
                    int[] changingValue;
                    char charValue;
                    int progress;
                    GetDataFromSavedValue(achievement.userValue, out userLevel, 
                        out changingValue, out charValue,out progress);

                    Achievement loadedAchievement = 
                        AchievementFactory.CreateAchievement(achievement.achievementID, achievementDatabase,userLevel,
                        changingValue,charValue);
                    
                    loadedAchievement.userLevel = userLevel;
                    loadedAchievement.changingValue = changingValue;
                    loadedAchievement.charValue = charValue;
                    loadedAchievement.progress = progress;

                    loadedAchievements.Add(loadedAchievement);

                }
            }

            return loadedAchievements;
        }

        private void GetDataFromSavedValue(string savedValue, out int userLevel,
            out int[] changingValue, out char charValue,out int progress)
        {
            string[] split = savedValue.Split('@');

            int.TryParse(split[0], out userLevel);

            string[] _changingValueSplit = split[1].Split(',');
            changingValue = new int[_changingValueSplit.Length];

            for (int i = 0; i < changingValue.Length; i++)
            {
                int.TryParse(_changingValueSplit[i], out changingValue[i]);
            }

            char.TryParse(split[2], out charValue);

            int.TryParse(split[3], out progress);
        }

        //Debug
        public void D_ShowNotificaion()
        {
            ShowNotification(currentAchievements.achievements[0]);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AchievementSystem
{
    /// <summary>
    /// Responsible for handling non daily goals or long term goals
    /// </summary>
    public class LongTermGoals : MonoBehaviour
    {
        [SerializeField] AchievementsMaker achievementsMaker;
        [SerializeField] Transform achievementItemParentTransform;
        [SerializeField] AchievementItemController achievementItemPrefab;
        [SerializeField] AchievementManager achievementManager;
        public CurrentAchievements currentAchievements;
        //[HideInInspector]
        //[NonSerialized]
        public SavedAchievements savedAchievements;

        public List<AchievementItemController> achievementItems;

        public const string PrefsKey = "AchievementsData";

        // Start is called before the first frame update
        public void Initialize(AchievementDatabase database)
        {
            currentAchievements = new CurrentAchievements();

            List<AchievementID> achievementIDs = GetAchievementIds();

            currentAchievements.achievements = achievementsMaker.LoadOrMakeAchievements(PrefsKey,
                out savedAchievements, achievementIDs,AchievementCompleted);

            achievementItems = new List<AchievementItemController>();
           
        }
        public void ShowingAchievementUI()
        {
            if (achievementItems.Count == 0)
                achievementItems = achievementManager.LoadAchievementsTable(achievementItemPrefab, achievementItems,
            currentAchievements.achievements, achievementItemParentTransform);
        }
        private static List<AchievementID> GetAchievementIds()
        {
            List<AchievementID> achievementList = new List<AchievementID>();

            foreach (AchievementID val in Enum.GetValues(typeof(AchievementID)))
            {
                achievementList.Add(val);
            }
            AchievementCombinationsGenerator generator = new AchievementCombinationsGenerator();
            List<AchievementID> achievementIDs = generator.GetRandomUniqueCombinations(achievementList,
                AchievementManager.MaxConcurrentAchievementCount);
            return achievementIDs;
        }

     
        void AchievementCompleted(string achievementId)
        {
            achievementManager.UnlockAchievement(achievementId,currentAchievements.achievements);
        }

        public void SaveAchievements()
        {
           AchievementPP.SaveNewAchievements(PrefsKey, currentAchievements.achievements, out savedAchievements);
        }
        
        public void Restart()
        {
            PlayerPrefs.DeleteKey(PrefsKey);
            achievementsMaker.LoadOrMakeAchievements(PrefsKey, out savedAchievements, GetAchievementIds(),
                AchievementCompleted);
            achievementItems = achievementManager.LoadAchievementsTable(achievementItemPrefab, achievementItems,
               currentAchievements.achievements, achievementItemParentTransform);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AchievementSystem
{
    [System.Serializable]
    public class AchievementBase
    {
        public string id;
        public string title;
        public string description;
        public int initialValue = 1;
        public string example;

    }
    /// <summary>
    /// Base class contain achievement data
    /// </summary>
    [System.Serializable]
    public class Achievement : AchievementBase
    {
        //ChangingValue value
        public int userLevel { get; set; }
        public int[] changingValue { get; set; }
        public char charValue { get; set; }
        public int progress { get; set; }
        public bool isCompleted { get; set; }

        public Action<string> onAchievementCompleted;
        public Achievement(string id, string title, string description,
            int userLevel, int[] changingValue, char charValue)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.userLevel = userLevel;
            this.changingValue = changingValue;
            this.charValue = charValue;
            this.progress = 0;
        }

        public Achievement(string id, string title, string description,
          int userLevel, int[] changingValue)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.userLevel = userLevel;
            this.changingValue = changingValue;
            this.progress = 0;
        }

        public float GetProgressPercentage()
        {
            // Calculate the progress percentage based on the current user value and the target value (changingValue[0])
            if (changingValue.Length > 0)
            {
                float currentValue = progress;
                int targetValue = changingValue[0];
                return (float)currentValue / (float)targetValue;
            }
            return 0f;
        }

        public bool IsReadyToClaim()
        {
            // Check if the achievement is ready to be claimed
            if (changingValue.Length > 0)
            {
                float currentValue = progress; 
                int targetValue = changingValue[0];
                return currentValue >= targetValue;
            }
            return false;
        }

        /// <summary>
        /// Call this when play progresses in game
        /// </summary>
        public void HitByUser()
        {
            if(!isCompleted)
            {
                int targetValue = changingValue[0];

                if (progress <= targetValue - 1) progress++;

                if (IsReadyToClaim()) { onAchievementCompleted?.Invoke(id); isCompleted = true; }
            }
        }

        public void IncrementUserLevelNreset()
        {
            userLevel++;
            if(changingValue.Length == 1)
                changingValue[0] = initialValue * userLevel;
            if(changingValue.Length == 2)
                changingValue[1] = AchievementUtility.GetRandomWordCount();
            if (charValue != AchievementManager.achievementDefaultCharacter)
                charValue = AchievementUtility.GetCharacter();

            ResetAchievement();
        }

        void ResetAchievement()
        {
            progress = 0;
            isCompleted = false;
        }
    }
}
 
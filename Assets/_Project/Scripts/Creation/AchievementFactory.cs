using AchievementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace AchievementSystem
{
    // Factory responsible for creating achievements based on AchievementID
    public class AchievementFactory
    {
        public static Achievement CreateAchievement(AchievementID achievementID,
            AchievementDatabase achievementDatabase, int userLevel, int[] changingValue, char charValue)
        {
            Achievement achievement = null;

            switch (achievementID)
            {
                case AchievementID.BeatLevel:
                case AchievementID.UseHint:
                case AchievementID.EarnCoin:
                case AchievementID.SpendCoins:
                case AchievementID.ExtraWords:
                case AchievementID.MakeWords:
                case AchievementID.MakeVovelsWords:
                    achievement = SingleUnknownValueAchievement(achievementDatabase, achievementID, 
                        userLevel, changingValue);

                    break;

                case AchievementID.PlayWord:
                case AchievementID.MakeLetterWords:
                    achievement =  TwoUnknownValueAchievement(achievementDatabase, achievementID,
                        userLevel, changingValue, charValue);
                    break;

                // Add more cases for other achievement types if needed

                default:
                    Debug.LogWarning($"Unsupported achievement ID: {achievementID}");
                    break;
            }

            return achievement;
        }

    
        private static Achievement SingleUnknownValueAchievement(AchievementDatabase achievementDatabase, 
            AchievementID achievement,int userStage, int[] changingValue)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());
            Achievement achievement1 = new Achievement(tmp.id, tmp.title, tmp.description,
                userStage, changingValue);

            achievement1.initialValue = tmp.initialValue;
            achievement1.description = string.Format(achievement1.description, achievement1.changingValue[0]);

            return achievement1;
        }

        private static Achievement TwoUnknownValueAchievement(AchievementDatabase achievementDatabase, 
            AchievementID achievement, int userStage, int[] changingValue, char charValue)
        {
            Achievement tmp = achievementDatabase.GetAchievement(achievement.ToString());

            Achievement achievement1 = new Achievement(tmp.id, tmp.title, tmp.description, userStage,
               changingValue, charValue);

            achievement1.initialValue = tmp.initialValue;

            if (achievement == AchievementID.PlayWord)
                achievement1.title = string.Format(tmp.title, achievement1.charValue);

            if (achievement == AchievementID.PlayWord)
                achievement1.description =
                 string.Format(achievement1.description,
                 achievement1.changingValue[0], AchievementUtility.AddSingleQuotes(achievement1.charValue.ToString()));
            if (achievement == AchievementID.MakeLetterWords)
                achievement1.description =
                string.Format(achievement1.description,
                achievement1.changingValue[0], AchievementUtility.AddSingleQuotes(achievement1.changingValue[1].ToString()));

            return achievement1;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AchievementSystem
{
    public class Tester : MonoBehaviour
    {
        public AchievementManager achievementManager;

        public void MilestoneReached()
        {
            achievementManager.UnlockAchievement(AchievementID.BeatLevel);
        }

        public void AchievementProgressed(int index)
        {
            achievementManager.AchievementHit(achievementManager.currentAchievements.achievements[index].id);
            Debug.Log("Name :" + achievementManager.currentAchievements.achievements[index].id +
                "Progress: " + achievementManager.currentAchievements.achievements[index].GetProgressPercentage());
        }

    }
}
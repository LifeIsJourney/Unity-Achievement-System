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

    }
}
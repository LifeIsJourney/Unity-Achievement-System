using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AchievementSystem
{
    public class Tester : MonoBehaviour
    {
        public AchievementManager achievementManager;
        public AchievementID achievementID;
        public Dropdown achievementDropdown;

        public void MilestoneReached()
        {
            if (achievementDropdown != null)
            { achievementManager.D_UnlockAchievement((AchievementID)achievementDropdown.value); }
           else achievementManager.D_UnlockAchievement(achievementID);
        }

        public void AchievementProgressed()
        {
            if(achievementDropdown != null)
            { achievementManager.AchievementHit((AchievementID)achievementDropdown.value); }
            else achievementManager.AchievementHit(achievementID);
            Debug.Log("Name :" + achievementID);
        }

    }
}
using AchievementSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordGame
{
    public class ShowAchievementButton : MonoBehaviour
    {
        public void OnButtonPressed(bool show)
        {
           if(show) AchievementManager.Instance.ShowAchievementUI();
           else AchievementManager.Instance.HideAchievementUI();
        }
    }
}

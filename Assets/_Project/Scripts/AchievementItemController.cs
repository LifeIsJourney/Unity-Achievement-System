using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AchievementSystem
{
    /// <summary>
    /// Achievement view in ui
    /// </summary>
    public class AchievementItemController : MonoBehaviour
    {

        [SerializeField] Image unlockedIcon;
        [SerializeField] Image lockedIcon;

        [SerializeField] Text titleLabel;
        [SerializeField] Text descriptionLabel;

        public bool unlocked;
        public Achievement achievement;

        public void RefreshView()
        {
            titleLabel.text = achievement.title;
            descriptionLabel.text = achievement.description;

            unlockedIcon.enabled = unlocked;
            lockedIcon.enabled = !unlocked;
        }

        private void OnValidate()
        {
            RefreshView();
        }

    }
}
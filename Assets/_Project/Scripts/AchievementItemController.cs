using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AchievementSystem
{
    /// <summary>
    /// Achievement view in ui
    /// </summary>
    public class AchievementItemController : MonoBehaviour
    {

        [SerializeField] Image unlockedIcon;
        [SerializeField] Image lockedIcon;

        [SerializeField] TextMeshProUGUI titleLabel;
        [SerializeField] TextMeshProUGUI descriptionLabel;
        [SerializeField] ProgressBar progressBar;
        [SerializeField] GameObject claimButtom;

        public bool unlocked;
        public Achievement achievement;

        public void RefreshView()
        {
            titleLabel.text = achievement.title;
            descriptionLabel.text = achievement.description;

            unlockedIcon.enabled = unlocked;
            lockedIcon.enabled = !unlocked;

            progressBar.Refresh(achievement.progress, achievement.changingValue[0]);

            claimButtom.SetActive(achievement.IsReadyToClaim());
        }

        public void OnClaimButtonPressed()
        {
            //increment userValue from achievement controller and reset the achivement
            Debug.Log("reward user");
            achievement.IncrementUserLevelNreset();
            RefreshView();
        }

    }
}
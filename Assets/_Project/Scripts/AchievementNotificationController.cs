using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AchievementSystem
{
    /// <summary>
    /// For notification animation
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AchievementNotificationController : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI achievementTitleLabel;

        private Animator m_animator;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void ShowNotification(Achievement achievement)
        {
            achievementTitleLabel.text = achievement.title;
            m_animator.SetTrigger("Appear");
        }
    }

}
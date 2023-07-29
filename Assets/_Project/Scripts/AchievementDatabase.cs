using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AchievementSystem
{
    /// <summary>
    /// Contain list of all the achievements
    /// </summary>
    [CreateAssetMenu()]
    public class AchievementDatabase : ScriptableObject
    {

        public List<Achievement> achievements;

        public Achievement GetAchievement(string Id)
        {
            foreach (var achievement in achievements)
            {
                if (Id == achievement.id) return achievement;
            }
            return null;
        }
    }
}
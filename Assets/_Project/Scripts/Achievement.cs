using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AchievementSystem
{
    /// <summary>
    /// Base class contain achievement data
    /// </summary>
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public int initialValue = 1;
        public string example;

        //ChangingValue value
        [NonSerialized]
        public int userLevel = 1;
        [NonSerialized]
        public int[] changingValue;
        [NonSerialized]
        public char charValue;

        public Achievement(string id, string title, string description,
            int userLevel, int[] changingValue, char charValue)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.userLevel = userLevel;
            this.changingValue = changingValue;
            this.charValue = charValue;
        }

        public Achievement(string id, string title, string description,
          int userLevel, int[] changingValue)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.userLevel = userLevel;
            this.changingValue = changingValue;
        }
    }

    //// BeatLevelAchievement class specific to "BeatLevel" achievement type
    //public class BeatLevelAchievement : Achievement
    //{
    //    public int[] changingValue;

    //    public BeatLevelAchievement(Achievement baseAchievement, int userLevel)
    //    {
    //        // Initialize the common properties from the base achievement
    //        id = baseAchievement.id;
    //        title = baseAchievement.title;
    //        description = baseAchievement.description;
    //        initialValue = baseAchievement.initialValue;
    //        example = baseAchievement.example;
    //        this.userLevel = userLevel;

    //        // Additional properties specific to BeatLevelAchievement
    //        changingValue = new int[] { baseAchievement.initialValue * userLevel };
    //        description = string.Format(description, changingValue[0]);
    //    }
    //}

    //// PlayWordAchievement class specific to "PlayWord" achievement type
    //public class PlayWordAchievement : Achievement
    //{
    //    public int[] changingValue;
    //    public char charValue;

    //    public PlayWordAchievement(Achievement baseAchievement, char randomChar, int userLevel)
    //    {
    //        // Initialize the common properties from the base achievement
    //        id = baseAchievement.id;
    //        title = baseAchievement.title;
    //        description = baseAchievement.description;
    //        initialValue = baseAchievement.initialValue;
    //        example = baseAchievement.example;
    //        this.userLevel = userLevel;

    //        // Additional properties specific to PlayWordAchievement
    //        changingValue = new int[] { baseAchievement.initialValue * userLevel };
    //        description = string.Format(description, changingValue[0], randomChar);
    //        charValue = randomChar;
    //        title = string.Format(title, randomChar);
    //    }
    //}

    //// DefaultAchievement class for handling unrecognized achievement types
    //public class DefaultAchievement : Achievement
    //{
    //    public DefaultAchievement(Achievement baseAchievement)
    //    {
    //        // Initialize the common properties from the base achievement
    //        id = baseAchievement.id;
    //        title = baseAchievement.title;
    //        description = baseAchievement.description;
    //        initialValue = baseAchievement.initialValue;
    //        example = baseAchievement.example;
    //        userLevel = baseAchievement.userLevel;
    //    }
    //}   
}
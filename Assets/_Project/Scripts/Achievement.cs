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
 
}
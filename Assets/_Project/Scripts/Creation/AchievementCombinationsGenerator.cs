using System.Collections.Generic;
using System;

namespace AchievementSystem
{
    // Generator for getting random unique combinations of achievements
    public class AchievementCombinationsGenerator
    {
        public List<AchievementID> GetRandomUniqueCombinations(List<AchievementID> allValues, int count)
        {
            System.Random random = new System.Random();
            int n = allValues.Count;

            // Use Fisher-Yates (Knuth) shuffle algorithm
            for (int i = 0; i < n; i++)
            {
                int randIndex = i + random.Next(n - i);
                AchievementID temp = allValues[randIndex];
                allValues[randIndex] = allValues[i];
                allValues[i] = temp;
            }

            return allValues.GetRange(0, Math.Min(n, count));
        }
    }
}
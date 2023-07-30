using System;
using System.Collections.Generic;

public class WeightedRandomSelector<T>
{
    private List<Tuple<T, float>> weightedList = new List<Tuple<T, float>>();
    private Random random = new Random();

    public void Add(T value, float weight)
    {
        weightedList.Add(new Tuple<T, float>(value, weight));
    }

    public T Select()
    {
        float totalWeight = 0f;

        foreach (var item in weightedList)
        {
            totalWeight += item.Item2;
        }

        float randomValue = (float)random.NextDouble() * totalWeight;

        foreach (var item in weightedList)
        {
            randomValue -= item.Item2;

            if (randomValue <= 0)
            {
                return item.Item1;
            }
        }

        // This should not happen, but just in case...
        return weightedList[0].Item1;
    }
}

using System;

public class RandomSeed
{
    private readonly Random rdm;

    public RandomSeed(int seed)
    {
        rdm = new Random(seed);
    }

    public int Range(int min, int max)
    {
        return rdm.Next(min, max);
    }

    public float Value()
    {
        return (float)rdm.NextDouble();
    }

    public bool Chance(float probability)
    {
        return Value() <= probability;
    }
}
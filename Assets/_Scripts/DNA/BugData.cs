#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;

#endregion

namespace Assets._Scripts.DNA
{
    public struct BugData
    {
        public float Health { get; private set; }
        public float FoodHealth { get; private set; }
        public float HealthLoss { get; private set; }
        public int Parts { get; private set; }
        public float ChildHealth { get; private set; }

        public BugData(float health, float foodHealth, float healthLoss, int sumParts, float childHealth) : this()
        {
            Health = health;
            FoodHealth = foodHealth;
            HealthLoss = healthLoss;
            Parts = sumParts;
            ChildHealth = childHealth;
        }
    }
}
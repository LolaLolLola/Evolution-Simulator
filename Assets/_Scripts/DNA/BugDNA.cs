using System;
using System.Collections.Generic;
using Assets._Scripts.Bugs;
using Assets._Scripts.Enums;
using Assets._Scripts.Enums.Bug;

namespace Assets._Scripts.DNA
{
    public class BugDNA
    {
        public float this[BugValue index] { get { return mBugValues[index]; } set { mBugValues[index] = value; } }

        private Dictionary<BugValue, float> mBugValues;

        public BugStructur Stucture { get; private set; }
        public float[][][] Weights { get; private set; }
        public float[][] Bases { get; private set; }

        public BugDNA(BugStructur stucture, float[][][] weights, float[][] bases)
        {
            mBugValues = new Dictionary<BugValue, float>();

            foreach (BugValue item in Enum.GetValues(typeof(BugValue)))
            {
                mBugValues[item] = 0.0f;
            }

            Stucture = stucture;
            Weights = weights;
            Bases = bases;
        }

        public void SetValues(float mutationRate, float speed, float turnMomentum, float foodHealt, float sigthDistance)
        {
            mBugValues[BugValue.MutationRate] = mutationRate;
            mBugValues[BugValue.MutationRate] = speed;
            mBugValues[BugValue.TurnMomentum] = turnMomentum;
            mBugValues[BugValue.FoodHealth] = foodHealt;
            mBugValues[BugValue.SightDistance] = sigthDistance;
        }
    }
}

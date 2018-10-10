#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using Assets._Scripts.Generators;
using UnityEngine;
using Assets._Scripts.M;

using static Assets._Scripts.M.Mathn;
using static System.Math;

#endregion

namespace Assets._Scripts.Bugs.Parts
{
    public class Brain
    {
        private readonly float mLerningRateFood = 0.010f;
        private readonly float mLerningRateWall = 0.005f;
        private readonly float mLerningRateMove = 0.002f;
        private float mMoveDistance = 7.0f;

        private NeuralNetwork mNetwork;
        private float mPreDisntaceFood;
        private float mPreDisntaceWall;
        private float mPostDisntaceFood;
        private float mPostDisntaceWall;

        public float[][][] Weights => mNetwork.Weigths;
        public float[][] Bases => mNetwork.Bases;
        public float[] Output(float[] input) => mNetwork.FeedForward(input);
        public float Speed { set { mMoveDistance = value * 0.7f; } }

        public Brain(bool random)
        {
            mNetwork = new NeuralNetwork(new int[] { 6, 10, 8, 2 }, GeneratorController.Random);

            if (random)
                mNetwork.RandomWeights();
        }

        public void SetNeuronValues(float[][][] weights, float[][] bases)
        {
            mNetwork.SetBases(bases);
            mNetwork.SetWeights(weights);
        }

        public void Train(EyeInfo info, Vector3 prePos, Vector3 postPos)
        {
            if (info.IsFood)
            {
                mPreDisntaceFood = Abs(Distance(info.Food.x, info.Food.z, prePos.x, prePos.z));
                mPostDisntaceFood = Abs(Distance(info.Food.x, info.Food.z, postPos.x, postPos.z));
            }

            if (info.IsWall)
            {
                mPreDisntaceWall = Abs(Distance(info.Wall.x, info.Wall.z, prePos.x, prePos.z));
                mPostDisntaceWall = Abs(Distance(info.Wall.x, info.Wall.z, postPos.x, postPos.z));
            }

            if (info.IsFood && (mPreDisntaceFood < mPostDisntaceFood))
            {
                mNetwork.TrainOne(mNetwork.Input, FindBiggest(mNetwork.Output), mLerningRateFood);
            }
            else if (info.IsFood)
            {
                float[] abs = FindBiggest(mNetwork.Output);
                mNetwork.TrainOne(mNetwork.Input, new float[] { abs[1], abs[0] }, mLerningRateFood);
                return;
            }

            if (info.IsWall && (mPreDisntaceWall > mPostDisntaceWall))
            {
                mNetwork.TrainOne(mNetwork.Input, FindBiggest(mNetwork.Output), mLerningRateWall);
                return;
            }
            else if (info.IsWall)
            {
                float[] abs = FindBiggest(mNetwork.Output);
                mNetwork.TrainOne(mNetwork.Input, new float[] { abs[1], abs[0] }, mLerningRateWall);
                return;
            }

            if (Abs(Distance(prePos.x, prePos.z, postPos.x, postPos.z)) < mMoveDistance * Time.deltaTime)
            {
                mNetwork.TrainOne(mNetwork.Input, FindBiggest(mNetwork.Output), mLerningRateMove);
            }
            else
            {
                float[] abs = FindBiggest(mNetwork.Output);
                mNetwork.TrainOne(mNetwork.Input, new float[] { abs[1], abs[0] }, mLerningRateMove);
            }
        }
    }
}
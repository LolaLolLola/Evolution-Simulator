#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using Assets._Scripts.DNA;
using Assets._Scripts.Enums;
using Assets._Scripts.Enums.Bug;
using Assets._Scripts.Generators;
using Assets._Scripts.M;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs.Parts.MainPart
{
    public partial class MainBugPart
    {
        private M.Random mRandom;

        public BugDNA Crossover(BugDNA other)
        {
            mRandom = GeneratorController.Random;

            BugDNA that = pGetBugDNA();
            BugDNA dna = new BugDNA(Crossover(other.Stucture, that.Stucture), Crossover(other.Weights, that.Weights), Crossover(other.Bases, that.Bases));

            foreach (BugValue item in Enum.GetValues(typeof(BugValue)))
            {
                dna[item] = mRandom.NextBool() ? that[item] : other[item];
            }

            return dna;
        }

        private BugStructur Crossover(BugStructur other, BugStructur that)
        {
            BugStructur buffer = new BugStructur();

            other.CalculatePoses();
            that.CalculatePoses();

            int MaxX = (int)Mathn.GetBigger(other.MaxX(), that.MaxX());
            int MaxY = (int)Mathn.GetBigger(other.MaxY(), that.MaxY());
            int MinX = (int)Mathn.GetBigger(other.MinX(), that.MinX());
            int MinY = (int)Mathn.GetBigger(other.MinY(), that.MinY());

            for (int x = MinX; x <= MaxX; x++)
            {
                for (int y = MinY; y <= MaxY; y++)
                {
                    BugPartData th = that.GetPartData(new Vector2(x, y));
                    BugPartData ot = that.GetPartData(new Vector2(x, y));

                    if (th == null && ot == null) continue;

                    if (ot == th)
                    {
                        buffer.AddVecPart(new Vector2(x, y), th.Fuction);
                        continue;
                    }

                    if (th != null && ot != null)
                    {
                        buffer.AddVecPart(new Vector2(x, y), mRandom.NextBool() ? th.Fuction : ot.Fuction);
                        continue;
                    }

                    if (th == null) th = ot;

                    if (mRandom.NextBool()) buffer.AddVecPart(new Vector2(x, y), th.Fuction);
                }
            }

            buffer.WriteVecParts();
            return buffer;
        }
        
        private float[][][] Crossover(float[][][] other, float[][][] that)
        {
            float[][][] buffer = new float[that.Length][][];

            for (int i = 0; i < that.Length; i++)
            {
                buffer[i] = new float[that[i].Length][];

                for (int n = 0; n < that[i].Length; n++)
                {
                    buffer[i][n] = new float[that[i][n].Length];

                    for (int w = 0; w < that[i][n].Length; w++)
                    {
                        buffer[i][n][w] = mRandom.NextBool() ? other[i][n][w] : that[i][n][w];
                    }
                }
            }

            return buffer;
        }

        private float[][] Crossover(float[][] other, float[][] that)
        {
            float[][] buffer = new float[that.Length][];

            for (int i = 0; i < that.Length; i++)
            {
                buffer[i] = new float[that[i].Length];

                for (int n = 0; n < that[i].Length; n++)
                {
                    buffer[i][n] = mRandom.NextBool() ? other[i][n] : that[i][n];
                }
            }

            return buffer;
        }
    }
}
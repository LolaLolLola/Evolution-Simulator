#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;

#endregion

namespace Assets._Scripts.M
{
    public class Random : System.Random
    {
        public Random(int seed) : base(seed) { }

        public float NextFloat(float max, int decimals = 5)
        {
            return Next((int)(max * Math.Pow(10, decimals))) / (float)Math.Pow(10, decimals);
        }

        public float NextFloat(float min, float max, int decimals = 5)
        {
            return Next((int)(min * Math.Pow(10, decimals)), (int)(max * Math.Pow(10, decimals))) / (float)Math.Pow(10, decimals);
        }

        public float[] NextFloatArray(float max, int length, int decimals = 5)
        {
            float[] r = new float[length];

            for (int i = 0; i < length; i++)
            {
                r[i] = NextFloat(max, decimals);
            }

            return r;
        }

        public float[] NextFloatArray(float min, float max, int length, int decimals = 5)
        {
            float[] r = new float[length];

            for (int i = 0; i < length; i++)
            {
                r[i] = NextFloat(min, max, decimals);
            }

            return r;
        }

        public bool NextBool()
        {
            return NextDouble() < 0.5f;
        }
    }
}

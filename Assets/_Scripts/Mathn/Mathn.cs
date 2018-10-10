#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;

#endregion

namespace Assets._Scripts.M
{
    public static class Mathn
    {
        public static float Sigmoid(float value)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        public static float DifSigmoid(float value)
        {
            float buffer = Sigmoid(value);
            return buffer * (1 - buffer);
        }

        public static float[] Softmax(float[] input)
        {
            float buffer = 0;

            for (int i = 0; i < input.Length; i++)
            {
                buffer += (float)Math.Pow(Math.E, input[i]);
            }

            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (float)Math.Pow(Math.E, input[i]) / buffer;
            }

            return input;
        }

        public static float DifSoftmax(float value)
        {
            return (1 - value) * value;
        }

        public static float Sum(IEnumerable<float> vlaues)
        {
            float buffer = 0;
            foreach (float item in vlaues)
            {
                buffer += item;
            }

            return buffer;
        }

        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Pow(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2), 0.5);
        }

        public static float[] FindBiggest(float[] input)
        {
            int index = 0;
            float buffer = input[0];

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] > buffer)
                {
                    index = i;
                }
            }

            float[] re = new float[input.Length];
            re[index] = 1;
            return re;
        }

        public static T SelectRandmo<T>(T[] array, Random random)
        {
            return array[random.Next(0, array.Length - 1)];
        }

        public static float GetBigger(float first, float second)
        {
            if (first > second) return first;

            return second;
        }
    }
}

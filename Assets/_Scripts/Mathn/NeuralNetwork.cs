#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Assets._Scripts.M
{
    public class NeuralNetwork
    {
        public float[] Input { get; private set; }
        public float[] Output { get; private set; }
        public int InputNodes { get; private set; }
        public int OutputNodes { get; private set; }
        public int[] HiddenNodes { get; private set; }
        public int[] Nodes { get; private set; }
        public int Length { get; private set; }
        public float LerningRate { get; private set; }
        public int ItemsTrained { get; private set; }
        public float OverallAccurecy { get; private set; }
        public float[][][] Weigths { get; private set; }
        public float[][] Bases { get; private set; }

        private float[][] mValues;

        private float[][][] mWeigthsDeltas;
        private float[][] mSignals;

        private float[][][] mPreWeigthsDeltas;

        private Random mRandom;
        private List<float> mDiffernces;

        public event Action ValueChange;

        public NeuralNetwork(int[] nodes, Random random)
        {
            InputNodes = nodes[0];
            OutputNodes = nodes[nodes.Length - 1];

            List<int> tempList = nodes.ToList();
            tempList.RemoveAt(0);
            Nodes = tempList.ToArray();

            tempList = nodes.ToList();
            tempList.RemoveAt(0);
            tempList.RemoveAt(tempList.Count - 1);
            HiddenNodes = tempList.ToArray();

            Length = nodes.Length - 1;
            LerningRate = 0.1f;
            ItemsTrained = 0;

            mRandom = random;
            mDiffernces = new List<float>();
            GenerateArrays();
        }

        private void GenerateArrays()
        {
            Weigths = new float[Length][][];
            Bases = new float[Length][];
            mValues = new float[Length][];
            mWeigthsDeltas = new float[Length][][];
            mSignals = new float[Length][];
            mPreWeigthsDeltas = new float[Length][][];

            for (int l = 0; l < Length; l++)
            {
                Weigths[l] = new float[Nodes[l]][];
                Bases[l] = new float[Nodes[l]];
                mValues[l] = new float[Nodes[l]];
                mWeigthsDeltas[l] = new float[Nodes[l]][];
                mSignals[l] = new float[Nodes[l]];
                mPreWeigthsDeltas[l] = new float[Nodes[l]][];

                for (int n = 0; n < Nodes[l]; n++)
                {
                    if (l == 0)
                    {
                        Weigths[l][n] = new float[InputNodes];
                        mWeigthsDeltas[l][n] = new float[InputNodes];
                        mPreWeigthsDeltas[l][n] = new float[InputNodes];
                    }
                    else
                    {
                        Weigths[l][n] = new float[Nodes[l - 1]];
                        mWeigthsDeltas[l][n] = new float[Nodes[l - 1]];
                        mPreWeigthsDeltas[l][n] = new float[InputNodes];
                    }
                }
            }
        }

        public void RandomWeights(float min = 0, float max = 1)
        {
            for (int l = 0; l < Length; l++)
            {
                for (int n = 0; n < Nodes[l]; n++)
                {
                    Bases[l][n] = mRandom.NextFloat(min, max, 7);

                    if (l == 0)
                    {
                        for (int i = 0; i < InputNodes; i++)
                        {
                            Weigths[l][n][i] = mRandom.NextFloat(min, max, 7);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Nodes[l - 1]; i++)
                        {
                            Weigths[l][n][i] = mRandom.NextFloat(min, max, 7);
                        }
                    }
                }
            }
        }

        public float[] FeedForward(float[] input)
        {
            Input = input;

            for (int l = 0; l < Length; l++)
            {
                for (int j = 0; j < Nodes[l]; j++)
                {
                    mValues[l][j] = 0;

                    if (l == 0)
                    {
                        for (int i = 0; i < InputNodes; i++)
                        {
                            mValues[l][j] += Input[i] * Weigths[l][j][i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Nodes[l - 1]; i++)
                        {
                            mValues[l][j] += mValues[l - 1][i] * Weigths[l][j][i];
                        }
                    }

                    if (l < Length - 1)
                    {
                        mValues[l][j] = Mathn.Sigmoid(mValues[l][j]);
                    }
                }
            }

            mValues[Length - 1] = Mathn.Softmax(mValues[Length - 1]);
            Output = mValues[Length - 1];

            return Output;
        }

        public void TrainOne(float[] input, float[] output, float lerningrate)
        {
            float lerningratebuffer;
            lerningratebuffer = LerningRate;
            LerningRate = lerningrate;

            TrainOne(input, output);

            LerningRate = lerningratebuffer;
        }

        public void TrainOne(float[] input, float[] output)
        {
            if (LerningRate <= 0) return;

            FeedForward(input);

            Signals(output);
            CalculateWeights(input);
            ApplyWeights();
            CalculateBases();

            Accuracy(output);
        }


        private void Signals(float[] output)
        {
            float[] dif = new float[OutputNodes];
            for (int i = 0; i < OutputNodes; i++)
            {
                dif[i] = Output[i] - output[i];
            }

            for (int j = 0; j < OutputNodes; j++)
            {
                mSignals[Length - 1][j] = Mathn.DifSoftmax(Output[j]) * dif[j];
            }

            for (int l = Length - 2; l >= 0; l--)
            {
                for (int j = 0; j < Nodes[l]; j++)
                {
                    float sum = 0;

                    for (int k = 0; k < Nodes[l + 1]; k++)
                    {
                        sum += Weigths[l + 1][k][j] * mSignals[l + 1][k];
                    }

                    mSignals[l][j] = Mathn.DifSigmoid(mValues[l][j]) * sum; //Check Value
                }
            }
        }

        private void CalculateWeights(float[] input)
        {
            for (int l = 0; l < Length; l++)
            {
                for (int j = 0; j < Nodes[l]; j++)
                {
                    if (l == 0)
                    {
                        for (int i = 0; i < InputNodes; i++)
                        {
                            mWeigthsDeltas[l][j][i] = -LerningRate * mSignals[l][j] * input[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Nodes[l - 1]; i++)
                        {
                            mWeigthsDeltas[l][j][i] = -LerningRate * mSignals[l][j] * mValues[l - 1][i];
                        }
                    }
                }
            }
        }

        private void ApplyWeights()
        {
            for (int l = 0; l < Length; l++)
            {
                for (int j = 0; j < Nodes[l]; j++)
                {
                    if (l == 0)
                    {
                        for (int i = 0; i < InputNodes; i++)
                        {
                            Weigths[l][j][i] += mWeigthsDeltas[l][j][i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Nodes[l - 1]; i++)
                        {
                            Weigths[l][j][i] += mWeigthsDeltas[l][j][i];
                        }
                    }
                }
            }
        }

        private void CalculateBases()
        {
            for (int l = 0; l < Length; l++)
            {
                for (int j = 0; j < Nodes[l]; j++)
                {
                    Bases[l][j] += -LerningRate * mSignals[l][j];
                }
            }
        }

        private void Accuracy(float[] output)
        {
            float buffer = 0;

            for (int i = 0; i < OutputNodes; i++)
            {
                buffer += (float)Math.Pow(output[i] - Output[i], 2);
            }
            mDiffernces.Add(buffer);

            if (mDiffernces.Count > 200)
            {
                mDiffernces.RemoveAt(0);
            }

            buffer = 0;

            foreach (float item in mDiffernces)
            {
                buffer += item;
            }

            OverallAccurecy = buffer / mDiffernces.Count;

            ValueChange?.Invoke();
        }

        public void SetWeights(float[][][] weights)
        {
            for (int l = 0; l < Weigths.Length; l++)
            {
                for (int n = 0; n < Weigths[l].Length; n++)
                {
                    for (int w = 0; w < Weigths[l][n].Length; w++)
                    {
                        Weigths[l][n][w] = weights[l][n][w];
                    }
                }
            }
        }

        public void SetBases(float[][] bases)
        {
            for (int l = 0; l < Bases.Length; l++)
            {
                for (int n = 0; n < Bases[l].Length; n++)
                {
                    Bases[l][n] = bases[l][n];
                }
            }
        }
    }
}
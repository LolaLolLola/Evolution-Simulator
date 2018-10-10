#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace Assets._Scripts.Generators
{
    public class FoodGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject mFood;
        [SerializeField] private int mStartAmount;
        [SerializeField] private float mWallSpace;
        [SerializeField] private int mFoodDelay;
        private Bounds mBounds;
        private bool mRunning;

        [HideInInspector] public List<Food> Foods { get; private set; }

        private void Start()
        {
            Foods = new List<Food>();
            mRunning = true;
        }

        private void OnDestroy()
        {
            mRunning = false;
        }

        public void Load(Bounds bounds)
        {
            mBounds = bounds;
            mBounds.SetMinMax(new Vector3(mBounds.min.x + mWallSpace, mBounds.min.y, mBounds.min.z - mWallSpace), new Vector3(mBounds.max.x - mWallSpace, mBounds.max.y, mBounds.max.z + mWallSpace));

            for (int i = 0; i < mStartAmount; i++)
            {
                Vector3 pos = GetRandomPos();
                Foods.Add(MakeFood(pos.x, pos.z));
            }
        }

        public Vector3 GetRandomPos()
        {
            float x;
            try
            {
                x = GeneratorController.Random.NextFloat(mBounds.min.x, mBounds.max.x, 2);
            }
            catch
            {
                x = GeneratorController.Random.NextFloat(mBounds.max.x, mBounds.min.x, 2);
            }

            float z;
            try
            {
                z = GeneratorController.Random.NextFloat(mBounds.min.z, mBounds.max.z, 2);
            }
            catch
            {
                z = GeneratorController.Random.NextFloat(mBounds.max.z, mBounds.min.z, 2);
            }

            return new Vector3(x, 0, z);
        }

        private Food MakeFood(float posX, float posZ)
        {
            Food f = Instantiate(mFood, new Vector3(posX, 0, posZ), Quaternion.Euler(Vector3.zero)).GetComponent<Food>();
            f.GetEaten += DestroyFood;
            f.MakeNew += MakeOneNew;
            return f;
        }

        private void DestroyFood(Food food) // Subscribed Function
        {
            Foods.Remove(food);
        }

        public async void MakeOneNew() // Subscribed Function
        {
            await Task.Delay(mFoodDelay);
            if (!mRunning) return;

            Vector3 b = GetRandomPos();
            Foods.Add(MakeFood(b.x, b.z));
        }
    }
}
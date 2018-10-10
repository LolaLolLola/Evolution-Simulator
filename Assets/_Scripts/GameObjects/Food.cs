#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;

#endregion

namespace Assets._Scripts.GameObjects
{
    public class Food : MonoBehaviour
    {
        public event Action<Food> GetEaten;
        public event Action MakeNew;

        private void OnDestroy()
        {
            GetEaten?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            MakeNew?.Invoke();
            Destroy(gameObject);
        }
    }
}
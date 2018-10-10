#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;

#endregion

namespace Assets._Scripts.GameObjects
{
    public class ColliderSubscriber : MonoBehaviour
    {
        public event Action<Collider> ColliderStay;

        private void OnTriggerStay(Collider collision)
        {
            ColliderStay?.Invoke(collision);
        }
    }
}
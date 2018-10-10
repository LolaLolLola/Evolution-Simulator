#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Assets._Scripts.UI
{
    public class ClickDetector : MonoBehaviour, IPointerClickHandler
    {
        public event Action Click;

        public void OnPointerClick(PointerEventData eventData)
        {
            Click?.Invoke();
        }
    }
}
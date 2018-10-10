#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Assets._Scripts.UI
{
    public class EnterDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action Enter;
        public event Action Exit;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Enter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit?.Invoke();
        }
    }
}
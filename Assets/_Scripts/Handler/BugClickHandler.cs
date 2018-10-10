#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs;
using UnityEngine;

#endregion

namespace Assets._Scripts.Handler
{
    public class BugClickHandler : MonoBehaviour
    {
        public Bug Parent { get; private set; }

        private void Start()
        {
            Parent = gameObject.transform.parent.GetComponent<Bug>();
        }
    }
}
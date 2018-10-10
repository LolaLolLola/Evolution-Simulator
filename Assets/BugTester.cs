#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs;
using UnityEngine;

#endregion

namespace Assets._Scripts
{
    public class BugTester : MonoBehaviour
    {
        public GameObject Bug;

        public Bug MakeBug()
        {
            return Instantiate(Bug, Vector3.zero, Quaternion.Euler(0, 0, 0)).GetComponent<Bug>();
        }
    }
}
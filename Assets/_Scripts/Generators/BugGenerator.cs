#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs;
using Assets._Scripts.Bugs.Parts.MainPart;
using Assets._Scripts.DNA;
using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Assets._Scripts.Generators
{
    public class BugGenerator : MonoBehaviour
    {
        #region Serializeable

        [SerializeField] private GameObject mBug;
        [SerializeField] private int mStartGenSize;
        [SerializeField] private int mMaxBugs;

        #endregion

        private Func<Vector3> mGetRandomPos;
        private M.Random mRandom;

        public List<Bug> Bugs { get; private set; }

        public int GenSize { get { return Bugs.Count; } }

        private void Start()
        {
            Bugs = new List<Bug>();
        }

        public void Load(Func<Vector3> getRandomPos, M.Random random)
        {
            mGetRandomPos = getRandomPos;
            mRandom = random;

            for (int i = 0; i < mStartGenSize; i++)
            {
                Vector3 p = mGetRandomPos();
                Bugs.Add(MakeBug(p.x, p.z));
            }
        }

        public void RegisterBug(Bug bug)
        {
            throw new NotImplementedException();
        }

        private Bug MakeBug(float posX, float posZ)
        {
            Bug b = Instantiate(mBug, new Vector3(posX, 0, posZ), Quaternion.Euler(0, 0, 0)).GetComponent<Bug>();
            Bugs.Add(b);
            b.Load();
            return b;
        }

        private void DestroyBug(MainBugPart bug) // Subscribed Function
        {
            Bugs.Remove((Bug)bug);
        }

        private void MakeChild(float posX, float posZ, BugDNA dna) // Subscribed Function
        {
            throw new NotImplementedException();
        }

        private void MakeChildTogether(float posX, float posZ, BugDNA dna1, BugDNA dna2) // Subscribed Function
        {
            throw new NotImplementedException();
        }
    }
}
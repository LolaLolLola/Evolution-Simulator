#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs.Parts
{
    public class Eye : BugPart
    {
        [SerializeField] private int mRayAmount;

        private float mRayDegree;
        private Ray[] mRays;
        private RaycastHit[] mRaycastHits;

        public bool Food { get; private set; }
        public bool Wall { get; private set; }
        public bool Bug { get; private set; }
        public float DistanceFood { get; private set; }
        public float DistanceWall { get; private set; }
        public float DistanceBug { get; private set; }
        public Vector3 ClosestFood { get; private set; }
        public Vector3 ClosestWall { get; private set; }
        public Vector3 ClosestBug { get; private set; }

        public EyeInfo GetEyeInfo => new EyeInfo(ClosestFood, ClosestWall, ClosestBug, DistanceWall, DistanceWall, DistanceBug, Food, Wall, Bug);

        public void Awake()
        {
            mRays = new Ray[(mRayAmount * 2) - 1];
            mRaycastHits = new RaycastHit[(mRayAmount * 2) - 1];
            mRayDegree = 90 / mRayAmount;

            for (int i = 0; i < mRayAmount; i++)
            {
                mRays[i] = new Ray();
                mRaycastHits[i] = new RaycastHit();
            }
        }

        public void Look(int layer, float seightDistance)
        {
            CalculateRay();
            MakeRayCasts(layer, seightDistance);
        }

        private void CalculateRay()
        {
            Vector3 forward = Quaternion.Euler(0, 90 * (int)pParentConnection, 0) * Main.transform.forward;
            for (int i = 0; i < mRayAmount; i++)
            {
                mRays[i].origin = transform.position + (forward * 1.2f);
                mRays[i].direction = Quaternion.Euler(new Vector3(0, mRayDegree * i , 0)) * forward;
            }

            for (int i = mRayAmount; i < (mRayAmount * 2) - 1; i++)
            {
                mRays[i].origin = transform.position + (forward * 1.2f);
                mRays[i].direction = Quaternion.Euler(new Vector3(0, -mRayDegree * (i - mRayAmount + 1), 0)) * forward;
            }
        }

        private void MakeRayCasts(int layer, float seightDistance)
        {
            DistanceFood = seightDistance + 1;
            DistanceWall = DistanceFood;
            DistanceBug = DistanceFood;

            ClosestFood = Vector3.zero;
            ClosestWall = Vector3.zero;
            ClosestBug = Vector3.zero;

            Food = false;
            Wall = false;
            Bug = false;

            for (int i = 0; i < mRays.Length; i++)
            {
                if (Physics.Raycast(mRays[i], out mRaycastHits[i], seightDistance, layer))
                {
                    switch (mRaycastHits[i].collider.tag)
                    {
                        case "Food":
                            if (mRaycastHits[i].distance > DistanceFood) break;

                            Food = true;
                            DistanceFood = mRaycastHits[i].distance;
                            ClosestFood = mRaycastHits[i].point;
                            break;

                        case "Wall":
                            if (mRaycastHits[i].distance > DistanceWall) break;

                            Wall = true;
                            DistanceWall = mRaycastHits[i].distance;
                            ClosestWall = mRaycastHits[i].point;
                            break;

                        case "Bug":
                            if (mRaycastHits[i].distance > DistanceBug) break;

                            Bug = true;
                            DistanceBug = mRaycastHits[i].distance;
                            ClosestBug = mRaycastHits[i].point;
                            break;
                    }
                }
            }
        }
    }

    public struct EyeInfo
    {
        public Vector3 Food { get; private set; }
        public Vector3 Wall { get; private set; }
        public Vector3 Bug { get; private set; }
        public float DistanceFood { get; private set; }
        public float DistanceWall { get; private set; }
        public float DistanceBug { get; private set; }
        public bool IsFood { get; private set; }
        public bool IsWall { get; private set; }
        public bool IsBug { get; private set; }

        public EyeInfo(Vector3 food, Vector3 wall, Vector3 bug, float distanceFood, float distanceWall, float distanceBug, bool isFood, bool isWall, bool isBug) : this()
        {
            Food = food;
            Wall = wall;
            Bug = bug;
            DistanceFood = distanceFood;
            DistanceWall = distanceWall;
            DistanceBug = distanceBug;
            IsFood = isFood;
            IsWall = isWall;
            IsBug = isBug;
        }
    }
}
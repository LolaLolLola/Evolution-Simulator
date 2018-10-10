#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Assets._Scripts.Generators
{
    public class WallGenearator : MonoBehaviour
    {
        public Camera MainCamera;
        public GameObject Wall;

        public Bounds CameraBounds { get; private set; }

        private Dictionary<string, GameObject> mWall;

        public void Load()
        {
            float maxX = MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x * 2;
            float maxZ = MainCamera.ScreenToWorldPoint(new Vector3(0, 0, MainCamera.pixelHeight)).z * 2;

            CameraBounds = new Bounds(MainCamera.transform.position, new Vector3(maxX, 0, maxZ));

            MakeWalls();
        }

        public void MakeWalls()
        {
            mWall = new Dictionary<string, GameObject>();

            mWall.Add("Right" ,Instantiate(Wall, new Vector3(CameraBounds.max.x, 0, CameraBounds.center.z), Quaternion.Euler(0, 0, 0)));
            mWall["Right"].transform.localScale = new Vector3(1, 1, CameraBounds.extents.z * -2);

            mWall.Add("Left", Instantiate(Wall, new Vector3(CameraBounds.min.x, 0, CameraBounds.center.z), Quaternion.Euler(0, 0, 0)));
            mWall["Left"].transform.localScale = new Vector3(1, 1, CameraBounds.extents.z * -2);

            mWall.Add("Top", Instantiate(Wall, new Vector3(CameraBounds.center.x, 0, CameraBounds.max.z), Quaternion.Euler(0, 0, 0)));
            mWall["Top"].transform.localScale = new Vector3((CameraBounds.extents.x * 2) - 1, 1, 1);

            mWall.Add("Bottom", Instantiate(Wall, new Vector3(CameraBounds.center.x, 0, CameraBounds.min.z), Quaternion.Euler(0, 0, 0)));
            mWall["Bottom"].transform.localScale = new Vector3((CameraBounds.extents.x * 2) - 1, 1, 1);
        }
    }
}
#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;

#endregion

namespace Assets._Scripts.Generators
{
    #region Generators

    [Serializable]
    public class Generators
    {
        public WallGenearator WallGenearator;
        public FoodGenerator FoodGenerator;
        public BugGenerator BugGenerator;
    }

    #endregion

    public class GeneratorController : MonoBehaviour
    {
        [SerializeField] private Generators mGenerators;
        [SerializeField] private float mTimeTillDisable;

        [HideInInspector] public WallGenearator WallGenearator;
        [HideInInspector] public FoodGenerator FoodGenerator;
        [HideInInspector] public BugGenerator BugGenerator;
        public static M.Random Random { get; private set; }
        public static int NormalLayerMask;
        public static int HornyLayerMask;
        public static int MeatLayerMask;
        public static int BugLayerMask;
        public static int BugClickLayerMask;
        public static bool DisableTimeFuction;

        private void Start()
        {
            WallGenearator = mGenerators.WallGenearator;
            WallGenearator.Load();
            if (Random == null) Random = new M.Random(DateTime.Now.Second);

            FoodGenerator = mGenerators.FoodGenerator;
            FoodGenerator.Load(WallGenearator.CameraBounds);

            BugGenerator = mGenerators.BugGenerator;
            BugGenerator.Load(FoodGenerator.GetRandomPos, Random);

            NormalLayerMask = LayerMask.GetMask(new string[] { "Food", "Wall" });
            HornyLayerMask = LayerMask.GetMask(new string[] { "Food", "Wall", "Bug" });
            MeatLayerMask = LayerMask.GetMask(new string[] { "Bug", "Wall" });
            BugLayerMask = LayerMask.GetMask(new string[] { "Bug" });
            BugClickLayerMask = LayerMask.GetMask(new string[] { "BugClick" });
            DisableTimeFuction = false;
        }

        private void Update()
        {
            if (DisableTimeFuction) return;
            if (Time.timeSinceLevelLoad > mTimeTillDisable)
                DisableTimeFuction = true;
        }

        public static M.Random SafeRandom()
        {
            if (Random == null) Random = new M.Random(DateTime.Now.Second);
            return Random;
        }
    }
}
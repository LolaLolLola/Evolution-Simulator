#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets._Scripts.DNA;
using Assets._Scripts.Enums;
using Assets._Scripts.Enums.Bug;
using Assets._Scripts.Generators;
using Assets._Scripts.Interfaces;
using Assets._Scripts.M;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs.Parts.MainPart
{
    [Serializable]
    public class Parts
    {
        public GameObject Eye;
        public GameObject Body;
    }

    public abstract partial class MainBugPart : MonoBehaviour, IConnectable
    {
        // Serialzed Fields
        [SerializeField] private Parts mParts;
        [SerializeField] private Animator mAnimator;

        // Protected Fields
        protected int pTaregtEyes { get; private set; }
        protected int pTargetBodys { get; private set; }
        protected Brain pBrain;
        protected Func<BugDNA> pGetBugDNA;

        // Priavte Fields
        private List<BugPart> mBugParts;
        private List<IConnectable> mConnectables;
        private Dictionary<Type, List<BugPart>> mFuctionParts;
        private BugStructur mBugStructur;

        // IConnectable Fields
        public int this[Connection value] { get { return mChildrenIDs[value]; } }
        private Dictionary<Connection, int> mChildrenIDs;

        // Public Fields
        public int PartID { get { return _PartID++; } private set { _PartID = value; } }
        private int _PartID;
        public List<Vector3> Locations { get; private set; }
        public readonly int ID = 0;

        // Events
        public event Action<MainBugPart> Destroied;

        protected async Task GernerateBug(int eyes, int bodys)
        {
            SetDefault(eyes, bodys);

            await GenerateParts();
        }

        protected void GenerateBug(BugStructur bugStructur)
        {
            mBugStructur = bugStructur;

            SetDefault(mBugStructur.AmountOf(typeof(Eye)), mBugStructur.AmountOf(typeof(Body)));

            GenerateParts(mBugStructur);
        }

        private void SetDefault(int eyes, int bodys)
        {
            pTaregtEyes = eyes;
            pTargetBodys = bodys;

            PartID = 1;

            mConnectables = new List<IConnectable>
            {
                this
            };

            Locations = new List<Vector3>
            {
                transform.position
            };

            mChildrenIDs = new Dictionary<Connection, int>
            {
                { Connection.Top, -1 },
                { Connection.Right, -1 },
                { Connection.Bottom, -1 },
                { Connection.Left, -1 }
            };

            mBugParts = new List<BugPart>();

            mFuctionParts = new Dictionary<Type, List<BugPart>>
            {
                { typeof(Eye), new List<BugPart>() }
            };
        }

        protected EyeInfo NextTarget(BugState state, float seightDistance)
        {
            switch (state)
            {
                case BugState.Normal:
                    return LookFor(GeneratorController.NormalLayerMask, seightDistance);
                case BugState.Horny:
                    return LookFor(GeneratorController.HornyLayerMask, seightDistance);
            }

            return new EyeInfo();
        }

        private EyeInfo LookFor(int layer, float sieghtDistance)
        {
            foreach (Eye item in mFuctionParts[typeof(Eye)])
                item.Look(layer, sieghtDistance);

            IEnumerable<Eye> food = from eye in mFuctionParts[typeof(Eye)] where (eye as Eye).Food orderby (eye as Eye).DistanceFood descending select eye as Eye;
            IEnumerable<Eye> wall = from eye in mFuctionParts[typeof(Eye)] where (eye as Eye).Wall orderby (eye as Eye).DistanceWall descending select eye as Eye;
            IEnumerable<Eye> bug = from eye in mFuctionParts[typeof(Eye)] where (eye as Eye).Bug orderby (eye as Eye).DistanceBug descending select eye as Eye;

            Eye aFood = default(Eye);
            Eye aWall = default(Eye);
            Eye aBug = default(Eye);

            foreach (Eye item in food)
            {
                aFood = item;
                break;
            }

            foreach (Eye item in wall)
            {
                aWall = item;
                break;
            }

            foreach (Eye item in bug)
            {
                aBug = item;
                break;
            }

            return new EyeInfo(aFood != default(Eye) ? aFood.ClosestFood : Vector3.zero,
                               aWall != default(Eye) ? aWall.ClosestWall : Vector3.zero,
                               aBug != default(Eye) ? aBug.ClosestBug : Vector3.zero,
                               aFood != default(Eye) ? aFood.DistanceFood : 0,
                               aWall != default(Eye) ? aWall.DistanceWall : 0,
                               aBug != default(Eye) ? aBug.DistanceBug : 0,
                               aFood != default(Eye),
                               aWall != default(Eye),
                               aBug != default(Eye));
        }

        protected IConnectable GetBugPart(int id)
        {
            if (id == 0) return this as IConnectable;
            foreach (BugPart item in from bug in mBugParts where bug.ID == id && bug is IConnectable select bug)
                return item as IConnectable;

            return null;
        }

        public virtual BugStructur GetBugStructur()
        {
            if (mBugStructur != null) return mBugStructur;

            BugStructur d = new BugStructur();

            foreach (BugPart item in mBugParts)
            {
                d.AddBugPart(item);
            }

            return d;
        }

        private GameObject GetPrefab(Type fuction)
        {
            if (fuction == typeof(Body)) return mParts.Body;
            if (fuction == typeof(Eye)) return mParts.Eye;

            throw new Exception("Invalid function type");
        }

        protected float MaxDistance()
        {
            IEnumerable<BugPart> p = from part in mBugParts orderby Math.Abs(Mathn.Distance(transform.position.x, transform.position.z, part.transform.position.x, part.transform.position.z)) ascending select part;

            foreach (BugPart item in p)
            {
                return Math.Abs(Mathn.Distance(transform.position.x, transform.position.z, item.transform.position.x, item.transform.position.z)) + (0.5f * item.transform.lossyScale.x);
            }

            return transform.lossyScale.x;
        }

        protected void OnDestroy()
        {
            Destroied?.Invoke(this);
        }

        protected void PickUp()
        {
            transform.position += new Vector3(0, 4, 0);
            mAnimator.SetTrigger("PickUp");
        }

        protected void Drop()
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            mAnimator.SetTrigger("Drop");
        }

        protected void Shake()
        {
            transform.Rotate(0, (float)Math.Sin(Time.time * 80f) * 3f, 0);
        }

        protected void OpenScan()
        {
            gameObject.layer = 13;

            foreach (Transform child in transform)
            {
                child.gameObject.layer = 13;
            }

            mAnimator.SetTrigger("OpenScan");
            transform.position = new Vector3(3, 4, 17);
            Debug.Log("Called");
        }

        protected void CloseScan()
        {
            gameObject.layer = 9;

            foreach (Transform child in transform)
            {
                child.gameObject.layer = 9;
            }
        }
    }
}
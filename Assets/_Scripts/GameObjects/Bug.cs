#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.DNA;
using Assets._Scripts.Enums;
using Assets._Scripts.GameObjects;
using Assets._Scripts.Generators;
using Assets._Scripts.M;
using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Assets._Scripts.GameObjects
{
    /*
    public class Bug : MonoBehaviour
    {
        [SerializeField] private GameObject mHornyColliderObj;
        [SerializeField] private Animator mAnim;
        [SerializeField] private int mRayAmount;
        [SerializeField] private float mSightDistance;
        [SerializeField] private float mSpeed;
        [SerializeField] private float mTurnMomentum;
        [SerializeField] private float mLerningrateFood;
        [SerializeField] private float mLerningrateWall;
        [SerializeField] private float mLerningrateWalk;
        [SerializeField] private int mWallDelay;
        [SerializeField] private float mMutationRate;
        [SerializeField] private float mBirthHealth;
        [SerializeField] private float mFoodHealth;
        [SerializeField] private float mHealthLossPerS;
        [SerializeField] private float mAChildHealth;
        [SerializeField] private float mMoveDistance;
        [SerializeField] private float mChildHealthLoss;
        [SerializeField] private float mTimeM;
        [SerializeField] private float mFixTime;
        [SerializeField] private float mChildHealth;
        [SerializeField] [HideInInspector] private Vector3 mNextAim;
        [SerializeField] [HideInInspector] private Vector3 mNextWall;
        private Ray[] mRays;
        private RaycastHit[] mRayHits;
        private float mRayDegree;
        private NeuralNetwork mBrain;
        private bool mWall { get { return _mWall; } set { SetmWall(value); } }
        private bool _mWall;
        private int mWallCounter;
        private bool mAim;
        private bool mSeeingOther;
        private float[] mDistances;
        private Material mBodyMaterial;

        public event Action<Bug> GetDestroyed;
        public event Action<float, float, DNAData> MakeChild;
        public event Action<float, float, DNAData, DNAData> MakeChildWith;

        public BugState State { get; private set; }
        public float Health { get; private set; }

        private void Start()
        {
            DefaultValues();

            if (mBrain == null)
            {
                mBrain = new NeuralNetwork(new int[] { 6, 12, 8, 2 }, GeneratorController.Random);
                mBrain.RandomWeights();
            }

            for (int i = 0; i < mRays.Length; i++)
            {
                mRays[i] = new Ray();
                mRayHits[i] = new RaycastHit();
            }
        }

        private void DefaultValues()
        {
            mRays = new Ray[mRayAmount * 2 - 1];
            mRayHits = new RaycastHit[mRayAmount * 2 - 1];
            mRayDegree = 90 / mRayAmount;
            Health = mBirthHealth;
            mDistances = new float[2];
            mHornyColliderObj.GetComponent<ColliderSubscriber>().ColliderStay += OnHornyColliderStay;
            mHornyColliderObj.SetActive(false);
            State = BugState.Normal;
            mSeeingOther = false;
            mBodyMaterial = GetComponent<Renderer>().material;
        }

        #region Unity

        private void Update()
        {
            switch (State)
            {
                case BugState.Normal:
                    Move();
                    HealthLoss();
                    break;
                case BugState.Horny:
                    Move();
                    HealthLoss();
                    break;
                case BugState.OverDel:
                    OverDel();
                    break;
                case BugState.InAir:
                    break;
                case BugState.OverScan:
                    break;
                case BugState.Scaned:
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (State == BugState.Normal || State == BugState.Horny) CheckHorny();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!(State == BugState.Horny || State == BugState.Normal)) return;

            if (other.tag == "Wall")
            {
                Destroy(gameObject);
            }
            else if (other.tag == "Food")
            {
                Health += mFoodHealth;
                Reproduction();
            }
            else if (State == BugState.Horny && other.tag == "Bug")
            {
                Bug b = other.gameObject.GetComponent<Bug>();

                if (b != null && b.State == BugState.Horny)
                {
                    b.MakeChildTogether(GetDNA());
                    Health -= mChildHealthLoss * 0.7f;
                    CheckHorny();
                }
            }
        }

        private void OnDestroy()
        {
            GetDestroyed?.Invoke(this);
        }

        #endregion

        #region Move

        private void CalculateRay()
        {
            for (int i = 0; i < mRayAmount; i++)
            {
                mRays[i].origin = transform.position + transform.right * 1.2f;
                mRays[i].direction = Quaternion.Euler(new Vector3(0, mRayDegree * i, 0)) * transform.right;
            }

            for (int i = mRayAmount; i < mRayAmount * 2 - 1; i++)
            {
                mRays[i].origin = transform.position + transform.right * 1.2f;
                mRays[i].direction = Quaternion.Euler(new Vector3(0, -mRayDegree * (i - mRayAmount + 1), 0)) * transform.right;
            }
        }

        private void MakeRayCasts()
        {
            float fooddist = 100;
            float walldist = 100;

            ResetNextValues();

            for (int i = 0; i < mRays.Length; i++)
            {
                if (Physics.Raycast(mRays[i], out mRayHits[i], mSightDistance, GeneratorController.NormalLayerMask))
                {
                    if (mRayHits[i].collider.tag == "Wall" && mRayHits[i].distance < walldist)
                    {
                        walldist = mRayHits[i].distance;
                        mNextWall = mRayHits[i].point;
                        mWall = true;
                    }
                    else if (mRayHits[i].collider.tag == "Food" && mRayHits[i].distance < fooddist && !mSeeingOther)
                    {
                        fooddist = mRayHits[i].distance;
                        mNextAim = mRayHits[i].collider.gameObject.transform.position;
                        mAim = true;
                    }
                }
            }

            mSeeingOther = false;
        }

        private void ResetNextValues()
        {
            mWall = false;

            if (!mWall)
            {
                mNextWall = Vector3.zero;
            }

            if (!mSeeingOther)
            {
                mAim = false;
                mNextAim = Vector3.zero;
            }
        }

        private void Move()
        {
            CalculateRay();
            MakeRayCasts();

            float[] input = new float[] { transform.position.x, transform.position.z, mNextAim.x, mNextAim.z, mNextWall.x, mNextWall.z };
            float[] output = mBrain.FeedForward(input);

            CalculateDistance();
            float preFoodDistance = mDistances[0];
            float preWallDistance = mDistances[1];
            Vector3 prePos = transform.position;

            transform.Rotate(0, (output[0] > output[1] ? -1 : 1) * mTurnMomentum * Time.deltaTime, 0);

            transform.position += transform.right * mSpeed * Time.deltaTime;

            CalculateDistance();
            float postFoodDistance = mDistances[0];
            float postWallDistance = mDistances[1];

            Train(input, output, preFoodDistance, preWallDistance, postFoodDistance, postWallDistance, prePos);
        }

        #endregion

        #region Train

        private void CalculateDistance()
        {
            if (mAim)
            {
                mDistances[0] = Math.Abs(Mathn.Mathn.Distance(transform.position.x, transform.position.z, mNextAim.x, mNextAim.z));
            }
            if (mWall)
            {
                mDistances[1] = Math.Abs(Mathn.Mathn.Distance(transform.position.x, transform.position.z, mNextWall.x, mNextWall.z));
            }
        }

        private void Train(float[] input, float[] output, float preFoodDistance, float preWallDistance, float postFoodDistance, float postWallDistance, Vector3 prePos)
        {
            if (mAim)
            {
                if (preFoodDistance < postFoodDistance)
                    mBrain.TrainOne(input, Mathn.Mathn.FindBiggest(output), mLerningrateFood);
                else
                    mBrain.TrainOne(input, Mathn.Mathn.FindBiggest(new float[] { output[1], output[0] }), mLerningrateFood);
            }
            else if (mWall)
            {
                if (preWallDistance > postWallDistance)
                    mBrain.TrainOne(input, Mathn.Mathn.FindBiggest(output), mLerningrateWall);
                else
                    mBrain.TrainOne(input, Mathn.Mathn.FindBiggest(new float[] { output[1], output[0] }), mLerningrateWall);
            }
            else
            {
                if (Math.Abs(Mathn.Mathn.Distance(prePos.x, prePos.z, transform.position.x, transform.position.z)) < mMoveDistance * Time.deltaTime)
                    mBrain.TrainOne(input, Mathn.Mathn.FindBiggest(output), mLerningrateWalk);
                else
                    mBrain.TrainOne(input, Mathn.Mathn.FindBiggest(new float[] { output[1], output[0] }), mLerningrateWalk);
            }
        }

        private void SetmWall(bool value)
        {
            if (value)
            {
                _mWall = true;
                mWallCounter = mWallDelay;
                return;
            }

            mWallCounter--;

            if (mWallCounter < 1)
            {
                mNextWall = Vector3.zero;
                _mWall = false;
            }
        }

        #endregion

        #region ReProduction

        public void MakeChildOff(DNAData data)
        {
            mBrain = new NeuralNetwork(new int[] { 6, 12, 8, 2 }, GeneratorController.Random);
            mBrain.SetBases(data.Bases);
            mBrain.SetWeights(data.Weights);

            mMutationRate = data.MutaionRate;
            mSightDistance = data.SightDistance;
            mSpeed = data.Speed;
            mTurnMomentum = data.TurnMomrntum;
            mBirthHealth = data.BirthHealth;
            mFoodHealth = data.FoodHealth;
            mAChildHealth = data.AChildHealth;
            mChildHealth = data.ChildHealth;
            mChildHealthLoss = data.ChildHealthLoss;

            mHealthLossPerS = (mSpeed * 0.1f * 0.4f) +
                              (mTurnMomentum * 0.0013f * 0.4f) +
                              (mFoodHealth * 0.066f * 0.2f);
            mChildHealthLoss = mAChildHealth * 0.55f + mChildHealth * 0.45f;
        }

        public void MakeChildTogether(DNAData data)
        {
            MakeChildWith?.Invoke(transform.position.x, transform.position.z, GetDNA(), data);
            Health -= mChildHealthLoss * 0.7f;
            CheckHorny();
        }

        public void Reproduction()
        {
            if (Health > mAChildHealth)
            {
                MakeChild?.Invoke(transform.position.x, transform.position.z, GetDNA());
                Health -= mChildHealthLoss;
            }
        }

        #endregion

        #region State Changes

        private void CheckHorny()
        {
            if (Health > mChildHealth && State == BugState.Normal)
            {
                mHornyColliderObj.SetActive(true);
                State = BugState.Horny;
                mBodyMaterial.color = new Color(212, 0, 244);
            }
            else if (Health < mChildHealth && State == BugState.Horny)
            {
                mHornyColliderObj.SetActive(false);
                State = BugState.Normal;
                mBodyMaterial.color = Color.red;
            }
        }

        public void PickUp()
        {
            transform.position += new Vector3(0, 4, 0);
            mAnim.SetTrigger("PickUp");
            State = BugState.InAir;
        }

        public void Drop()
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            mAnim.SetTrigger("Drop");
            State = BugState.Normal;
        }

        public void OverDel(bool value)
        {
            if (value && State == BugState.InAir)
                State = BugState.OverDel;
            else if (State == BugState.OverDel)
                State = BugState.InAir;
        }

        public void OverScan(bool value)
        {
            if (value && State == BugState.InAir)
            {
                State = BugState.OverScan;
            }
            else if (State == BugState.OverScan)
            {
                State = BugState.InAir;
            }
        }

        public DNAData OpenScan()
        {
            if (State != BugState.OverScan) return default(DNAData);

            gameObject.layer = 13;

            foreach (Transform child in transform)
            {
                if (child.gameObject.tag == "Bug")
                    child.gameObject.layer = 13;
            }

            State = BugState.Scaned;
            mAnim.SetTrigger("OpenScan");
            transform.position = new Vector3(3, 4, 17);
            return GetDNA();
        }

        public void CloseScan()
        {
            if (State != BugState.Scaned) return;

            gameObject.layer = 9;

            foreach (Transform child in transform)
            {
                if (child.gameObject.tag == "Bug")
                    child.gameObject.layer = 9;
            }

            State = BugState.InAir;
        }

        private void HealthLoss()
        {
            Health -= mHealthLossPerS * Time.deltaTime;
            if (Health <= 0)
                Destroy(gameObject);
        }

        #endregion

        private void OnHornyColliderStay(Collider other)
        {
            if (other.tag == "Horny")
            {
                mNextAim = other.transform.position;
                mAim = true;
                mSeeingOther = true;
            }
        }

        public float TimeFuction(float value)
        {
            if (GeneratorController.DisableTimeFuction) return 0;

            if (!(Time.timeSinceLevelLoad < mFixTime))
            {
                value = (-mTimeM * (Time.timeSinceLevelLoad - mFixTime) + 1) * value;
                if (value <= 0)
                    return 0;
                else
                    return value;
            }
            return value;
        }

        private void OverDel()
        {
            if (State != BugState.OverDel) return;

            transform.Rotate(0, (float)Math.Sin(Time.timeSinceLevelLoad * 80f) * 3f, 0);
        }

        public DNAData GetDNA()
        {
            return new DNAData(mMutationRate, mSightDistance, mSpeed, mTurnMomentum, mBirthHealth, mFoodHealth, mHealthLossPerS, mAChildHealth, mChildHealth, mChildHealthLoss, mBrain.Weigths, mBrain.Bases);
        }
    }*/
}
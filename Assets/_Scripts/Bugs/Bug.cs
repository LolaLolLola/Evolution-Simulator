#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using Assets._Scripts.Bugs.Parts;
using Assets._Scripts.Bugs.Parts.MainPart;
using Assets._Scripts.DNA;
using Assets._Scripts.Enums;
using Assets._Scripts.Enums.Bug;
using Assets._Scripts.Generators;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs
{
    public partial class Bug : MainBugPart
    {
        #region Stats

        private BugStructur mBugStructur;
        private float mChildHealth;
        private float mAChildHealth;
        private float mHealthLoss;

        public float MutationRate { get; private set; }
        public float Speed { get; private set; }
        public float TurnMomentum { get; private set; }
        public float FoodHealth { get; private set; }
        public float SightDistance { get; private set; }

        #endregion

        private EyeInfo mNextTarget;
        private float[] mOutput;
        private Vector3 mPrePos;

        public float Health { get; private set; }

        public async void Load()
        {
            //await GernerateBug(GeneratorController.SafeRandom().Next(1, 3), GeneratorController.SafeRandom().Next(2, 6));
            await GernerateBug(4, 0);
            mBugStructur = GetBugStructur();

            SetDefaults();
        }

        private void SetDefaults()
        {
            MutationRate = 0.7f;
            Speed = 10.0f;
            TurnMomentum = 720.0f;
            FoodHealth = 15.0f;
            SightDistance = 6.0f;
            pBrain = new Brain(true);

            CalculateValues();
        }

        public void Load(BugDNA dna)
        {
            GenerateBug(dna.Stucture);
            mBugStructur = dna.Stucture;

            MutationRate = dna[BugValue.MutationRate];
            Speed = dna[BugValue.Speed];
            TurnMomentum = dna[BugValue.TurnMomentum];
            FoodHealth = dna[BugValue.FoodHealth];
            SightDistance = dna[BugValue.SightDistance];

            pBrain = new Brain(false);
            pBrain.SetNeuronValues(dna.Weights, dna.Bases);

            CalculateValues();
        }

        private void CalculateValues()
        {
            mStateMachin = new StateMachin();

            mHealthLoss = (0.2f * FoodHealth * 0.07f)
                        + (0.4f * Speed * 0.1f)
                        + (0.4f * TurnMomentum * 0.002f)
                        + (0.2f * SightDistance * 0.16f);

            Speed *= (float)Math.Pow(0.98f, mBugStructur.Count);
            TurnMomentum *= (float)Math.Pow(0.98f, mBugStructur.Count);
            mHealthLoss *= (float)Math.Pow(1.01f, mBugStructur.Count);

            mChildHealth = mHealthLoss * 80 * (float)Math.Pow(0.9f, mBugStructur.Count);
            mAChildHealth = mChildHealth * 2 * (float)Math.Pow(0.94f, mBugStructur.Count);

            SightDistance *= (float)Math.Pow(0.96f, Speed);

            pBrain.Speed = Speed;
            Health = 20;

            GetComponent<SphereCollider>().radius = MaxDistance();
            pGetBugDNA = GetBugDNA;

            RegisterStatetransisons();
            mStateMachin.SetToken(BugToken.Start);
        }

        private void Update()
        {
            switch (State)
            {
                case BugState.Normal:
                    GetTarget();
                    Move();
                    HealthLoss();
                    break;
                case BugState.OverDelet:
                    Shake();
                    break;
            }
        }

        private void Move()
        {
            mOutput = mNextTarget.IsBug
                ? pBrain.Output(new float[] { mNextTarget.Bug.x, mNextTarget.Bug.z, mNextTarget.Wall.x, mNextTarget.Wall.z, transform.position.x, transform.position.z })
                : pBrain.Output(new float[] { mNextTarget.Food.x, mNextTarget.Food.z, mNextTarget.Wall.x, mNextTarget.Wall.z, transform.position.x, transform.position.z });

            mPrePos = new Vector3(transform.position.x, 0, transform.position.z);

            transform.Rotate(0, (mOutput[0] > mOutput[1] ? -1 : 1) * TurnMomentum * Time.deltaTime, 0);
            transform.position += transform.right * Speed * Time.deltaTime;

            pBrain.Train(mNextTarget, mPrePos, transform.position);
        }

        private void GetTarget()
        {
            mNextTarget = NextTarget(State, SightDistance);
        }

        public sealed override BugStructur GetBugStructur()
        {
            return mBugStructur ?? base.GetBugStructur();
        }

        private void HealthLoss()
        {
            Health -= mHealthLoss * Time.deltaTime;

            if (Health < 0) mStateMachin.SetToken(BugToken.End);

            if (Health > mChildHealth && State != BugState.Horny) mStateMachin.SetToken(BugToken.MakeHorny);
            if (Health < mChildHealth && State == BugState.Horny) mStateMachin.SetToken(BugToken.LoseHrony);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!OnGround) return;

            if (other.tag == "Food")
                Health += FoodHealth;

            if (State != BugState.Horny) return;

            if (other.tag == "Bug")

        }

        public BugDNA GetBugDNA()
        {
            BugDNA dna = new BugDNA(GetBugStructur(), pBrain.Weights, pBrain.Bases);
            dna.SetValues(MutationRate, Speed, TurnMomentum, FoodHealth, SightDistance);
            return dna;
        }

        public BugData GetBugData()
        {
            return new BugData(Health, FoodHealth, mHealthLoss, GetBugStructur().Count, mChildHealth);
        }
    }
}
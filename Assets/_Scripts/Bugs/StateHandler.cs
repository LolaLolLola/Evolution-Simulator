#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets._Scripts.Enums.Bug;
using UnityEngine;

#endregion

namespace Assets._Scripts.Bugs
{
    public partial class Bug
    {
        private StateMachin mStateMachin;
        public BugState State => mStateMachin.CurrentState;
        public bool OnGround => mStateMachin.OnGround;
        public bool SetTocken(BugToken token) => mStateMachin.SetToken(token);

        private void RegisterStatetransisons()
        {
            mStateMachin = new StateMachin();

            mStateMachin.RegisterState(BugToken.Start, new BugState[] { BugState.Loading }, BugState.Normal, null);
            mStateMachin.RegisterState(BugToken.End, AllBugSate.GetStates, BugState.Loading, delegate { Destroy(gameObject); });
            mStateMachin.RegisterState(BugToken.PickUp, new BugState[] { BugState.Normal, BugState.Horny }, BugState.InAir, PickUp);
            mStateMachin.RegisterState(BugToken.Drop, new BugState[] { BugState.InAir, BugState.OverDelet }, BugState.Normal, Drop);
            mStateMachin.RegisterState(BugToken.EnterDelet, new BugState[] { BugState.InAir }, BugState.OverDelet, null);
            mStateMachin.RegisterState(BugToken.ExitDelet, new BugState[] { BugState.OverDelet }, BugState.InAir, null);
            mStateMachin.RegisterState(BugToken.OpenScan, new BugState[] { BugState.InAir }, BugState.Scaneing, OpenScan);
            mStateMachin.RegisterState(BugToken.CloseScan, new BugState[] { BugState.Scaneing }, BugState.InAir, CloseScan);
            mStateMachin.RegisterState(BugToken.MakeHorny, new BugState[] { BugState.Normal }, BugState.Horny, null);
            mStateMachin.RegisterState(BugToken.LoseHrony, new BugState[] { BugState.Horny }, BugState.Normal, null);
        }

        private class StateMachin
        {
            private Dictionary<BugToken, Action> mTokenActions;
            private Dictionary<BugToken, BugState[]> mTokenValidation;
            private BugToken mCurrentToken;
            
            public BugState CurrentState { get; private set; }
            public bool OnGround { get; private set; }

            public StateMachin()
            {
                mTokenActions = new Dictionary<BugToken, Action>();
                mTokenValidation = new Dictionary<BugToken, BugState[]>();

                CurrentState = BugState.Loading;
            }

            public void RegisterState(BugToken token, BugState[] fromStates, BugState toState, Action @event)
            {
                mTokenValidation.Add(token, fromStates);
                mTokenActions.Add(token, delegate {

                    if (mTokenValidation[mCurrentToken].Contains(CurrentState))
                    {
                        CurrentState = toState;
                        @event?.Invoke();
                    }
                    else
                    {
                        throw new Exception(String.Format("Invalid Change from {0} to {1} with {2}", CurrentState, toState, token));
                    }
                });
            }

            public bool SetToken(BugToken token) 
            {
                try
                {
                    mCurrentToken = token;
                    mTokenActions[token]();

                    OnGround = CurrentState == BugState.Normal || CurrentState == BugState.Horny;

                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    return false;
                }
            }
        }
    }
}

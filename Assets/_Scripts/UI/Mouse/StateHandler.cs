#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets._Scripts.Enums.Bug;
using Assets._Scripts.Enums.Mous;
using UnityEngine;

#endregion

namespace Assets._Scripts.UI.Mous
{
    public partial class MousHandler
    {
        public MousState State => mStateMachin.CurrentState;
        private bool mCanPickUp => State == MousState.Normal;
        private bool mHasBug => State == MousState.PickedUp || State == MousState.OverInfo;

        private void RegisterStates()
        {
            mStateMachin = new StateMachin();

            mStateMachin.RegisterState(MousToken.Pickup, new MousState[] { MousState.Normal }, MousState.PickedUp, delegate { PickUp?.Invoke(); mTarget.SetTocken(BugToken.PickUp); });
            mStateMachin.RegisterState(MousToken.Drop, new MousState[] { MousState.PickedUp }, MousState.Normal, delegate { Drop?.Invoke(); mTarget.SetTocken(BugToken.Drop); });
            mStateMachin.RegisterState(MousToken.DeletBug, new MousState[] { MousState.PickedUp }, MousState.Normal, delegate { mTarget = null; });
            mStateMachin.RegisterState(MousToken.EnterInfo, new MousState[] { MousState.PickedUp }, MousState.OverInfo, null);
            mStateMachin.RegisterState(MousToken.ExitInfo, new MousState[] { MousState.OverInfo }, MousState.PickedUp, null);
            mStateMachin.RegisterState(MousToken.InfoClick, new MousState[] { MousState.OverInfo }, MousState.Scaning, delegate { OpenScan?.Invoke(); mTarget.SetTocken(BugToken.OpenScan); });
            mStateMachin.RegisterState(MousToken.InfoClick, new MousState[] { MousState.Scaning }, MousState.PickedUp, delegate { CloseScan?.Invoke(); mTarget.SetTocken(BugToken.CloseScan); });
        }

        private class StateMachin
        {
            private Dictionary<MousToken, Dictionary<MousState, Action>> mTokenActions;
            private MousToken mCurrentToken;
            private List<MousToken> mLocked;

            public MousState CurrentState { get; private set; }

            public StateMachin()
            {
                mTokenActions = new Dictionary<MousToken, Dictionary<MousState, Action>>();
                mLocked = new List<MousToken>();

                CurrentState = MousState.Normal;
            }

            public void RegisterState(MousToken token, MousState[] fromStates, MousState toState, Action @event)
            {
                if (!mTokenActions.ContainsKey(token))
                    mTokenActions.Add(token, new Dictionary<MousState, Action>());

                foreach (MousState item in fromStates)
                {
                    mTokenActions[token].Add(item, delegate
                    {
                        CurrentState = toState;
                        @event?.Invoke();
                        Lock();
                    });
                }
            }

            private async void Lock()
            {
                if (mCurrentToken == MousToken.InfoClick)
                {
                    mLocked.Add(mCurrentToken);
                    int index = mLocked.IndexOf(mCurrentToken);

                    await Task.Delay(100);
                    mLocked.RemoveAt(index);
                }
            }

            public bool SetToken(MousToken token)
            {
                try
                {
                    mCurrentToken = token;
                    if (mLocked.Contains(token)) return false;

                    mTokenActions[token][CurrentState]();

                    return true;
                }
                catch
                {
                    Debug.Log(String.Format("Invalid Change from {0} with {1}", CurrentState, token));
                    return false;
                }
            }
        }
    }
}
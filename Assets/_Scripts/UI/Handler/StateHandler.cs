#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.Enums.UI;
using UnityEngine;

#endregion

namespace Assets._Scripts.UI.Handler
{
    public partial class UIHandler
    {
        private StateMachin mStateMachin;

        private void RegisterStates()
        {
            mStateMachin = new StateMachin();

            mStateMachin.RegisterState(UIToken.BugMenuOpen, new UIState[] { UIState.Hidden }, UIState.BugMenu, BugMenuOpen);
            mStateMachin.RegisterState(UIToken.BugMenuClose, new UIState[] { UIState.BugMenu }, UIState.Hidden, BugMenuClose);
            mStateMachin.RegisterState(UIToken.ScanUIOpen, new UIState[] { UIState.BugMenu }, UIState.Scan, OpenScan);
            mStateMachin.RegisterState(UIToken.ScanUIClose, new UIState[] { UIState.Scan }, UIState.BugMenu, CloseScan);
        }

        private class StateMachin
        {
            private Dictionary<UIToken, Dictionary<UIState, Action>> mTokenActions;
            private UIToken mCurrentToken;

            public UIState CurrentState { get; private set; }

            public StateMachin()
            {
                mTokenActions = new Dictionary<UIToken, Dictionary<UIState, Action>>();

                CurrentState = UIState.Hidden;
            }

            public void RegisterState(UIToken token, UIState[] fromStates, UIState toState, Action @event)
            {
                if (!mTokenActions.ContainsKey(token))
                    mTokenActions.Add(token, new Dictionary<UIState, Action>());

                foreach (UIState item in fromStates)
                {
                    mTokenActions[token].Add(item, delegate
                    {
                        CurrentState = toState;
                        @event?.Invoke();
                    });
                }
            }

            public bool SetToken(UIToken token)
            {
                try
                {
                    mCurrentToken = token;
                    mTokenActions[token][CurrentState]();

                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log(String.Format("Invalid Change from {0} with {1}", CurrentState, token));
                    return false;
                }
            }
        }
    }
}
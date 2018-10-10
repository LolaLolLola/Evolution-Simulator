#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace Assets._Scripts.Handler
{
    public class AnimationHandler : StateMachineBehaviour
    {
        private static Dictionary<string, AnimationHandler> mHandlers;

        [SerializeField] private string mName;

        public event Action Enter;
        public event Action StopRunning;
        public event Action Exit;

        private void Awake()
        {
            if (mHandlers == null) mHandlers = new Dictionary<string, AnimationHandler>();

            mHandlers.Add(mName, this);
        }

        public static AnimationHandler GetInstance(string name)
        {
            return mHandlers[name];
        }

        public override async void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Enter?.Invoke();

            await Task.Delay((int)(animatorStateInfo.length * 1000));
            StopRunning?.Invoke();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Exit?.Invoke();
        }
    }
}
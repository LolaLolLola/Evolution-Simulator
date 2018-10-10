#pragma warning disable IDE0044 // Add readonly modifier

#region using

using Assets._Scripts.Bugs;
using Assets._Scripts.DNA;
using Assets._Scripts.Enums;
using Assets._Scripts.Enums.Bug;
using Assets._Scripts.Enums.Mous;
using Assets._Scripts.Generators;
using System;
using UnityEngine;

#endregion

namespace Assets._Scripts.UI.Mous
{
    public partial class MousHandler : MonoBehaviour
    {
        private Bug mTarget;
        private StateMachin mStateMachin;

        public event Action PickUp;
        public event Action Drop;
        public event Action OpenScan;
        public event Action CloseScan;

        public Action EnterDelet { get; private set; }
        public Action ExitDelet { get; private set; }
        public Action EnterInfo { get; private set; }
        public Action ExitInfo { get; private set; }
        public Action ClickDelet { get; private set; }
        public Action ClickInfo { get; private set; }

        public BugDNA BugDNA => mTarget.GetBugDNA();
        public BugData BugData => mTarget.GetBugData();

        public void Start()
        {
            RegisterStates();

            EnterDelet = delegate { mTarget.SetTocken(BugToken.EnterDelet); };
            ExitDelet = delegate { mTarget.SetTocken(BugToken.ExitDelet); };
            ClickDelet = delegate { mTarget.SetTocken(BugToken.End); mStateMachin.SetToken(MousToken.DeletBug); };
            EnterInfo = delegate { mStateMachin.SetToken(MousToken.EnterInfo); };
            ExitInfo = delegate { mStateMachin.SetToken(MousToken.ExitInfo); };
            ClickInfo = delegate { mStateMachin.SetToken(MousToken.InfoClick); };
        }

        private void Update()
        {
            MoveBug();

            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

            if (mCanPickUp)
            {
                mTarget = GetBug();

                if (mTarget != null)
                {
                    mStateMachin.SetToken(MousToken.Pickup);
                }
            }
            else if (mHasBug)
            {
                mStateMachin.SetToken(MousToken.Drop);
            }
        }

        private void MoveBug()
        {
            if (!mHasBug) return;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mTarget.transform.position = new Vector3(pos.x, 4, pos.z);
        }

        private Bug GetBug()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit = new RaycastHit();

            return Physics.Raycast(ray, out rayhit, 40, GeneratorController.BugLayerMask)
                ? rayhit.collider.gameObject.GetComponent<Bug>()
                : null;
        }
    }
}
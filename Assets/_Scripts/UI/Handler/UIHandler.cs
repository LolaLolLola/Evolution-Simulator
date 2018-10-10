#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using UnityEngine;
using UnityEngine.UI;
using Assets._Scripts.UI;
using Assets._Scripts.DNA;
using System.Collections;
using System.Threading.Tasks;
using Assets._Scripts.Enums;
using Assets._Scripts.UI.Mous;
using Assets._Scripts.Enums.UI;
using Assets._Scripts.Enums.Bug;
using Assets._Scripts.Handler;

#endregion

namespace Assets._Scripts.UI.Handler
{
    [Serializable]
    public class BugMenu
    {
        public GameObject Delet;
        public GameObject Info;
        public GameObject ScanPanle;
    }

    [Serializable]
    public class InfoText
    {
        public Text MutatioRate;
        public Text SightDistance;
        public Text Speed;
        public Text TurnMomentum;
        public Text Parts;
        public Text HealthPerFood;
        public Text ChildCap;
        public Text ChildHealthLoss;
        public Text Health;
        public Text HealthLossPerS;
    }

    public partial class UIHandler : MonoBehaviour
    {
        [SerializeField] private Camera mCamera;
        [SerializeField] private MousHandler mMousHandler;
        [SerializeField] private BugMenu mBugMenu;
        [SerializeField] private InfoText mInfos;
        private Animator mDeletAnimator;
        private Animator mInfoAnimator;
        private Animator mScanPanelAnimator;

        private void Start()
        {
            mMousHandler.PickUp += delegate { mStateMachin.SetToken(UIToken.BugMenuOpen); };
            mMousHandler.Drop += delegate { mStateMachin.SetToken(UIToken.BugMenuClose); };
            mMousHandler.OpenScan += delegate { mStateMachin.SetToken(UIToken.ScanUIOpen); };
            mMousHandler.CloseScan += delegate { mStateMachin.SetToken(UIToken.ScanUIClose); };

            mDeletAnimator = mBugMenu.Delet.GetComponent<Animator>();
            mInfoAnimator = mBugMenu.Info.GetComponent<Animator>();
            mScanPanelAnimator = mBugMenu.ScanPanle.GetComponent<Animator>();

            EnterDetector d = mBugMenu.Delet.GetComponent<EnterDetector>();
            ClickDetector c = mBugMenu.Delet.GetComponent<ClickDetector>();
            d.Enter += mMousHandler.EnterDelet;
            d.Exit += mMousHandler.ExitDelet;
            c.Click += mMousHandler.ClickDelet;

            d = mBugMenu.Info.GetComponent<EnterDetector>();
            c = mBugMenu.Info.GetComponent<ClickDetector>();
            d.Enter += mMousHandler.EnterInfo;
            d.Exit += mMousHandler.ExitInfo;
            c.Click += mMousHandler.ClickInfo;

            RegisterStates();
        }

        private void BugMenuClose()
        {
            mDeletAnimator.SetTrigger("Go");
            mInfoAnimator.SetTrigger("Go");
        }

        private void BugMenuOpen()
        {
            mDeletAnimator.SetTrigger("Come");
            mInfoAnimator.SetTrigger("Come");
        }

        private void OpenScan()
        {
            mCamera.depth = 0;

            SetInfos(mMousHandler.BugDNA, mMousHandler.BugData);

            mInfoAnimator.SetTrigger("Open");
            mScanPanelAnimator.SetTrigger("Open");
        }

        private void CloseScan()
        {
            mCamera.depth = -2;

            mInfoAnimator.SetTrigger("Close");
            mScanPanelAnimator.SetTrigger("Close");
        }
        
        private void SetInfos(BugDNA dna, BugData data)
        {
            mInfos.Health.text = Math.Round(data.Health, 2).ToString();
            mInfos.HealthLossPerS.text = Math.Round(data.HealthLoss, 2).ToString();
            mInfos.ChildCap.text = Math.Round(data.ChildHealth, 2).ToString();
            mInfos.HealthPerFood.text = Math.Round(data.FoodHealth, 2).ToString();
            mInfos.MutatioRate.text = Math.Round(dna[BugValue.MutationRate], 2).ToString();
            mInfos.Parts.text = data.Parts.ToString();
            mInfos.SightDistance.text = Math.Round(dna[BugValue.SightDistance], 2).ToString();
            mInfos.Speed.text = Math.Round(dna[BugValue.Speed], 2).ToString();
            mInfos.TurnMomentum.text = Math.Round(dna[BugValue.TurnMomentum], 2).ToString();
        }
    }
}
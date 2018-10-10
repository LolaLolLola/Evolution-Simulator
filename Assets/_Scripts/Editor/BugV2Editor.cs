#pragma warning disable IDE0044 // Add readonly modifier

#region using

using System;
using Assets._Scripts.Bugs;
using Assets._Scripts.Bugs.Parts.MainPart;
using UnityEditor;
using UnityEngine;

#endregion

namespace Assets._Scripts.CustumEditor
{
    [CustomEditor(typeof(BugTester))]
    public class BugV2Editor : Editor
    {
        private static BugStructur mBS;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorGUILayout.DropdownButton(new GUIContent("Load"), FocusType.Passive))
            {
                Bug b = ((BugTester)target).MakeBug();
                b.Load();
                mBS = b.GetBugStructur();
            }

            if (EditorGUILayout.DropdownButton(new GUIContent("Make Copy of Last"), FocusType.Passive))
            {
                Bug b = ((BugTester)target).MakeBug();
                //b.Load(mBS);
            }
        }
    }
}
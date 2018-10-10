#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0414 // Not used value
//#define DebugLR

#region using

using Assets._Scripts.Bugs;
using UnityEditor;
using UnityEngine;

#endregion

/*
namespace Assets._Scripts.CustumEditor
{
    [CustomEditor(typeof(Bug))]
    public class BugEditor : Editor
    {
        private Bug mTarget;

        private SerializedProperty mNextAim;
        private SerializedProperty mNextWall;
#if DebugLR
        private SerializedProperty mLRFood;
        private SerializedProperty mLRWall;
        private SerializedProperty mLRWalk;
#endif

        private void OnEnable()
        {
            mTarget = (Bug)target;

            mNextAim = serializedObject.FindProperty("mNextAim");
            mNextWall = serializedObject.FindProperty("mNextWall");
#if DebugLR
            mLRFood = serializedObject.FindProperty("mLerningrateFood");
            mLRWalk = serializedObject.FindProperty("mLerningrateWalk");
            mLRWall = serializedObject.FindProperty("mLerningrateWall");
#endif
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!EditorApplication.isPlaying)
            {
                base.OnInspectorGUI();
            }
            else
            {
                EditorGUILayout.Vector3Field(new GUIContent("Next Aim"), mNextAim.vector3Value);
                EditorGUILayout.Vector3Field(new GUIContent("Next Wall"), mNextWall.vector3Value);
                //EditorGUILayout.FloatField(new GUIContent("Health"), mTarget.Health);
#if DebugLR
                EditorGUILayout.FloatField(new GUIContent("Leraning rate Food"), mTarget.TimeFuction(mLRFood.floatValue));
                EditorGUILayout.FloatField(new GUIContent("Leraning rate Wall"), mTarget.TimeFuction(mLRWall.floatValue));
                EditorGUILayout.FloatField(new GUIContent("Leraning rate Walk"), mTarget.TimeFuction(mLRWalk.floatValue));
#endif
            }
        }
    }
}

    */
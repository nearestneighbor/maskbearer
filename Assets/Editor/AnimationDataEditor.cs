 using System.Collections;
 using System.Collections.Generic;
 using UnityEditor;
 using UnityEngine;
 
 [CustomEditor(typeof(AnimationData))]
 public class AnimationDataEditor : Editor
 {
     public override void OnInspectorGUI()
     {
         base.OnInspectorGUI();
         AnimationData data = (AnimationData)target;
 
             if(GUILayout.Button("Load", GUILayout.Height(40)))
             {
                 data.Load();
             }
         
     }
 }
  
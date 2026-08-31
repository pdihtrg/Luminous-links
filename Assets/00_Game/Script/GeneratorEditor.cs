using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    
    public override void OnInspectorGUI(){
        DrawDefaultInspector(); // Hiển thị các biến mặc định trong Inspector

        Generator gameManager = (Generator)target;

        if (GUILayout.Button("Save Map"))
        {
            gameManager.SaveData();
        }
    }
    
}
#endif
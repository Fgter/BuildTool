using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomScriptingDefineConfig))]
public class MyScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CustomScriptingDefineConfig myObject = (CustomScriptingDefineConfig)target;

        GUILayout.Space(10);
        if (GUILayout.Button("编译脚本", GUILayout.Height(30)))
        {
            myObject.Compile();
        }
    }
}
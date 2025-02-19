using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(BlurPanel))]
public class BlurPanelEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animate"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("time"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}

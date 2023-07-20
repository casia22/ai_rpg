using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class General
{
    public static System.Random random = new System.Random();
    public static float CaDist2(Transform orin, Transform target)
    {
        return (orin.position - target.position).x + (orin.position - target.position).y;
    }
}

public class DisplayOnly : PropertyAttribute
{

}
[CustomPropertyDrawer(typeof(DisplayOnly))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(RadarChart))]
public class RadarChartEditor : Editor
{
    private SerializedProperty color, raycast, material;
    private SerializedProperty percents, radius, startDegree, sideCnt, doSet, realTime;

    void OnEnable()
    {
        percents = serializedObject.FindProperty("m_Percents");
        radius = serializedObject.FindProperty("m_Radius");
        startDegree = serializedObject.FindProperty("m_StartDegree");
        sideCnt = serializedObject.FindProperty("m_SideCnt");
        doSet = serializedObject.FindProperty("m_DoSet");
        realTime = serializedObject.FindProperty("m_RealTime");

        color = serializedObject.FindProperty("m_Color");
        raycast = serializedObject.FindProperty("m_RaycastTarget");
        material = serializedObject.FindProperty("m_Material");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(sideCnt, new GUIContent("边数"));
        EditorGUILayout.PropertyField(startDegree, new GUIContent("开始偏移角度"));
        EditorGUILayout.PropertyField(doSet, new GUIContent("重置"));
        EditorGUILayout.PropertyField(realTime, new GUIContent("实时绘制"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(raycast);
        EditorGUILayout.PropertyField(material);
        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(radius, new GUIContent("半径"));

        EditorGUILayout.Space();

        int ss = percents.arraySize;
        for (int i = 0; i < ss; i++)
        {
            SerializedProperty p = percents.GetArrayElementAtIndex(i);
            EditorGUILayout.Slider(p, 0, 1, new GUIContent("百分比_" + i));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
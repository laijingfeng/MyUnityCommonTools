using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AnimationCurveHelper))]
public class AnimationCurveHelperEditor : Editor
{
    protected AnimationCurveHelper info;

    public override void OnInspectorGUI()
    {
        info = target as AnimationCurveHelper;

        info.m_AnimClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("TarAnimationClip"), info.m_AnimClip, typeof(AnimationClip), true);
        info.m_CurveName = EditorGUILayout.TextField(new GUIContent("CurveName"), info.m_CurveName);

        info.m_LoadDataFromCurve = EditorGUILayout.Toggle(new GUIContent("LoadDataFromCurve"), info.m_LoadDataFromCurve);
        if (info.m_LoadDataFromCurve)
        {
            info.m_ExtraAnimClip = (AnimationClip)EditorGUILayout.ObjectField(new GUIContent("DataAnimationClip"), info.m_ExtraAnimClip, typeof(AnimationClip), true);
            if (info.m_ExtraAnimClip != null)
            {
                info.UpdateExtra();
                info._SelectedCurveIndex = EditorGUILayout.Popup("Node", info._SelectedCurveIndex, info._CurveNames);
                if (info._Curves != null
                    && info._SelectedCurveIndex >= 0 
                    && info._SelectedCurveIndex < info._Curves.Length)
                {
                    info.m_AnimCurve = AnimationUtility.GetEditorCurve(info.m_ExtraAnimClip, info._Curves[info._SelectedCurveIndex]);
                    info.m_AnimCurve = EditorGUILayout.CurveField(new GUIContent("CurCurve"), info.m_AnimCurve);
                }
            }
        }
        else
        {
            info.m_CurveData = EditorGUILayout.TextField(new GUIContent("CurveData"), info.m_CurveData);
            if(!string.IsNullOrEmpty(info.m_CurveData))
            {
                info.m_AnimCurve = info.CreateCurve();
                info.m_AnimCurve = EditorGUILayout.CurveField(new GUIContent("CurCurve"), info.m_AnimCurve);
            }
        }
    }
}

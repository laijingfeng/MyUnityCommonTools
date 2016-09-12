#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class AnimationCurveHelper : MonoBehaviour
{
    public AnimationClip m_AnimClip;
    public string m_CurveName;
    public string m_CurveData;
    public AnimationClip m_ExtraAnimClip;
    public AnimationCurve m_AnimCurve = new AnimationCurve();
    public bool m_LoadDataFromCurve;

    void Start()
    {
    }

    void Update()
    {
        UpdateExtra();
    }

    #region DoExtra

    private string[] _CurveNames;
    private int _SelectedCurveIndex = 1;

    private EditorCurveBinding[] _Curves;

    private void UpdateExtra()
    {
        if (m_ExtraAnimClip == null)
        {
            return;
        }

        _Curves = AnimationUtility.GetCurveBindings(m_ExtraAnimClip);
        _SelectedCurveIndex = 0;
        int curveCount = _Curves.Length;
        _CurveNames = new string[curveCount];
        for (int i = 0; i < curveCount; ++i)
        {
            if (_Curves[i].path == "")
            {
                _CurveNames[i] = _Curves[i].propertyName;
            }
            else
            {
                _CurveNames[i] = _Curves[i].path + "/" + _Curves[i].propertyName;
            }
        }

        AnimationUtility.GetEditorCurve(m_ExtraAnimClip, _Curves[_SelectedCurveIndex]);

        //AnimationCurveExtractor aceWindow = AnimationCurveExtractor.GetWindow(typeof(AnimationCurveExtractor)) as AnimationCurveExtractor;
        //aceWindow.Init(_PopupTargetAnimationCurveProperty);
    }

    #endregion DoExtra

    #region GetCurve

    private AnimationCurve GetCurve()
    {
        return m_AnimCurve;
    }

    private AnimationCurve CreateCurve()
    {
        if (m_LoadDataFromCurve)
        {
            return GetCurve();
        }

        string[] datas = Util.StringToTArray<string>(m_CurveData, ';');
        if (datas == null)
        {
            Debug.LogError("error");
            return null;
        }

        Debug.Log("cnt=" + datas.Length);

        Keyframe[] ks = new Keyframe[datas.Length];

        for (int i = 0, imax = datas.Length; i < imax; i++)
        {
            float[] dd = Util.StringToTArray<float>(datas[i], ',');
            if (dd == null || dd.Length != 2)
            {
                Debug.LogError("error");
                return null;
            }
            ks[i] = new Keyframe(dd[0], dd[1]);
        }

        //for (int i = 0, imax = datas.Length; i < imax; i++)
        //{
        //    if (i == 0)
        //    {
        //        ks[i].inTangent = 0f;
        //    }
        //    else
        //    {
        //        ks[i].inTangent = 45f;
        //    }

        //    if (i == imax - 1)
        //    {
        //        ks[i].outTangent = 0f;
        //    }
        //    else
        //    {
        //        ks[i].inTangent = 45f;
        //    }
        //}

        AnimationCurve curve = new AnimationCurve(ks);
        curve.preWrapMode = WrapMode.Loop;
        curve.postWrapMode = WrapMode.Loop;

        return curve;
    }

    #endregion GetCurve

    [ContextMenu("==Work==")]
    private void Work()
    {
        if (m_AnimClip == null)
        {
            Debug.LogError("AnimClip不能为空");
            return;
        }

        if (m_LoadDataFromCurve == false && string.IsNullOrEmpty(m_CurveData))
        {
            Debug.LogError("Data不能为空");
            return;
        }

        if (string.IsNullOrEmpty(m_CurveName))
        {
            Debug.LogError("CurveName不能为空");
            return;
        }

        string path = AssetDatabase.GetAssetPath(m_AnimClip);
        ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;

        if (mi.animationType != ModelImporterAnimationType.Human)
        {
            return;
        }

        SerializedObject so = new SerializedObject(mi);
        SerializedProperty clips = so.FindProperty("m_ClipAnimations");
        for (int i = 0; i < clips.arraySize; i++)
        {
            if (clips.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue.Equals(m_AnimClip.name))
            {
                SerializedProperty curves = clips.GetArrayElementAtIndex(i).FindPropertyRelative("curves");
                if (curves != null)
                {
                    curves.arraySize += 1;
                    SerializedProperty curinfo = curves.GetArrayElementAtIndex(curves.arraySize - 1);
                    if (curinfo != null)
                    {
                        curinfo.FindPropertyRelative("name").stringValue = m_CurveName;
                        curinfo.FindPropertyRelative("curve").animationCurveValue = CreateCurve();
                    }
                }
                break;
            }
        }
        so.ApplyModifiedProperties();
        AssetDatabase.ImportAsset(path);

        Debug.Log(m_AnimClip.name + " is ok, 如有旧的curve，请手动删除");
    }
}
#endif
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

    #region DoExtra

    public string[] _CurveNames;
    public int _SelectedCurveIndex = 0;
    public EditorCurveBinding[] _Curves;
    private string m_LastPath = "";
    private string m_CurPath = "";

    public void UpdateExtra()
    {
        if (m_ExtraAnimClip == null)
        {
            return;
        }

        m_CurPath = AssetDatabase.GetAssetPath(m_ExtraAnimClip);
        if (m_CurPath.Equals(m_LastPath))
        {
            return;
        }
        m_LastPath = m_CurPath;

        _Curves = AnimationUtility.GetCurveBindings(m_ExtraAnimClip);
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

        for (int i = 0; i < _CurveNames.Length; i++)
        {
            if (_CurveNames[i].Equals(m_CurveName))
            {
                _SelectedCurveIndex = i;
                break;
            }
        }
    }

    #endregion DoExtra

    #region GetCurve

    public class KeyData
    {
        public float time;
        public float val;
        public float inTangent;
        public float outTangent;
        public int tangentMode;
    }

    private AnimationCurve GetCurve(bool handle = false)
    {
        if (handle)
        {
            float s = 1.0f;

            if (m_AnimClip != null)
            {
                s = 1.0f / m_AnimClip.length;
            }

            float left = m_AnimCurve.keys[0].time;
            float right = m_AnimCurve.keys[m_AnimCurve.length - 1].time;
            float minVal = 100f, min_tmp;
            for (float ii = left; ii <= right; ii += 0.001f)
            {
                min_tmp = m_AnimCurve.Evaluate(ii);
                if (min_tmp < minVal)
                {
                    minVal = min_tmp;
                }
            }

            List<KeyData> data = new List<KeyData>();
            for (int i = 0, imax = m_AnimCurve.length; i < imax; i++)
            {
                data.Add(new KeyData()
                {
                    time = m_AnimCurve.keys[i].time * s,
                    val = m_AnimCurve.keys[i].value - minVal,
                    inTangent = m_AnimCurve.keys[i].inTangent / s,
                    outTangent = m_AnimCurve.keys[i].outTangent / s,
                    tangentMode = m_AnimCurve.keys[i].tangentMode,
                });
            }

            if (Mathf.Approximately(data[0].time, 0f) == false)
            {
                data.Insert(0, new KeyData()
                {
                    time = 0f,
                    val = 0.5f * (data[0].val + data[data.Count - 1].val),
                });
            }

            if (Mathf.Approximately(data[data.Count - 1].time, 1f) == false)
            {
                data.Add(new KeyData()
                {
                    time = 1f,
                    val = 0.5f * (data[0].val + data[data.Count - 1].val),
                });
            }

            return CreateCurveByData(data);
        }
        return m_AnimCurve;
    }

    public AnimationCurve CreateCurveByData(List<KeyData> data)
    {
        if (data == null)
        {
            Debug.LogError("error");
            return null;
        }

        Keyframe[] ks = new Keyframe[data.Count];

        for (int i = 0, imax = data.Count; i < imax; i++)
        {
            ks[i] = new Keyframe(data[i].time, data[i].val);
            ks[i].tangentMode = data[i].tangentMode;
            ks[i].inTangent = data[i].inTangent;
            ks[i].outTangent = data[i].outTangent;
        }

        AnimationCurve curve = new AnimationCurve(ks);
        curve.preWrapMode = WrapMode.Loop;
        curve.postWrapMode = WrapMode.Loop;

        return curve;
    }

    public AnimationCurve CreateCurve(bool handle = false)
    {
        if (m_LoadDataFromCurve)
        {
            return GetCurve(handle);
        }
        else
        {
            return CreateCurveByData(JsonUtility.FromJson<List<KeyData>>(m_CurveData));
        }
    }

    #endregion GetCurve

    [ContextMenu("==Refresh==")]
    private void DoRefresh()
    {
        m_LastPath = string.Empty;
    }

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

        SerializedProperty curinfo;
        for (int i = 0; i < clips.arraySize; i++)
        {
            if (clips.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue.Equals(m_AnimClip.name))
            {
                SerializedProperty curves = clips.GetArrayElementAtIndex(i).FindPropertyRelative("curves");
                if (curves != null)
                {
                    //delete old
                    for (int j = 0; j < curves.arraySize; j++)
                    {
                        curinfo = curves.GetArrayElementAtIndex(j);
                        if (curinfo != null && curinfo.FindPropertyRelative("name").stringValue.Equals(m_CurveName.ToString()))
                        {
                            curves.DeleteArrayElementAtIndex(j);
                            so.ApplyModifiedProperties();
                            break;
                        }
                    }

                    curves.arraySize += 1;
                    curinfo = curves.GetArrayElementAtIndex(curves.arraySize - 1);
                    if (curinfo != null)
                    {
                        curinfo.FindPropertyRelative("name").stringValue = m_CurveName;
                        curinfo.FindPropertyRelative("curve").animationCurveValue = GetCurve(true);
                    }
                }
                break;
            }
        }
        so.ApplyModifiedProperties();
        AssetDatabase.ImportAsset(path);

        Debug.Log(m_AnimClip.name + " is ok");
    }
}
#endif
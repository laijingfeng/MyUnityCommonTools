using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 工具
/// </summary>
public class Util
{
    #region 查找

    /// <summary>
    /// 查找
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static GameObject FindGo<T>(GameObject parent, string name, bool includeInactive = true) where T : Component
    {
        if (parent == null)
        {
            return null;
        }

        T t = FindCo<T>(parent.transform, name, includeInactive);

        return (t == null) ? null : t.gameObject;
    }

    /// <summary>
    /// 查找
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static GameObject FindGo(GameObject parent, string name, bool includeInactive = true)
    {
        if (parent == null)
        {
            return null;
        }

        Transform t = FindCo<Transform>(parent.transform, name, includeInactive);

        return (t == null) ? null : t.gameObject;
    }

    /// <summary>
    /// 查找
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static T FindCo<T>(GameObject parent, string name, bool includeInactive = true) where T : Component
    {
        if (parent == null)
        {
            return null;
        }

        return FindCo<T>(parent.transform, name, includeInactive);
    }

    /// <summary>
    /// 查找子结点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static T FindCo<T>(Transform parent, string name, bool includeInactive = true) where T : Component
    {
        if (parent == null)
        {
            return null;
        }

        foreach (T t in parent.GetComponentsInChildren<T>(includeInactive))
        {
            if (t.name.Equals(name))
            {
                return t;
            }
        }

        return null;
    }

    #endregion

    #region 删除

    /// <summary>
    /// 删除所有儿子结点
    /// </summary>
    /// <param name="go"></param>
    public static void DestroyAllChildren(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        List<GameObject> list = new List<GameObject>();

        for (int i = 0, imax = go.transform.childCount; i < imax; i++)
        {
            list.Add(go.transform.GetChild(i).gameObject);
        }

        foreach (GameObject g in list)
        {
            UnityEngine.Object.Destroy(g);
        }
        list.Clear();
    }

    /// <summary>
    /// 删除所有儿子结点
    /// </summary>
    /// <param name="comp"></param>
    public static void DestroyAllChildren(Component comp)
    {
        if (comp == null)
        {
            return;
        }
        DestroyAllChildren(comp.gameObject);
    }

    #endregion 删除

    #region 坐标转化

    /// <summary>
    /// <para>计算UI相对父节点的偏移量</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <param name="child"></param>
    /// <returns>z轴为0</returns>
    public static Vector3 CalUIPosRelateToCanvas(Transform child, bool includeSelf = false)
    {
        Vector2 ret = Vector2.zero;
        if (child == null)
        {
            return ret;
        }
        if (includeSelf)
        {
            ret += new Vector2(child.localPosition.x, child.localPosition.y);
        }
        while (child.parent != null)
        {
            child = child.parent;
            if (child.GetComponent<Canvas>() != null)
            {
                break;
            }
            ret += new Vector2(child.localPosition.x, child.localPosition.y);
        }
        return ret;
    }

    /// <summary>
    /// <para>World->Canvas</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <param name="pos">世界位置</param>
    /// <param name="canvas">UI的Canvas</param>
    /// <param name="tf">使用结果的UI结点，空则是相对Canvas</param>
    /// <returns></returns>
    public static Vector3 PosWorld2Canvas(Vector3 pos, Canvas canvas, Transform tf = null)
    {
        Vector3 ret = Vector3.zero;
        if (canvas == null)
        {
            return ret;
        }

        ret = Camera.main.WorldToScreenPoint(pos);
        ret = Util.PosScreen2Canvas(canvas, ret, tf);

        return ret;
    }

    /// <summary>
    /// <para>Canvas->Screen</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="tf"></param>
    /// <returns></returns>
    public static Vector3 PosCanvas2Screen(Canvas canvas, Transform tf)
    {
        Vector2 ret = Vector2.zero;
        if (canvas == null ||
            tf == null)
        {
            return ret;
        }

        Vector2 pos = Util.CalUIPosRelateToCanvas(tf, true);
        RectTransform canvasRect = canvas.transform as RectTransform;
        pos += canvasRect.sizeDelta * 0.5f;
        pos = new Vector2(pos.x / canvasRect.sizeDelta.x, pos.y / canvasRect.sizeDelta.y);
        ret = new Vector2(Screen.width * pos.x, Screen.height * pos.y);
        return ret;
    }

    /// <summary>
    /// <para>Canvas->Screen</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="pos">点</param>
    /// <returns></returns>
    public static Vector3 PosCanvas2Screen(Canvas canvas, Vector3 pos)
    {
        Vector2 ret = Vector2.zero;
        if (canvas == null)
        {
            return ret;
        }

        Vector2 pos2 = pos;

        RectTransform canvasRect = canvas.transform as RectTransform;
        pos2 += canvasRect.sizeDelta * 0.5f;
        pos2 = new Vector2(pos2.x / canvasRect.sizeDelta.x, pos2.y / canvasRect.sizeDelta.y);
        ret = new Vector2(Screen.width * pos2.x, Screen.height * pos2.y);
        return ret;
    }

    /// <summary>
    /// <para>ScreenMouse->Canvas</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <param name="canvas">Canvas</param>
    /// <param name="tf">使用结果的UI结点，空则是相对Canvas</param>
    /// <returns></returns>
    public static Vector3 PosMouse2Canvas(Canvas canvas, Transform tf = null)
    {
        return Util.PosScreen2Canvas(canvas, Util.GetClickPos(), tf);
    }

    /// <summary>
    /// <para>Screen->Canvas</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <param name="canvas">Canvas</param>
    /// <param name="pos">Screen Position</param>
    /// <param name="tf">使用结果的UI结点，空则是相对Canvas</param>
    /// <returns></returns>
    public static Vector3 PosScreen2Canvas(Canvas canvas, Vector3 pos, Transform tf = null)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 viewportPos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
        Vector3 ret = new Vector2(viewportPos.x * canvasRect.sizeDelta.x, viewportPos.y * canvasRect.sizeDelta.y) - canvasRect.sizeDelta * 0.5f;

        Vector3 relate = Vector3.zero;
        if (tf != null)
        {
            relate = Util.CalUIPosRelateToCanvas(tf, false);
        }

        ret = ret - relate;

        return ret;
    }

    #endregion 坐标转化

    #region 时间转化

    public static double DateTime2Timestamp(System.DateTime t)
    {
        return t.Subtract(new System.DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
    }

    #endregion 时间转化

    /// <summary>
    /// <para>获得点击位置</para>
    /// <para>移动设备用第一个触摸点</para>
    /// <para>返回值z轴为0</para>
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetClickPos()
    {
        Vector3 pos = Input.mousePosition;
#if UNITY_EDITOR
        pos = Input.mousePosition;
#else
#if UNITY_ANDROID || UNITY_IPHONE
            if(Input.touchCount > 0)
            {
                pos = Input.touches[0].position;
            }
            else
            {
                pos = Input.mousePosition;
            }
#else
            pos = Input.mousePosition;
#endif
#endif
        pos.z = 0;
        return pos;
    }

    /// <summary>
    /// <para>StringToIntArray</para>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int[] StringToIntArray(string str, char separator = ',')
    {
        List<int> list = new List<int>();

        if (string.IsNullOrEmpty(str))
        {
            return list.ToArray();
        }

        int tmp;

        string[] str_array = str.Split(separator);
        foreach (string s in str_array)
        {
            if (int.TryParse(s, out tmp) == false)
            {
                return list.ToArray();
            }
            list.Add(tmp);
        }

        return list.ToArray();
    }
}

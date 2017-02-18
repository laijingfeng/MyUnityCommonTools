using UnityEngine;
using System.Collections.Generic;
using System;

namespace Jerry
{
    public class JerryUtil
    {
        #region 克隆对象

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GameObject CloneGo(CloneGoData data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.parant != null && data.clean)
            {
                DestroyAllChildren(data.parant);
            }

            GameObject go = null;
            if (data.prefab == null)
            {
                go = new GameObject();
            }
            else
            {
                go = GameObject.Instantiate(data.prefab) as GameObject;
            }
            
            go.SetActive(data.active);
            if (string.IsNullOrEmpty(data.name) == false)
            {
                go.name = data.name;
            }
            if (data.parant != null)
            {
                go.transform.SetParent(data.parant);
            }
            go.transform.localScale = Vector3.one * data.scale;

            if (data.useOrignal && data.prefab != null)
            {
                go.transform.localPosition = data.prefab.transform.localPosition;
                go.transform.localEulerAngles = data.prefab.transform.localEulerAngles;
            }
            else
            {
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
            }
            
            if (data.isStretchUI)
            {
                (go.transform as RectTransform).offsetMin = Vector2.zero;
                (go.transform as RectTransform).offsetMax = Vector2.zero;
            }
            return go;
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T CloneGo<T>(CloneGoData data) where T : MonoBehaviour
        {
            GameObject go = CloneGo(data);
            if (go == null)
            {
                return null;
            }
            return go.AddComponent<T>();
        }

        public class CloneGoData
        {
            /// <summary>
            /// 预设
            /// </summary>
            public GameObject prefab = null;
            /// <summary>
            /// 父节点，空则在外部
            /// </summary>
            public Transform parant = null;
            /// <summary>
            /// 名称，空则用默认
            /// </summary>
            public string name = null;
            /// <summary>
            /// 是否是要Stretch的UI
            /// </summary>
            public bool isStretchUI = false;
            /// <summary>
            /// 缩放系数
            /// </summary>
            public float scale = 1f;
            /// <summary>
            /// 清理父节点
            /// </summary>
            public bool clean = false;
            /// <summary>
            /// 使用原始位置信息
            /// </summary>
            public bool useOrignal = false;
            /// <summary>
            /// 激活
            /// </summary>
            public bool active = false;
        }

        #endregion 克隆对象

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
            ret = JerryUtil.PosScreen2Canvas(canvas, ret, tf);

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
            if (canvas == null ||
                tf == null)
            {
                return Vector2.zero;
            }

            Vector2 pos = JerryUtil.CalUIPosRelateToCanvas(tf, true);
            return PosCanvas2Screen(canvas, pos);
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
            return JerryUtil.PosScreen2Canvas(canvas, JerryUtil.GetClickPos(), tf);
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
                relate = JerryUtil.CalUIPosRelateToCanvas(tf, false);
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

        public static System.DateTime Timestamp2DateTime(double timestamp)
        {
            return (new System.DateTime(1970, 1, 1).ToLocalTime()).AddSeconds(timestamp);
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

        #region 数值转化

        /// <summary>
        /// <para>StringToTArray</para>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T[] String2TArray<T>(string str, char separator = ',')
        {
            List<T> list = new List<T>();

            if (string.IsNullOrEmpty(str))
            {
                return list.ToArray();
            }

            T tmp = default(T);

            string[] str_array = str.Split(separator);
            foreach (string s in str_array)
            {
                try
                {
                    tmp = (T)Convert.ChangeType(s, typeof(T));
                }
                catch (Exception ex)
                {
                    Debug.LogError(string.Format("StringToTArray error {0} : cant not change {1} to {2}", ex.Message, s, typeof(T)));
                    continue;
                }
                list.Add(tmp);
            }

            return list.ToArray();
        }

        /// <summary>
        /// TArrayToString
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TArray2String<T>(T[] data, char separator = ',')
        {
            string strData = "";
            string sepStr = separator.ToString();
            bool first = true;
            foreach (T t in data)
            {
                strData += string.Format("{0}{1}", first ? "" : sepStr, t);
                first = false;
            }
            return strData;
        }

        #endregion 数值转化

        #region LayerMask处理

        /// <summary>
        /// <para>直接设置LayerMask为Everything或Nothing</para>
        /// <para>true:Everything</para>
        /// <para>false:Nothing</para>
        /// </summary>
        /// <param name="everythingOrNothing"></param>
        /// <returns></returns>
        public static int MakeLayerMask(bool everythingOrNothing)
        {
            if (everythingOrNothing)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 通过id构造LayerMask
        /// </summary>
        /// <param name="ids">id组</param>
        /// <param name="invert">是否反转</param>
        /// <returns></returns>
        public static int MakeLayerMask(int[] ids, bool invert = false)
        {
            if (ids == null)
            {
                return 0;
            }

            int ret = 0;
            foreach (int id in ids)
            {
                ret |= (1 << id);
            }
            if (invert)
            {
                ret = ~ret;
            }
            return ret;
        }

        /// <summary>
        /// 通过name构造LayerMask
        /// </summary>
        /// <param name="names">名称组</param>
        /// <param name="invert">是否反转</param>
        /// <returns></returns>
        public static int MakeLayerMask(string[] names, bool invert = false)
        {
            if (names == null)
            {
                return 0;
            }
            int[] ids = new int[names.Length];
            int idx = 0;
            foreach (string name in names)
            {
                ids[idx++] = LayerMask.NameToLayer(name);
            }
            return MakeLayerMask(ids, invert);
        }

        /// <summary>
        /// 通过name构造LayerMask
        /// </summary>
        /// <param name="oldLayerMask">旧的layerMask</param>
        /// <param name="addNames">增加的</param>
        /// <param name="subNames">减去的</param>
        /// <returns></returns>
        public static int MakeLayerMask(int oldLayerMask, string[] addNames = null, string[] subNames = null)
        {
            int[] addIds = null;
            if (addNames != null)
            {
                addIds = new int[addNames.Length];
                int idx = 0;
                foreach (string name in addNames)
                {
                    addIds[idx++] = LayerMask.NameToLayer(name);
                }
            }
            int[] subIds = null;
            if (subNames != null)
            {
                subIds = new int[subNames.Length];
                int idx = 0;
                foreach (string name in subNames)
                {
                    subIds[idx++] = LayerMask.NameToLayer(name);
                }
            }
            return MakeLayerMask(oldLayerMask, addIds, subIds);
        }

        /// <summary>
        /// 通过id构造LayerMask
        /// </summary>
        /// <param name="oldLayerMask">旧的layerMask</param>
        /// <param name="addNames">增加的</param>
        /// <param name="subNames">减去的</param>
        /// <returns></returns>
        public static int MakeLayerMask(int oldLayerMask, int[] addIds = null, int[] subIds = null)
        {
            int ret = oldLayerMask;
            if (addIds != null)
            {
                foreach (int id in addIds)
                {
                    ret |= (1 << id);
                }
            }
            if (subIds != null)
            {
                foreach (int id in subIds)
                {
                    ret &= ~(1 << id);
                }
            }
            return ret;
        }

        #endregion LayerMask处理
    }
}
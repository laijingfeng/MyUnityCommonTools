using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace Jerry
{
    public class Drawer : MonoBehaviour
    {
        //id id
        //type 类型，DrawType
        //from 起点
        //to 终点
        //pos 位置
        //size 大小
        //size_factor 大小比例 
        //delay 延时秒数删除
        //color 颜色
        //wire 网格
        //text 文本

        private static Drawer m_Instance;

        private static Drawer Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = new GameObject("Drawer");
                    m_Instance = go.AddComponent<Drawer>();
                }
                return m_Instance;
            }
        }

        private static List<Hashtable> m_DrawerList = new List<Hashtable>();

        #region 对外接口

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        public static void Remove(string id)
        {
            Hashtable table = m_DrawerList.Find((x) => ((string)x["id"]).Equals(id));
            if (table != null)
            {
                m_DrawerList.Remove(table);
            }
        }

        /// <summary>
        /// 删除所有
        /// </summary>
        public static void RemoveAll()
        {
            m_DrawerList.Clear();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="table"></param>
        public static void Remove(Hashtable table)
        {
            if (table != null)
            {
                if (m_DrawerList.Contains(table))
                {
                    m_DrawerList.Remove(table);
                }
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Exist(string id)
        {
            return m_DrawerList.Exists((x) => ((string)x["id"]).Equals(id));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string Add(Hashtable table)
        {
            //只是为了创建一下
            Instance.GetComponent<Transform>();

            table = CleanArgs(table);
            string id;
            if (table.Contains("id") == false)
            {
                id = GenerateID();
                table.Add("id", id);
            }
            else
            {
                id = (string)table["id"];
            }

            Remove(id);
            m_DrawerList.Add(table);

            if (table.Contains("delay"))
            {
                Instance.StartCoroutine("DelayDelete", table);
            }

            return id;
        }

        /// <summary>
        /// Hash params
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Hashtable Hash(params object[] args)
        {
            int len = args.Length;
            Hashtable hashTable = new Hashtable(len >> 1);
            if ((len ^ 1) == 1)
            {
                Debug.LogError("Drawer Error: Hash requires an even number of arguments!");
                return null;
            }
            else
            {
                int i = 0;
                while (i < len - 1)
                {
                    hashTable.Add(args[i], args[i + 1]);
                    i += 2;
                }
                return hashTable;
            }
        }

        /// <summary>
        /// 绘制类型
        /// </summary>
        public enum DrawType
        {
            Line = 0,
            Cube,
            Label,
        }

        #endregion 对外接口

        private IEnumerator DelayDelete(Hashtable table)
        {
            if (table == null)
            {
                yield break;
            }
            yield return new WaitForSeconds((float)table["delay"]);
            if (table != null)
            {
                Remove(table);
            }
        }

        /// <summary>
        /// cast any accidentally supplied doubles and ints as floats as iTween only uses floats internally and unify parameter case
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Hashtable CleanArgs(Hashtable args)
        {
            Hashtable argsCopy = new Hashtable(args.Count);
            Hashtable argsCaseUnified = new Hashtable(args.Count);

            foreach (DictionaryEntry item in args)
            {
                argsCopy.Add(item.Key, item.Value);
            }

            foreach (DictionaryEntry item in argsCopy)
            {
                if (item.Value.GetType() == typeof(System.Int32))
                {
                    int original = (int)item.Value;
                    float casted = (float)original;
                    args[item.Key] = casted;
                }
                if (item.Value.GetType() == typeof(System.Double))
                {
                    double original = (double)item.Value;
                    float casted = (float)original;
                    args[item.Key] = casted;
                }
            }

            foreach (DictionaryEntry item in args)
            {
                argsCaseUnified.Add(item.Key.ToString().ToLower(), item.Value);
            }

            args = argsCaseUnified;

            return args;
        }

        /// <summary>
        /// generate id
        /// </summary>
        /// <returns></returns>
        private static string GenerateID()
        {
            int strlen = 15;
            char[] chars = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8' };
            int num_chars = chars.Length - 1;
            string randomChar = "";
            for (int i = 0; i < strlen; i++)
            {
                randomChar += chars[(int)Mathf.Floor(UnityEngine.Random.Range(0, num_chars))];
            }
            return randomChar;
        }

        #region 绘制

        void OnDrawGizmos()
        {
            for (int i = 0, imax = m_DrawerList.Count; i < imax; i++)
            {
                Hashtable table = (Hashtable)m_DrawerList[i];
                switch ((DrawType)table["type"])
                {
                    case DrawType.Line:
                        {
                            DrawLine(table);
                        }
                        break;
                    case DrawType.Cube:
                        {
                            DrawCube(table);
                        }
                        break;
                    case DrawType.Label:
                        {
                            DrawLabel(table);
                        }
                        break;
                }
            }
        }

        private void DrawLine(Hashtable table)
        {
            if (table.Contains("from") == false || table.Contains("to") == false)
            {
                return;
            }

            if (table.Contains("color"))
            {
                Gizmos.color = (Color)table["color"];
            }

            Gizmos.DrawLine((Vector3)table["from"], (Vector3)table["to"]);
            Gizmos.color = Color.white;
        }

        private void DrawCube(Hashtable table)
        {
            if (table.Contains("color"))
            {
                Gizmos.color = (Color)table["color"];
            }

            if (table.Contains("pos") == false)
            {
                return;
            }

            Vector3 size = Vector3.one;

            if (table.Contains("size"))
            {
                size = (Vector3)table["size"];
            }

            if (table.Contains("size_factor"))
            {
                size *= (float)table["size_factor"];
            }

            if (table.Contains("wire") && ((bool)table["wire"]) == true)
            {
                Gizmos.DrawWireCube((Vector3)table["pos"], size);
            }
            else
            {
                Gizmos.DrawCube((Vector3)table["pos"], size);
            }

            Gizmos.color = Color.white;
        }

        private void DrawLabel(Hashtable table)
        {
            if (table.Contains("color"))
            {
                GUI.color = (Color)table["color"];
            }

            if (table.Contains("pos") == false || table.Contains("text") == false)
            {
                return;
            }
#if UNITY_EDITOR
            Handles.Label((Vector3)table["pos"], (string)table["text"]);
#endif
            GUI.color = Color.white;
        }

        #endregion 绘制
    }
}
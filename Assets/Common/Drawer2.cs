using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif

namespace Jerry
{
    public class Drawer2 : MonoBehaviour
    {
#if UNITY_EDITOR

        private static Drawer2 _instance;
        private static Drawer2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Drawer");
                    _instance = go.AddComponent<Drawer2>();
                }
                return _instance;
            }
        }

        private static List<Drawer2ElementBase> m_DrawerList = new List<Drawer2ElementBase>();

        #region 对外接口

        public static Drawer2ElementBase Add()
        {
            return new Drawer2ElementBase();
        }

        #endregion 对外接口

        public class Drawer2ElementBase
        {
            /// <summary>
            /// id
            /// </summary>
            private string _id;

            /// <summary>
            /// 延时删除，小于等于0不删除
            /// </summary>
            private float _delayDelete;

            /// <summary>
            /// 颜色
            /// </summary>
            private Color _color;

            #region 对外接口

            public string ID
            {
                get
                {
                    return _id;
                }
            }

            public Drawer2ElementBase()
            {
                Debug.Log("Drawer2ElementBase");
                _id = GenerateID();
                _delayDelete = 0f;
                _color = Color.white;
            }

            public Drawer2ElementBase SetDelayDelete(float time)
            {
                Debug.Log("SetDelayDelete");
                _delayDelete = time;
                return this;
            }

            public Drawer2ElementBase SetID(string id)
            {
                Debug.Log("SetID");
                _id = id;
                return this;
            }

            public Drawer2ElementBase SetColor(Color col)
            {
                Debug.Log("SetColor");
                _color = col;
                return this;
            }

            #endregion 对外接口

            #region 辅助

            private string GenerateID()
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

            #endregion 辅助
        }
#endif
    }
}
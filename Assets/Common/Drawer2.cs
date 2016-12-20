using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
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

        private static List<Drawer2ElementBase> _drawerList = new List<Drawer2ElementBase>();
        private List<Drawer2ElementBase> _listToDelete = new List<Drawer2ElementBase>();

        #region 对外接口

        public static Drawer2ElementCube DrawCube(Vector3 pos)
        {
            Drawer2ElementCube ret = new Drawer2ElementCube(pos);
            Add(ret);
            return ret;
        }

        public static Drawer2ElementLine DrawLine(Vector3 from, Vector3 to)
        {
            Drawer2ElementLine ret = new Drawer2ElementLine(from, to);
            Add(ret);
            return ret;
        }

        public static Drawer2ElementLabel DrawLabel(Vector3 pos, string text)
        {
            Drawer2ElementLabel ret = new Drawer2ElementLabel(pos, text);
            Add(ret);
            return ret;
        }

        public static void Remove(Drawer2ElementBase ele)
        {
            Drawer2ElementBase ele2 = _drawerList.Find((x) => x.ID == ele.ID);
            if (ele2 != null)
            {
                _drawerList.Remove(ele2);
            }
        }

        public static void RemoveAll()
        {
            _drawerList.Clear();
        }

        #endregion 对外接口

        private static void Add(Drawer2ElementBase ele)
        {
            if (ele == null)
            {
                return;
            }

            //只是为了创建一下
            Instance.GetComponent<Transform>();

            Remove(ele);
            _drawerList.Add(ele);
        }

        void OnDrawGizmos()
        {
            _listToDelete.Clear();
            for (int i = 0, imax = _drawerList.Count; i < imax; i++)
            {
                if (_drawerList[i].Draw() == false)
                {
                    _listToDelete.Add(_drawerList[i]);
                }
            }
            for (int i = 0, imax = _listToDelete.Count; i < imax; i++)
            {
                Remove(_listToDelete[i]);
            }
        }
#endif
    }

    public class Drawer2ElementCube : Drawer2ElementBase
    {
        protected Vector3 _pos;
        protected Vector3 _size;
        protected float _sizeFactor;
        protected bool _wire;

        public Drawer2ElementCube(Vector3 pos)
            : base()
        {
            _pos = pos;

            _sizeFactor = 1f;
            _wire = false;
            _size = Vector3.one;
        }

        #region 对外接口

        public Drawer2ElementCube SetWire(bool wire)
        {
            _wire = wire;
            return this;
        }

        public Drawer2ElementCube SetSize(Vector3 size)
        {
            _size = size;
            return this;
        }

        public Drawer2ElementCube SetSizeFactor(float sizeFactor)
        {
            _sizeFactor = sizeFactor;
            return this;
        }

        #endregion 对外接口

        public override bool Draw()
        {
            if (base.Draw() == false)
            {
                return false;
            }

            Gizmos.color = _color;

            if (_wire)
            {
                Gizmos.DrawWireCube(_pos, _size * _sizeFactor);
            }
            else
            {
                Gizmos.DrawCube(_pos, _size * _sizeFactor);
            }

            Gizmos.color = Color.white;

            return true;
        }
    }

    public class Drawer2ElementLabel : Drawer2ElementBase
    {
        protected Vector3 _pos;
        protected string _text;

        public Drawer2ElementLabel(Vector3 pos, string text)
            : base()
        {
            _pos = pos;
            _text = text;
        }

        public override bool Draw()
        {
            if (base.Draw() == false)
            {
                return false;
            }

            GUI.color = _color;
            Handles.Label(_pos, _text);
            GUI.color = Color.white;

            return true;
        }
    }

    public class Drawer2ElementLine : Drawer2ElementBase
    {
        protected Vector3 _from;
        protected Vector3 _to;

        public Drawer2ElementLine(Vector3 from, Vector3 to)
            : base()
        {
            _from = from;
            _to = to;
        }

        public override bool Draw()
        {
            if (base.Draw() == false)
            {
                return false;
            }

            Gizmos.color = _color;
#if UNITY_EDITOR
            Gizmos.DrawLine(_from, _to);
#endif
            Gizmos.color = Color.white;

            return true;
        }
    }

    public class Drawer2ElementBase
    {
        /// <summary>
        /// id
        /// </summary>
        protected string _id;

        /// <summary>
        /// 延时删除，小于等于0不删除
        /// </summary>
        protected float _delayDelete;

        /// <summary>
        /// 颜色
        /// </summary>
        protected Color _color;

        /// <summary>
        /// 创建时间
        /// </summary>
        protected float _createTime;

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
            //Debug.Log("Drawer2ElementBase");
            _id = GenerateID();
            _delayDelete = 0f;
            _color = Color.white;
            _createTime = Time.realtimeSinceStartup;
        }

        public Drawer2ElementBase SetDelayDelete(float time)
        {
            //Debug.Log("SetDelayDelete");
            _delayDelete = time;
            return this;
        }

        public Drawer2ElementBase SetColor(Color col)
        {
            //Debug.Log("SetColor");
            _color = col;
            return this;
        }

        #endregion 对外接口

        /// <summary>
        /// 绘制
        /// </summary>
        /// <returns>是否还有效</returns>
        public virtual bool Draw()
        {
            if (_delayDelete > 0 && Time.realtimeSinceStartup - _createTime > _delayDelete)
            {
                return false;
            }
            return true;
        }

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
}
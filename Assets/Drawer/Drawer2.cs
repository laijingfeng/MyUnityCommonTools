using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jerry
{
    [ExecuteInEditMode]
    public class Drawer2 : MonoBehaviour
    {
        private static Drawer2 _instance;
        private static Drawer2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = GameObject.Find("_JerryDrawer2");
                    if (go == null)
                    {
                        go = new GameObject("_JerryDrawer2");
                    }
                    _instance = go.GetComponent<Drawer2>();
                    if (_instance == null)
                    {
                        _instance = go.AddComponent<Drawer2>();
                    }
                }
                return _instance;
            }
        }

        private static List<Drawer2ElementBase> _drawerList = new List<Drawer2ElementBase>();
        private List<Drawer2ElementBase> _listToDelete = new List<Drawer2ElementBase>();

        #region 对外接口

        public static T Draw<T>() where T : Drawer2ElementBase
        {
            T ret = (T)Activator.CreateInstance(typeof(T), true);
            Add(ret);
            return ret;
        }

        public static T GetElement<T>(string id) where T : Drawer2ElementBase
        {
            T ele = _drawerList.Find((x) => id.Equals(x.ID)) as T;
            return ele;
        }

        public static void Remove(Drawer2ElementBase ele)
        {
            if (_drawerList.Contains(ele))
            {
                _drawerList.Remove(ele);
            }
        }

        public static void RemoveByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            Drawer2ElementBase ele = _drawerList.Find((x) => id.Equals(x.ID));
            if (ele != null)
            {
                _drawerList.Remove(ele);
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
                if(_drawerList[i].IsExecuteInEditMode == false
                    && Application.isPlaying == false)
                {
                    continue;
                }

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
    }

    public class Drawer2ElementPath : Drawer2ElementBase
    {
        protected Vector3[] _points;
        protected Transform[] _tfPoints;
        
        public Drawer2ElementPath()
            : base()
        {
            _points = null;
            _tfPoints = null;
        }

        #region 对外接口

        public virtual Drawer2ElementPath SetPoints(params Transform[] tfPoints)
        {
            _tfPoints = tfPoints;
            _useVectorPoint = false;
            return this;
        }

        public virtual Drawer2ElementPath SetPoints(params Vector3[] points)
        {
            _points = points;
            _useVectorPoint = true;
            return this;
        }

        public virtual Drawer2ElementPath SetAddPoints(params Vector3[] points)
        {
            if (_points == null)
            {
                _points = points;
            }
            else
            {
                List<Vector3> tmp = new List<Vector3>();
                tmp.AddRange(_points);
                tmp.AddRange(points);
                _points = tmp.ToArray();
            }
            _useVectorPoint = true;
            return this;
        }

        public virtual new Drawer2ElementPath SetID(string id)
        {
            return base.SetID(id) as Drawer2ElementPath;
        }

        public virtual new Drawer2ElementPath SetColor(Color col)
        {
            return base.SetColor(col) as Drawer2ElementPath;
        }

        public virtual new Drawer2ElementPath SetLife(float time)
        {
            return base.SetLife(time) as Drawer2ElementPath;
        }

        public virtual new Drawer2ElementPath SetExecuteInEditMode(bool executeInEditMode)
        {
            return base.SetExecuteInEditMode(executeInEditMode) as Drawer2ElementPath;
        }

        #endregion 对外接口

        public override bool Draw()
        {
            if (base.Draw() == false)
            {
                return false;
            }

            Gizmos.color = _color;
            if (_useVectorPoint)
            {
                if (_points != null)
                {
                    for (int i = 1, imax = _points.Length; i < imax; i++)
                    {
                        Gizmos.DrawLine(_points[i - 1], _points[i]);
                    }
                }
            }
            else
            {
                if (_tfPoints != null)
                {
                    for (int i = 1, imax = _tfPoints.Length; i < imax; i++)
                    {
                        if (_tfPoints[i - 1] != null && _tfPoints[i] != null)
                        {
                            Gizmos.DrawLine(_tfPoints[i - 1].position, _tfPoints[i].position);
                        }
                    }
                }
            }
            Gizmos.color = Color.white;

            return true;
        }
    }

    public class Drawer2ElementCube : Drawer2ElementBase
    {
        protected Transform _tfPos;
        protected Vector3 _pos;
        protected Vector3 _size;
        protected float _sizeFactor;
        protected bool _wire;

        public Drawer2ElementCube()
            : base()
        {
            _tfPos = null;
            _pos = Vector3.zero;
            _sizeFactor = 1f;
            _wire = false;
            _size = Vector3.one;
        }

        #region 对外接口

        public virtual Drawer2ElementCube SetPos(Transform tfPos)
        {
            _tfPos = tfPos;
            _useVectorPoint = false;
            return this;
        }

        public virtual Drawer2ElementCube SetPos(Vector3 pos)
        {
            _pos = pos;
            _useVectorPoint = true;
            return this;
        }

        public virtual Drawer2ElementCube SetWire(bool wire)
        {
            _wire = wire;
            return this;
        }

        public virtual Drawer2ElementCube SetSize(Vector3 size)
        {
            _size = size;
            return this;
        }

        public virtual Drawer2ElementCube SetSizeFactor(float sizeFactor)
        {
            _sizeFactor = sizeFactor;
            return this;
        }

        public virtual new Drawer2ElementCube SetID(string id)
        {
            return base.SetID(id) as Drawer2ElementCube;
        }

        public virtual new Drawer2ElementCube SetColor(Color col)
        {
            return base.SetColor(col) as Drawer2ElementCube;
        }

        public virtual new Drawer2ElementCube SetLife(float time)
        {
            return base.SetLife(time) as Drawer2ElementCube;
        }

        public virtual new Drawer2ElementCube SetExecuteInEditMode(bool executeInEditMode)
        {
            return base.SetExecuteInEditMode(executeInEditMode) as Drawer2ElementCube;
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
                if (_useVectorPoint == false && _tfPos != null)
                {
                    Gizmos.DrawWireCube(_tfPos.position, _size * _sizeFactor);
                }
                else
                {
                    Gizmos.DrawWireCube(_pos, _size * _sizeFactor);
                }
            }
            else
            {
                if (_useVectorPoint == false && _tfPos != null)
                {
                    Gizmos.DrawCube(_tfPos.position, _size * _sizeFactor);
                }
                else
                {
                    Gizmos.DrawCube(_pos, _size * _sizeFactor);
                }
            }

            Gizmos.color = Color.white;

            return true;
        }
    }

    public class Drawer2ElementLabel : Drawer2ElementBase
    {
        protected Transform _tfPos;
        protected Vector3 _pos;
        protected string _text;

        public Drawer2ElementLabel()
            : base()
        {
            _pos = Vector3.zero;
            _text = string.Empty;
        }

        #region 对外接口

        public virtual Drawer2ElementLabel SetPos(Transform tfPos)
        {
            _tfPos = tfPos;
            _useVectorPoint = false;
            return this;
        }

        public virtual Drawer2ElementLabel SetPos(Vector3 pos)
        {
            _pos = pos;
            _useVectorPoint = true;
            return this;
        }

        public virtual Drawer2ElementLabel SetText(string text)
        {
            _text = text;
            return this;
        }

        public virtual new Drawer2ElementLabel SetID(string id)
        {
            return base.SetID(id) as Drawer2ElementLabel;
        }

        public virtual new Drawer2ElementLabel SetColor(Color col)
        {
            return base.SetColor(col) as Drawer2ElementLabel;
        }

        public virtual new Drawer2ElementLabel SetLife(float time)
        {
            return base.SetLife(time) as Drawer2ElementLabel;
        }

        public virtual new Drawer2ElementLabel SetExecuteInEditMode(bool executeInEditMode)
        {
            return base.SetExecuteInEditMode(executeInEditMode) as Drawer2ElementLabel;
        }

        #endregion 对外接口

        public override bool Draw()
        {
            if (base.Draw() == false)
            {
                return false;
            }

            GUI.color = _color;
#if UNITY_EDITOR
            if (_useVectorPoint == false && _tfPos != null)
            {
                Handles.Label(_tfPos.position, _text);
            }
            else
            {
                Handles.Label(_pos, _text);
            }
#endif
            GUI.color = Color.white;

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
        /// 生命周期，小于等于0表示无穷，单位秒
        /// </summary>
        protected float _life;

        /// <summary>
        /// 颜色
        /// </summary>
        protected Color _color;

        /// <summary>
        /// 创建时间
        /// </summary>
        protected float _createTime;

        protected bool _executeInEditMode;

        /// <summary>
        /// 点使用Vector而不是Transform
        /// </summary>
        protected bool _useVectorPoint;

        public Drawer2ElementBase()
        {
            _id = string.Empty;
            _life = 0f;
            _color = Color.white;
            _createTime = Time.realtimeSinceStartup;
            _executeInEditMode = false;
            _useVectorPoint = true;
        }

        #region 对外接口

        public string ID
        {
            get
            {
                return _id;
            }
        }

        public bool IsExecuteInEditMode
        {
            get
            {
                return _executeInEditMode;
            }
        }

        /// <summary>
        /// 编辑器模式可用
        /// </summary>
        /// <param name="executeInEditMode"></param>
        /// <returns></returns>
        public virtual Drawer2ElementBase SetExecuteInEditMode(bool executeInEditMode)
        {
            _executeInEditMode = executeInEditMode;
            return this;
        }

        public virtual Drawer2ElementBase SetID(string id)
        {
            _id = id;
            return this;
        }

        public virtual Drawer2ElementBase SetLife(float time)
        {
            _createTime = Time.realtimeSinceStartup;
            _life = time;
            return this;
        }

        public virtual Drawer2ElementBase SetColor(Color col)
        {
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
            if (_life > 0 && Time.realtimeSinceStartup - _createTime > _life)
            {
                return false;
            }
            return true;
        }
    }
}
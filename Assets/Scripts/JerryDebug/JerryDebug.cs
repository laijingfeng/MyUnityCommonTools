using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class JerryDebug : MonoBehaviour
{
    #region 配置

    /// <summary>
    /// 单类最大日志量
    /// </summary>
    private const int MAX_MSG_CNT = 100;

    /// <summary>
    /// 是否启用
    /// </summary>
    private static bool m_Active = true;

    /// <summary>
    /// 是否接收消息
    /// </summary>
    private static bool m_ReceiveMsg = true;

    /// <summary>
    /// 多余日志按时间顺序删除
    /// </summary>
    private static bool m_LogDeleteByTime = true;

    /// <summary>
    /// LOG文件目录
    /// </summary>
    private static string LOG_FILE_PATH = Application.dataPath + "/../JerryDebug.txt";

    /// <summary>
    /// 按原始类型输出的类型
    /// </summary>
    private static List<string> m_OriginalTypes = new List<string>()
    {
        "System.String",//string
        "System.Int32",//int
        "System.Single",//float
    };

    #endregion 配置

    private static int m_CntInfo = 0;
    private static int m_CntWarning = 0;
    private static int m_CntError = 0;

    /// <summary>
    /// 单例
    /// </summary>
    private static JerryDebug m_instance = null;

    /// <summary>
    /// Log类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info = 0,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }

    /// <summary>
    /// 消息信息
    /// </summary>
    private class MsgInfo
    {
        public string m_strMessage;
        public LogType m_logType;
    }

    /// <summary>
    /// 消息列表
    /// </summary>
    private static List<MsgInfo> m_listMsgList = new List<MsgInfo>();

    /// <summary>
    /// Log面板当前浏览进度
    /// </summary>
    private Vector2 m_vtScrollView = Vector2.zero;

    /// <summary>
    /// 低端控制按钮高度
    /// </summary>
    private float m_oneHeight = 22f;

    /// <summary>
    /// 是否显示最新的
    /// </summary>
    private bool m_ShowNewer = false;

    /// <summary>
    /// 是否显示帮助
    /// </summary>
    private bool m_Help = false;

    /// <summary>
    /// 是否显示错误
    /// </summary>
    private bool m_ShowError = true;

    /// <summary>
    /// 是否显示警告
    /// </summary>
    private bool m_ShowWarning = true;

    /// <summary>
    /// 是否显示普通信息
    /// </summary>
    private bool m_ShowInfo = true;

    /// <summary>
    /// 窗口大小
    /// </summary>
    private static Rect m_rect = new Rect(0, 0, Screen.width * 0.5f + 15, Screen.height * 0.5f);

    /// <summary>
    /// 帮助面板大小
    /// </summary>
    private Rect m_recHelp = new Rect(Screen.width * 0.5f + 15, 0, Screen.width * 0.5f - 15, Screen.height * 0.5f);

    #region 对外接口

    /// <summary>
    /// LOG
    /// </summary>
    /// <param name="msg"></param>
    public static void Log(object msg)
    {
        AddLog(msg, LogType.Info);
    }

    /// <summary>
    /// 警告
    /// </summary>
    /// <param name="msg"></param>
    public static void LogWarning(object msg)
    {
        AddLog(msg, LogType.Warning);
    }

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="msg"></param>
    public static void LogError(object msg)
    {
        AddLog(msg, LogType.Error);
    }

    /// <summary>
    /// 输出到文件
    /// </summary>
    /// <param name="logMsg"></param>
    /// <returns></returns>
    public static bool LogFile(object msg)
    {
        bool bRet = true;

        string logMsg = HandleInfo(msg);

        FileMode fileMode = FileMode.Create;

        if (File.Exists(LOG_FILE_PATH))
        {
            fileMode = FileMode.Append;
        }

        try
        {
            FileStream fileStream = new FileStream(LOG_FILE_PATH, fileMode);
            StreamWriter streamWriter = new StreamWriter(fileStream);

            streamWriter.WriteLine(string.Format("{0}\n{1}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logMsg));

            streamWriter.Close();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("err=" + ex.ToString());
            bRet = false;
        }

        return bRet;
    }

    #endregion 对外接口

    private static string HandleInfo(object obj)
    {
        System.Type t = obj.GetType();
        foreach (string s in m_OriginalTypes)
        {
            if (t.ToString().Equals(s))
            {
                return obj.ToString();
            }
        }
        return JsonUtility.ToJson(obj, true);
    }

    /// <summary>
    /// Log
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="color"></param>
    private static void AddLog(object msg, LogType logType)
    {
        if (!Application.isPlaying || m_Active == false || m_ReceiveMsg == false)
        {
            return;
        }

        string strMessage = HandleInfo(msg); 

        if (m_instance == null)
        {
            GameObject go = new GameObject("JerryDebug");
            m_instance = go.AddComponent<JerryDebug>();
            DontDestroyOnLoad(go);
        }

        #region CheckCnt

        if (m_LogDeleteByTime)
        {
            if (m_CntWarning + m_CntInfo + m_CntError >= MAX_MSG_CNT * 3)
            {
                MsgInfo mi = m_listMsgList[0];
                if (mi != null)
                {
                    m_listMsgList.Remove(mi);
                    switch (mi.m_logType)
                    {
                        case LogType.Info:
                            {
                                m_CntInfo--;
                            }
                            break;
                        case LogType.Warning:
                            {
                                m_CntWarning--;
                            }
                            break;
                        case LogType.Error:
                            {
                                m_CntError--;
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            switch (logType)
            {
                case LogType.Warning:
                    {
                        while (m_CntWarning >= MAX_MSG_CNT)
                        {
                            MsgInfo mi = m_listMsgList.Find((x) => x.m_logType == LogType.Warning);
                            if (mi != null)
                            {
                                m_listMsgList.Remove(mi);
                                m_CntWarning--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
                case LogType.Info:
                    {
                        while (m_CntInfo >= MAX_MSG_CNT)
                        {
                            MsgInfo mi = m_listMsgList.Find((x) => x.m_logType == LogType.Info);
                            if (mi != null)
                            {
                                m_listMsgList.Remove(mi);
                                m_CntInfo--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
                case LogType.Error:
                    {
                        while (m_CntError >= MAX_MSG_CNT)
                        {
                            MsgInfo mi = m_listMsgList.Find((x) => x.m_logType == LogType.Error);
                            if (mi != null)
                            {
                                m_listMsgList.Remove(mi);
                                m_CntError--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        #endregion

        switch (logType)
        {
            case LogType.Info:
                {
                    m_CntInfo++;
                }
                break;
            case LogType.Warning:
                {
                    m_CntWarning++;
                }
                break;
            case LogType.Error:
                {
                    m_CntError++;
                }
                break;
        }

        m_listMsgList.Add(new MsgInfo()
        {
            m_strMessage = System.DateTime.Now.ToString("HH:mm:ss") + "：\n" + strMessage,
            m_logType = logType,
        });
    }

    void OnGUI()
    {
        if (m_Active == false)
        {
            return;
        }

        m_rect = GUI.Window(0, m_rect, RefreshUI, "JerryDebug");
        if (m_Help)
        {
            m_recHelp.x = m_rect.x + m_rect.width;
            m_recHelp.y = m_rect.y;
            GUI.Window(1, m_recHelp, RefreshHelp, "Setting");
        }
        FillBottomCtr();
    }

    #region 扩展

    /// <summary>
    /// 填充下面操作
    /// </summary>
    private void FillBottomCtr()
    {
        GUI.color = Color.green;
        if (GUI.Button(new Rect(m_rect.x, m_rect.y + m_rect.height, m_rect.width * 0.5f, m_oneHeight), "Help"))
        {
            m_Help = !m_Help;
        }
        GUI.color = Color.white;

        if (GUI.Button(new Rect(m_rect.x + m_rect.width * 0.5f, m_rect.y + m_rect.height, m_rect.width * 0.5f, m_oneHeight), "Clear"))
        {
            m_listMsgList.Clear();
            m_CntError = 0;
            m_CntInfo = 0;
            m_CntWarning = 0;
        }
    }

    public static Action CtrAction1;
    public static string CtrAction1Name = "1";

    public static Action CtrAction2;
    public static string CtrAction2Name = "2";

    public static Action CtrAction3;
    public static string CtrAction3Name = "3";

    /// <summary>
    /// 填充控制面板
    /// </summary>
    private void FillCtrButton()
    {
        GUILayout.BeginHorizontal();

        GUI.color = Color.green;

        if (GUILayout.Button(CtrAction1Name))
        {
            if(CtrAction1 != null)
            {
                CtrAction1();
            }
        }

        if (GUILayout.Button(CtrAction2Name))
        {
            if (CtrAction2 != null)
            {
                CtrAction2();
            }
        }

        if (GUILayout.Button(CtrAction3Name))
        {
            if (CtrAction3 != null)
            {
                CtrAction3();
            }
        }

        GUI.color = Color.white;
        GUILayout.EndHorizontal();
    }

    #endregion 扩展

    private void RefreshHelp(int iWindowID)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        m_ReceiveMsg = GUILayout.Toggle(m_ReceiveMsg, "ReceiveMsg");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUI.color = Color.white;
        m_ShowNewer = GUILayout.Toggle(m_ShowNewer, "ToBottom");
        GUI.color = Color.white;

        GUI.color = Color.white;
        m_LogDeleteByTime = GUILayout.Toggle(m_LogDeleteByTime, "DeleteByTimeOrType");
        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUI.color = Color.white;
        m_ShowInfo = GUILayout.Toggle(m_ShowInfo, "Info");
        GUI.color = Color.white;

        GUI.color = Color.yellow;
        m_ShowWarning = GUILayout.Toggle(m_ShowWarning, "Warn");
        GUI.color = Color.white;

        GUI.color = Color.red;
        m_ShowError = GUILayout.Toggle(m_ShowError, "Error");
        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        FillCtrButton();

        GUILayout.EndVertical();
    }

    /// <summary>
    /// 刷新信息
    /// </summary>
    private void RefreshUI(int iWindowID)
    {
        float width = Screen.width * 0.5f;
        float height = Screen.height * 0.5f;
        float width1 = 15, height1 = 30;
        GUI.DragWindow(new Rect(0, 0, width - width1 - 10, height));
        GUI.DragWindow(new Rect(width, 0, 15, height));
        if (m_ShowNewer)
        {
            m_vtScrollView.y = 10000f;
        }
        m_vtScrollView = GUILayout.BeginScrollView(m_vtScrollView, GUILayout.Width(width - width1), GUILayout.Height(height - height1));
        for (int i = 0, imax = m_listMsgList.Count; i < imax; i++)
        {
            if (!IsShow(m_listMsgList[i].m_logType))
            {
                continue;
            }
            GUI.color = GetColor(m_listMsgList[i].m_logType);
            GUI.skin.box.alignment = TextAnchor.UpperLeft;
            GUI.skin.box.wordWrap = true;
            GUILayout.Box(m_listMsgList[i].m_strMessage);
        }
        GUILayout.EndScrollView();
    }

    private Color GetColor(LogType type)
    {
        Color cc = Color.white;
        switch (type)
        {
            case LogType.Info:
                {
                    cc = Color.white;
                }
                break;
            case LogType.Warning:
                {
                    cc = Color.yellow;
                }
                break;
            case LogType.Error:
                {
                    cc = Color.red;
                }
                break;
        }
        return cc;
    }

    private bool IsShow(LogType type)
    {
        if (m_ShowError && type == LogType.Error)
        {
            return true;
        }
        if (m_ShowWarning && type == LogType.Warning)
        {
            return true;
        }
        if (m_ShowInfo && type == LogType.Info)
        {
            return true;
        }
        return false;
    }
}
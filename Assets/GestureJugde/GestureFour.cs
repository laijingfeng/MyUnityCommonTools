using UnityEngine;

public class GestureFour : GestureBase
{
    /// <summary>
    /// 手势方向
    /// </summary>
    public enum GestureDir
    {
        Right = 0,
        Up,
        Left,
        Down,
    }

    protected override void Start()
    {
        m_CutPart = 4;
        base.Start();
    }

    protected override void JudgeDir(int idx)
    {
        base.JudgeDir(idx);
        Debug.LogWarning((GestureDir)idx);
    }
}
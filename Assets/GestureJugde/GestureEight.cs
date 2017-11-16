using UnityEngine;

public class GestureEight : GestureBase
{
    /// <summary>
    /// 手势方向
    /// </summary>
    public enum GestureDir
    {
        Right = 0,
        RightUp,
        Up,
        LeftUp,
        Left,
        LeftDown,
        Down,
        RightDown,
    }

    protected override void Start()
    {
        m_CutPart = 8;
        base.Start();
    }

    protected override void JudgeDir(int idx)
    {
        base.JudgeDir(idx);
        Debug.LogWarning((GestureDir)idx);
    }
}
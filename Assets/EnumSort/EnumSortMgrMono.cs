using System.Collections.Generic;
using System;
using UnityEngine;

public class EnumSortMgr
{
    private List<EnumSort> list;

    public EnumSortMgr()
    {
        list = new List<EnumSort>();
    }

    public void DoTest<T>()
    {
        Array arr = Enum.GetValues(typeof(T));
        foreach (T val in arr)
        {
            this.Add(val.ToString(), val.GetHashCode());
        }
        Print();
    }

    private void Add(string name, int value)
    {
        list.Add(new EnumSort()
        {
            name = name,
            value = value,
        });
    }

    private void Print()
    {
        if (list == null || list.Count <= 0)
        {
            return;
        }
        list.Sort();

        string res1 = string.Empty;
        string res = string.Empty;

        foreach (EnumSort s in list)
        {
            res1 += s.name + ",";
            res += s.value + ",";
        }
        if (!string.IsNullOrEmpty(res))
        {
            res1 = res1.Substring(0, res1.Length - 1);
            res = res.Substring(0, res.Length - 1);
        }

        Debug.LogWarning(res1);
        Debug.LogWarning(res);
    }
}

public class EnumSort : IComparable
{
    public string name;
    public int value;

    public int CompareTo(object obj)
    {
        EnumSort other = obj as EnumSort;
        int result = this.name.CompareTo(other.name);
        return result;
    }
}

[ExecuteInEditMode]
public class EnumSortMgrMono : MonoBehaviour
{
    public bool DoWork = false;

    void Update()
    {
        if (DoWork)
        {
            DoWork = false;
            Test();
        }
    }

    private void Test()
    {
        EnumSortMgr enumMgr = new EnumSortMgr();
        enumMgr.DoTest<NewType>();
    }
}
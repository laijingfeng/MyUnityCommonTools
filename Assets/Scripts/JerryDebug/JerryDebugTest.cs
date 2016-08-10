using UnityEngine;
using System.Collections;

public class JerryDebugTest : MonoBehaviour
{
    void Start()
    {
        JerryDebug.Log(new DebugInfo());
        JerryDebug.LogFile(new DebugInfo());
        JerryDebug.Log(1);
        JerryDebug.Log(2);
        JerryDebug.Log("hello");
    }

    public class DebugInfo
    {
        public enum Sex
        {
            MALE,
            FEMAL,
        }

        public string name;
        public int age;
        public Sex sex;
        public float val;
        
        public DebugInfo()
        {
            age = 10;
            val = 1.1f;
            name = "Jerry";
            sex = Sex.MALE;
        }
    }
}

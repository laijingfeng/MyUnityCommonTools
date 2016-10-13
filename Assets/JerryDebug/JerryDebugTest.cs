using UnityEngine;
using System.Collections;

public class JerryDebugTest : MonoBehaviour
{
    void Start()
    {
        JerryDebug.Log(new DebugInfo());
        JerryDebug.LogFile(new DebugInfo());
        JerryDebug.Log(1);
        JerryDebug.Log(2, JerryDebug.LogType.Warning);
        JerryDebug.Log("hello", JerryDebug.LogType.Error);

        JerryDebug.CtrAction.Add(new JerryDebug.ExtenActionConfig()
        {
            name = "lai0",
            action = () =>
            {
                Debug.LogError("lai0");
            },
        });

        JerryDebug.CtrAction.Add(new JerryDebug.ExtenActionConfig()
        {
            name = "lai1",
            action = () =>
            {
                Debug.LogError("lai1");
            },
        });

        JerryDebug.CtrAction.Add(new JerryDebug.ExtenActionConfig()
        {
            name = "lai2",
            action = () =>
            {
                Debug.LogError("lai2");
            },
        });

        JerryDebug.CtrAction.Add(new JerryDebug.ExtenActionConfig()
        {
            name = "lai3",
            action = () =>
            {
                Debug.LogError("lai3");
            },
        });
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

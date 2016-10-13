using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Table;

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
                JerryDebug.Log("click lai0");
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

        SCENE scene = new SCENE();
        scene.desc = "desc";
        scene.id = 1;
        scene.num_float = 1.1f;
        scene.num_uint32.AddRange(new List<uint>() { 1, 2, 3 });

        JerryDebug.Log(scene, JerryDebug.LogType.Warning, true);
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

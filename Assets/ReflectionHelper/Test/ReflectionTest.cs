using UnityEngine;

public class ReflectionTest : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ReflectionHelper.ExecuteStaticMethod(typeof(RTest), "Test1");
            ReflectionHelper.ExecuteStaticMethod(typeof(RTest), "Test2", 100);
            //ReflectionHelper.ExecuteStaticMethod(typeof(RTest), "Test3");
        }
    }
}

public class RTest
{
    /// <summary>
    /// private不行
    /// </summary>
    public static void Test1()
    {
        Debug.LogError("Test1");
    }

    public static void Test2(int id)
    {
        Debug.LogError("Test2 " + id);
    }

    public void Test3()
    {
        Debug.LogError("Test3");
    }
}
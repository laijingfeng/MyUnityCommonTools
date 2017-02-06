using UnityEngine;
using Jerry;

public class ReflectionTest : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ReflectionHelper.GetStaticMemberValue(typeof(RTest), "Test1");
            ReflectionHelper.SetStaticMemberValue(typeof(RTest), "Test2", 100);
            
            ReflectionHelper.SetNonStaticMemberValue(new RTest(), "Test3");
            ReflectionHelper.SetNonStaticMemberValue(new RTest(), "Test4");
        }
    }
}

public class RTest
{
    private static void Test1()
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

    private void Test4()
    {
        Debug.LogError("Test4");
    }
}
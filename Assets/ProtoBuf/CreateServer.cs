using UnityEngine;

public class CreateServer : MonoBehaviour
{
    void Start()
    {
        NetServer.Instance.Start();
    }
}
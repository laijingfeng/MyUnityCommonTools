using UnityEngine;
using Jerry;

public class GetGraph : MonoBehaviour
{
    public AIMgr mgr;

    void Start()
    {

    }

    void Update()
    {

    }

    [ContextMenu("DoGet")]
    public void DoGet()
    {
        if (mgr != null)
        {
            Debug.Log(mgr.GetGraph());
        }
    }
}
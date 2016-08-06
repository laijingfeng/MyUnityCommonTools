using UnityEngine;
using System.Collections;

public class RadarChartTest : MonoBehaviour
{
    private RadarChart rc;

    void Start()
    {
        rc = this.gameObject.AddComponent<RadarChart>();
        rc.Reset(new float[6] { 0.1f, 0.5f, 0.2f, 0.5f, 0.5f, 0.3f });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            rc.Change(Random.Range(0, 6), Random.Range(0f, 1f));
        }
    }
}

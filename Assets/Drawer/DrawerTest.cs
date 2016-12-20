using UnityEngine;
using System.Collections;
using Jerry;

public class DrawerTest : MonoBehaviour
{
    void Start()
    {
        Drawer2.Draw<Drawer2ElementPath>()
            .SetPoints(Vector3.one, Vector3.zero, Vector3.left)
            .SetAddPoints(Vector3.one);
        Drawer2.Draw<Drawer2ElementLabel>()
            .SetPos(new Vector3(1, 2, 1))
            .SetText("xxx")
            .SetColor(Color.red);
        Drawer2.Draw<Drawer2ElementCube>()
            .SetPos(new Vector3(1, 0, 1))
            .SetWire(true)
            .SetSize(Vector3.one)
            .SetSizeFactor(3)
            .SetColor(Color.green);
    }

    void Update()
    {

    }
}

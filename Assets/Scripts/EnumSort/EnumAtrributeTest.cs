using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnumAtrributeTest : MonoBehaviour
{
    [EnumLabel("NewType", 4, 3, 2, 1)]
    public NewType newType = NewType.One;

    void Start()
    {

    }
}

public enum NewType
{
    [EnumLabel("One")]
    One = 1,
    [EnumLabel("Two")]
    Two = 2,
    [EnumLabel("Three")]
    Three = 3,
    [EnumLabel("Four")]
    Four = 4,
}

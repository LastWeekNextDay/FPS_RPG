using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBackpack : Backpack
{
    public GameObject containerOf;
    public ContainerBackpack(GameObject container) : base()
    {
        containerOf = container;
    }
}

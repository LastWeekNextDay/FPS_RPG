using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public List<Item> items;
    public Backpack Backpack;

    void Start()
    {
        if (items == null)
        {
            items = new List<Item>();
        }
        Backpack = new Backpack(null, this);
        foreach (var item in items)
        {
            Backpack.TryAddItem(item);
        }
    }
}

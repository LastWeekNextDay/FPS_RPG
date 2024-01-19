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
        for (int i = 0; i < items.Count; i++)
        {
            Backpack.TryAddItem(items[i]);
            items[i].transform.SetParent(transform);
            items[i].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            items[i].gameObject.SetActive(false);
        }
    }
}

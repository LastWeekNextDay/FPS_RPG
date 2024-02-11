using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerObject : HittableObject
{
    public List<Item> items;
    public ContainerBackpack containerBackpack;

    public override void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint)
    {
        var args = new HittableObjectHitArgs{
            MaterialType = MaterialType,
            Direction = dirFromWhereHit,
            HitPoint = hitPoint
        };
        OnHit?.Invoke(args);
    }

    void Start()
    {
        items ??= new List<Item>();
        containerBackpack = new ContainerBackpack(gameObject);
        
        for (int i = 0; i < items.Count; i++)
        {
            containerBackpack.TryAddItem(items[i]);
            items[i].transform.SetParent(transform);
            items[i].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            items[i].gameObject.SetActive(false);
        }
    }
}

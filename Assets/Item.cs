using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Miscellaneous
}

public class Item : MonoBehaviour
{
    [NonSerialized] public ItemType ItemType;
    [NonSerialized] public bool isEquipment;
    public bool IsInteractable;
    public bool IsForcefullyEquipped;
    public Sprite itemIcon;

    void Start()
    {
        if (GetComponent<Weapon>() != null)
        {
            ItemType = ItemType.Weapon;
            isEquipment = true;
        } else {
            ItemType = ItemType.Miscellaneous;
            isEquipment = false;
        }
    }

    public bool IsMovableFromSlot()
    {
        if (IsInteractable == false)
        {
            return false;
        }
        if (IsForcefullyEquipped == true)
        {
            return false;
        }
        return true;
    }

    public bool IsToBeReplaced()
    {
        if (IsInteractable == false && IsForcefullyEquipped == false)
        {
            return true;
        }
        return false;
    }

    public void SetActiveInWorld(bool active, Vector3 pos = default, Transform parent = null)
    {
        if (active)
        {
            transform.position = pos;
        }
        else
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        gameObject.SetActive(active);
        transform.SetParent(parent);   
    }
}

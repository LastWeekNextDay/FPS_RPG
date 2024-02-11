using System;
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
    public string itemName;
    [NonSerialized] public ItemType ItemType;
    public bool IsInteractable;
    public bool IsForcefullyEquipped;
    public Sprite itemIcon;
    [NonSerialized] public Vector3 OriginalScale;

    void Awake()
    {
        OriginalScale = transform.localScale;
    }

    protected virtual void Start()
    {
        
    }

    public bool isEquipment()
    {
        switch (ItemType)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
                return true;
            default:
                return false;
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
        gameObject.SetActive(active);
        
        if (active)
        {
            transform.position = pos;
        }
        else
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        transform.SetParent(parent);
        transform.localScale = OriginalScale;
    }
}

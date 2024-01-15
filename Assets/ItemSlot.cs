using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryItemUI item;
    public Backpack backpack;
    public bool isEquipmentSlot;

    public void Attach(InventoryItemUI item)
    {
        this.item = item;
        item.transform.SetParent(transform);
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        item.transform.localScale = Vector3.one;
        item.slotAttachedTo = this;
    }

    public void Detach()
    {
        item.slotAttachedTo = null;
        item.transform.SetParent(null);
        item = null;
    }
}

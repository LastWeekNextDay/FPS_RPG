using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public InventoryItemUI invItem;
    public Backpack backpack;
    public bool isEquipmentSlot;

    public void Attach(InventoryItemUI item)
    {
        invItem = item;
        invItem.transform.SetParent(transform);
        invItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        invItem.transform.localScale = Vector3.one;
        invItem.slotAttachedTo = this;
    }

    public void Detach()
    {
        invItem.slotAttachedTo = null;
        invItem.transform.SetParent(null);
        invItem = null;
    }
}

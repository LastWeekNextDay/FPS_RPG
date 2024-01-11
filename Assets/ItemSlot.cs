using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public InventoryItem item;
    public Backpack backpack;

    public void Attach(InventoryItem item)
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

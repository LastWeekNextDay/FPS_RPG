using System;
using UnityEngine;

public enum SlotType
{
    Container,
    Backpack,
    EquipmentWeaponPrimary,
    EquipmentWeaponSecondary,
    EquipmentArmorHead,
    EquipmentArmorChest,
    EquipmentArmorLegs,
    EquipmentArmorFeet
}

public abstract class InventorySlot : MonoBehaviour
{
    [NonSerialized] public int SlotIndex;
    public SlotType slotType;
    public InventoryItemContainer attachedInventoryItemContainer;
    public static Action<InventoryItemContainerSlotAttachmentArgs> OnInventoryItemContainerSlotAttachment;

    public void AssignVisual(InventoryItemContainer item)
    {
        item.transform.SetParent(transform);
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        item.transform.localScale = Vector3.one;

        attachedInventoryItemContainer = item;
        attachedInventoryItemContainer.slotAttachedTo = this;
    }

    public void UnassignVisual()
    {
        attachedInventoryItemContainer.slotAttachedTo = null;
        attachedInventoryItemContainer = null;
    }

    public abstract void Assign(InventoryItemContainer itemContainer);

    public abstract void Unassign();
}

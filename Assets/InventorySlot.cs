using System;
using UnityEngine;

public enum SlotType
{
    Backpack,
    EquipmentWeaponPrimary,
    EquipmentWeaponSecondary,
    EquipmentArmorHead,
    EquipmentArmorChest,
    EquipmentArmorLegs,
    EquipmentArmorFeet
}

public class InventorySlot : MonoBehaviour
{
    [NonSerialized] public int backpackSlotIndex;
    public SlotType slotType;
    public InventoryItemUI attachedInventoryItem;
    public Backpack Backpack;
    public Equipment Equipment;

    public void ForceAttach(InventoryItemUI item)
    {
        attachedInventoryItem = item;
        attachedInventoryItem.transform.SetParent(transform);
        attachedInventoryItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        attachedInventoryItem.transform.localScale = Vector3.one;
        attachedInventoryItem.slotAttachedTo = this;
    }

    public void ForceDetach()
    {
        attachedInventoryItem.slotAttachedTo = null;
        attachedInventoryItem.transform.SetParent(null);
        attachedInventoryItem = null;
    }

    public bool IsEquipmentSlot()
    {
        switch(slotType)
        {
            case SlotType.EquipmentWeaponPrimary:
            case SlotType.EquipmentWeaponSecondary:
            case SlotType.EquipmentArmorHead:
            case SlotType.EquipmentArmorChest:
            case SlotType.EquipmentArmorLegs:
            case SlotType.EquipmentArmorFeet:
                return true;
            default:
                return false;
        }
    }

    public void Attach(InventoryItemUI item)
    {
        if (attachedInventoryItem == null || attachedInventoryItem.RepresentedItem.IsToBeReplaced())
        {
            if (item.slotType != slotType)
            {
                item.CancelSelection();
            }
            // If this is an equipment slot, do special checks
            if (IsEquipmentSlot())
            {
                switch (slotType)
                {
                    case SlotType.EquipmentWeaponPrimary:
                        // Remove the temporary equipment item first
                        if (Equipment.WeaponInventoryItem != null){
                            if (Equipment.WeaponInventoryItem.RepresentedItem.IsToBeReplaced())
                            {
                                if (Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsReady ||
                                    Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsAttacking)
                                {
                                        return;
                                }
                                Destroy(Equipment.WeaponInventoryItem.RepresentedItem);
                                Destroy(Equipment.WeaponInventoryItem);  
                            }
                        }
                        if (item.RepresentedItem.GetComponent<Weapon>().IsTwoHanded == false)
                        {
                            // TODO: Special checkif two handed.
                        }
                        if (Equipment.WeaponInventoryItem == null)
                        {
                            if (item.slotAttachedTo != null){
                                item.slotAttachedTo.ForceDetach();
                            }
                            item.EquipOn(Equipment.Owner, this);
                            return;
                        }
                        return; 
                    default:
                        item.CancelSelection();
                        return;
                }
            } else {
                if (item.slotAttachedTo != null)
                {
                    // If the item trying to attach is already attached to a slot, and that slot is an equipment slot
                    // Unequip it first
                    if (item.slotAttachedTo.IsEquipmentSlot())
                    {
                        switch (item.slotAttachedTo.slotType)
                        {
                            case SlotType.EquipmentWeaponPrimary:
                                var invItem = item.slotAttachedTo.Equipment.WeaponInventoryItem;
                                invItem.UnequipFrom(item.slotAttachedTo.Equipment.Owner, invItem.slotAttachedTo);
                                if (invItem.RepresentedItem.IsToBeReplaced())
                                {
                                    Destroy(invItem.RepresentedItem);
                                    Destroy(invItem);
                                    return;  
                                }
                                break;
                        }  
                    } else {
                        item.slotAttachedTo.Backpack.TryRemoveItem(item);
                    }
                    item.slotAttachedTo.ForceDetach();
                }
                ForceAttach(item);
                Backpack.TryChangeItemPosition(item, backpackSlotIndex);
            }
            return; 
        } else {
            item.CancelSelection();
        }
    }
}

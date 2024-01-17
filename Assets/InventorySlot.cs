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
    public InventoryItem attachedInventoryItem;
    public Backpack Backpack;
    public Equipment Equipment;

    public void AssignVisual(InventoryItem item)
    {
        attachedInventoryItem = item;
        attachedInventoryItem.transform.SetParent(transform);
        attachedInventoryItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        attachedInventoryItem.transform.localScale = Vector3.one;
        attachedInventoryItem.slotAttachedTo = this;
    }

    public void UnassignVisual()
    {
        attachedInventoryItem.slotAttachedTo = null;
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

    public void Assign(InventoryItem invItem)
    {
        if (IsEquipmentSlot())
        {
            if (attachedInventoryItem == null || attachedInventoryItem.RepresentedItem.IsToBeReplaced())
            {
                switch (slotType)
                {
                    case SlotType.EquipmentWeaponPrimary:
                        // Remove the temporary equipment item first
                        if (Equipment.WeaponItem != null){
                            if (Equipment.WeaponItem.GetComponent<Weapon>().IsReady ||
                                Equipment.WeaponItem.GetComponent<Weapon>().IsAttacking)
                            {
                                    invItem.CancelSelection();
                                    return;
                            }
                            if (Equipment.WeaponItem.IsToBeReplaced())
                            {
                                Destroy(Equipment.WeaponItem);  
                            }
                        }
                        if (invItem.RepresentedItem.GetComponent<Weapon>().IsTwoHanded == false)
                        {
                            // TODO: Special checkif two handed.
                        }
                        if (Equipment.WeaponItem == null)
                        {
                            if (Equipment.TryEquip(invItem.RepresentedItem)){
                                if (invItem.slotAttachedTo != null){
                                    invItem.slotAttachedTo.Unassign();
                                }
                                AssignVisual(invItem);
                                invItem.isEquipped = true;
                                invItem.equippedBy = Equipment.Owner;
                                return;
                            } else {
                                invItem.CancelSelection();
                                return;
                            }
                        }
                        return; 
                    default:
                        throw new NotImplementedException();
                }
            }
        } 
        else if (attachedInventoryItem == null)
        {
            if (invItem.slotAttachedTo != null)
            {
                invItem.slotAttachedTo.Unassign();
            }
            if (Backpack.TryChangeItemPosition(invItem.RepresentedItem, backpackSlotIndex))
            {
                AssignVisual(invItem);
                invItem.isEquipped = false;
                invItem.equippedBy = null;
            }    
        } else {
            invItem.CancelSelection();
        }
    }

    public void Unassign()
    {
        if (IsEquipmentSlot())
        {
            switch (slotType)
            {
                case SlotType.EquipmentWeaponPrimary:
                    var item = attachedInventoryItem;
                    if (Equipment.TryUnequip(item.RepresentedItem.GetComponent<Weapon>()))
                    {
                        item.isEquipped = false;
                        item.equippedBy = null;
                        UnassignVisual();
                    } else {
                        return;
                    }
                    if (item.RepresentedItem.IsToBeReplaced())
                    {
                        Destroy(item.RepresentedItem);
                        Destroy(item);
                        return;  
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }  
        } else {
            if (Backpack.TryRemoveItem(attachedInventoryItem.RepresentedItem))
            {
                UnassignVisual();
            } else {
                return;
            }
        }
    }
}

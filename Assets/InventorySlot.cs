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
    public ItemContainer attachedItemContainer;
    public Backpack Backpack;
    public Equipment Equipment;

    public void AssignVisual(ItemContainer item)
    {
        attachedItemContainer = item;
        attachedItemContainer.transform.SetParent(transform);
        attachedItemContainer.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        attachedItemContainer.transform.localScale = Vector3.one;
        attachedItemContainer.slotAttachedTo = this;
    }

    public void UnassignVisual()
    {
        attachedItemContainer.slotAttachedTo = null;
        attachedItemContainer = null;
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

    public void Assign(ItemContainer itemContainer)
    {
        if (IsEquipmentSlot())
        {
            if (attachedItemContainer == null || attachedItemContainer.RepresentedItem.IsToBeReplaced())
            {
                switch (slotType)
                {
                    case SlotType.EquipmentWeaponPrimary:
                        // Remove the temporary equipment item first
                        if (Equipment.WeaponItem != null){
                            if (Equipment.WeaponItem.GetComponent<Weapon>().IsReady ||
                                Equipment.WeaponItem.GetComponent<Weapon>().IsAttacking)
                            {
                                    itemContainer.CancelSelection();
                                    return;
                            }
                            if (Equipment.WeaponItem.IsToBeReplaced())
                            {
                                Destroy(Equipment.WeaponItem);  
                            }
                        }
                        if (itemContainer.RepresentedItem.GetComponent<Weapon>().IsTwoHanded == false)
                        {
                            // TODO: Special checkif two handed.
                        }
                        if (Equipment.WeaponItem == null)
                        {
                            if (Equipment.TryEquip(itemContainer.RepresentedItem)){
                                if (itemContainer.slotAttachedTo != null){
                                    itemContainer.slotAttachedTo.Unassign();
                                }
                                AssignVisual(itemContainer);
                                itemContainer.isEquipped = true;
                                itemContainer.equippedBy = Equipment.Owner;
                                return;
                            } else {
                                itemContainer.CancelSelection();
                                return;
                            }
                        }
                        return; 
                    default:
                        throw new NotImplementedException();
                }
            }
        } 
        else if (attachedItemContainer == null)
        {
            if (itemContainer.slotAttachedTo != null)
            {
                itemContainer.slotAttachedTo.Unassign();
            }
            if (Backpack.TryChangeItemPosition(itemContainer.RepresentedItem, backpackSlotIndex))
            {
                AssignVisual(itemContainer);
                itemContainer.isEquipped = false;
                itemContainer.equippedBy = null;
            }    
        } else {
            itemContainer.CancelSelection();
        }
    }

    public void Unassign()
    {
        if (IsEquipmentSlot())
        {
            switch (slotType)
            {
                case SlotType.EquipmentWeaponPrimary:
                    var item = attachedItemContainer;
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
            if (Backpack.TryRemoveItem(attachedItemContainer.RepresentedItem))
            {
                UnassignVisual();
            } else {
                return;
            }
        }
    }
}

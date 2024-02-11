using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventorySlot : InventorySlot
{
    public CharacterBackpack CharacterBackpack;
    public Equipment Equipment;

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

    public override void Assign(InventoryItemContainer itemContainer)
    {
        if (IsEquipmentSlot())
        {
            if (attachedInventoryItemContainer == null || attachedInventoryItemContainer.RepresentedItem.IsToBeReplaced())
            {
                switch (slotType)
                {
                    case SlotType.EquipmentWeaponPrimary:
                        // Remove the temporary equipment item first
                        if (Equipment.WeaponItem != null)
                        {
                            var weapon = Equipment.WeaponItem.GetComponent<Weapon>();

                            if (weapon.IsReady || weapon.IsAttacking)
                            {
                                itemContainer.CancelSelection();
                                return;
                            }

                            if (weapon.IsToBeReplaced())
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
                            if (Equipment.TryEquip(itemContainer.RepresentedItem))
                            {
                                if (itemContainer.slotAttachedTo != null)
                                {
                                    itemContainer.slotAttachedTo.Unassign();
                                }

                                AssignVisual(itemContainer);

                                itemContainer.isEquipped = true;
                                itemContainer.equippedBy = Equipment.Owner;
                            } 
                            else 
                            {
                                itemContainer.CancelSelection();
                                return;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        } 
        else if (attachedInventoryItemContainer == null)
        {
            if (itemContainer.slotAttachedTo != null)
            {
                itemContainer.slotAttachedTo.Unassign();
            }

            if (CharacterBackpack.TryChangeItemPosition(itemContainer.RepresentedItem, SlotIndex))
            {
                AssignVisual(itemContainer);

                itemContainer.isEquipped = false;
                itemContainer.equippedBy = null;
            }    
        } 
        else 
        {
            itemContainer.CancelSelection();
            return;
        }

        var args = new InventoryItemContainerSlotAttachmentArgs
        {
            InventoryItemContainer = itemContainer,
            Slot = this
        };
        OnInventoryItemContainerSlotAttachment?.Invoke(args);
    }

    public override void Unassign()
    {
        if (IsEquipmentSlot())
        {
            switch (slotType)
            {
                case SlotType.EquipmentWeaponPrimary:
                    var item = attachedInventoryItemContainer;
                    if (Equipment.TryUnequip(item.RepresentedItem.GetComponent<Weapon>()))
                    {
                        item.isEquipped = false;
                        item.equippedBy = null;
                        UnassignVisual();
                    } 
                    else 
                    {
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
        } 
        else 
        {
            if (CharacterBackpack.TryRemoveItem(attachedInventoryItemContainer.RepresentedItem))
            {
                UnassignVisual();
            } 
            else 
            {
                return;
            }
        }
    }
}

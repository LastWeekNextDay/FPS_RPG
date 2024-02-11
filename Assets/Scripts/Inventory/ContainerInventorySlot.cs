using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInventorySlot : InventorySlot
{
    public ContainerBackpack ContainerBackpack;

    public override void Assign(InventoryItemContainer itemContainer)
    {
        if (attachedInventoryItemContainer == null)
        {
            if (itemContainer.slotAttachedTo != null)
            {
                itemContainer.slotAttachedTo.Unassign();
            }

            if (ContainerBackpack.TryChangeItemPosition(itemContainer.RepresentedItem, SlotIndex))
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
        if (ContainerBackpack.TryRemoveItem(attachedInventoryItemContainer.RepresentedItem))
        {
            UnassignVisual();
        } 
        else 
        {
            return;
        }
    }
}

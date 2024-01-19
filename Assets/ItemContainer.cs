using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour
{
    public Item RepresentedItem { get; private set; }
    [NonSerialized] public bool isEquipped;
    [NonSerialized] public Character equippedBy;
    [NonSerialized] public bool isSelected;

    [NonSerialized] public InventorySlot slotAttachedTo;
    public Image imageRenderer;
    public Sprite DefaultIcon;
    [NonSerialized] public SlotType slotType;

    public static Action<MouseAttachArgs> OnMouseAttach;
    public static Action<MouseDetachArgs> OnMouseDetach;
    public static Action<CancelSelectionArgs> OnCancelSelection;
    public static Action<ItemContainerDestructionArgs> OnItemContainerDestruction;
    public static Action<DropArgs> OnDrop;
    public static Action<EquipArgs> OnEquip;
    public static Action<UnequipArgs> OnUnequip;

    private GameObject _canvasObject;
    private Character playerChar; 

    void Start()
    {
        _canvasObject = GameObject.Find("Canvas");
        var Player = GameObject.Find("Player");
        playerChar = Player.GetComponent<Character>();
    }

    public bool IsEquipment()
    {
        var isEquipment = false;
        switch (slotType)
        {
            case SlotType.EquipmentWeaponPrimary:
            case SlotType.EquipmentWeaponSecondary:
            case SlotType.EquipmentArmorHead:
            case SlotType.EquipmentArmorChest:
            case SlotType.EquipmentArmorLegs:
            case SlotType.EquipmentArmorFeet:
                isEquipment = true;
                break;
        }
        return isEquipment;
    }

    public void AttachItemToItemContainer(Item item)
    {
        RepresentedItem = item;
        SetActiveInWorld(false, parent: transform);

        Sprite image = item.GetComponent<Item>().itemIcon ?? DefaultIcon;
        imageRenderer.sprite = image;

        switch (item.ItemType)
        {
            case ItemType.Weapon:
                slotType = SlotType.EquipmentWeaponPrimary;
                break;
            case ItemType.Armor:
                throw new NotImplementedException();
            default:
                slotType = SlotType.Backpack;
                break;
        }
    }

    public void SetActiveInWorld(bool active, Vector3 pos = default, Transform parent = null)
    {
        if (RepresentedItem != null)
        {
            RepresentedItem.SetActiveInWorld(active, pos, parent);
            if (active)
            {
                if (RepresentedItem.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                }
                if (RepresentedItem.TryGetComponent<Collider>(out var collider))
                {
                    collider.isTrigger = false;
                }
            }
            else
            {
                if (RepresentedItem.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
                if (RepresentedItem.TryGetComponent<Collider>(out var collider))
                {
                    collider.isTrigger = true;
                }
            }
        }   
    }

    public void DestroyItemContainter()
    {
        if (RepresentedItem != null)
        {
            Character ownerOfItem = null;
            if (slotAttachedTo != null)
            {
                Character owner = null;
                if (slotAttachedTo.Backpack != null)
                {
                    owner = slotAttachedTo.Backpack.Owner;
                } else if (slotAttachedTo.Equipment != null)
                {
                    owner = slotAttachedTo.Equipment.Owner;
                }
                if (owner != null)
                {
                    if (owner.Backpack.TryGetItemIndex(RepresentedItem, out _) || equippedBy == owner)
                    {
                        ownerOfItem = owner;
                    }
                }
                slotAttachedTo.UnassignVisual();
            }
            if (ownerOfItem != null)
            {
                RepresentedItem.transform.SetParent(ownerOfItem.transform);
            } else {
                RepresentedItem.transform.SetParent(null);
            }
            RepresentedItem = null;
        }
        var args = new ItemContainerDestructionArgs
        {
            ItemContainer = this
        };
        OnItemContainerDestruction?.Invoke(args);
        Destroy(gameObject);
    }

    public void AttachToMouse()
    {
        if (RepresentedItem.IsToBeReplaced() || RepresentedItem.IsMovableFromSlot() == false)
        {
            return;
        }

        if (slotType == SlotType.EquipmentWeaponPrimary)
        {
            if (playerChar.Equipment.WeaponItem != null)
            {
                if (RepresentedItem == playerChar.Equipment.WeaponItem){
                    if (playerChar.Equipment.WeaponItem.GetComponent<Weapon>().IsAttacking ||
                    playerChar.Equipment.WeaponItem.GetComponent<Weapon>().IsReady)
                    {
                        return;
                    }
                }
            }
        }

        isSelected = true;

        if (slotAttachedTo != null)
        {
            if (slotAttachedTo.IsEquipmentSlot())
            {
                slotAttachedTo.Unassign();
            }
        }

        transform.SetParent(_canvasObject.transform);
        transform.SetSiblingIndex(transform.parent.childCount - 1);
        var args = new MouseAttachArgs
        {
            ItemContainer = this
        };
        OnMouseAttach?.Invoke(args);
    }

    public void FollowMouse()
    {
        if (RepresentedItem.IsToBeReplaced() || RepresentedItem.IsMovableFromSlot() == false)
        {
            return;
        }
        if (slotType == SlotType.EquipmentWeaponPrimary)
        {
            if (playerChar.Equipment.WeaponItem != null)
            {
                if (RepresentedItem == playerChar.Equipment.WeaponItem){
                    if (playerChar.Equipment.WeaponItem.GetComponent<Weapon>().IsAttacking ||
                    playerChar.Equipment.WeaponItem.GetComponent<Weapon>().IsReady)
                    {
                        return;
                    }
                }
            }
        }
        var mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }

    public void DetachFromMouse()
    {
        if (RepresentedItem.IsToBeReplaced() || RepresentedItem.IsMovableFromSlot() == false)
        {
            return;
        }
        if (slotType == SlotType.EquipmentWeaponPrimary)
        {
            if (playerChar.Equipment.WeaponItem != null)
            {
                if (RepresentedItem == playerChar.Equipment.WeaponItem){
                    if (playerChar.Equipment.WeaponItem.GetComponent<Weapon>().IsAttacking ||
                    playerChar.Equipment.WeaponItem.GetComponent<Weapon>().IsReady)
                    {
                        return;
                    }
                }
            }
        }

        var eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        var rayCaster = _canvasObject.GetComponent<GraphicRaycaster>();
        var pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        var hits = new List<RaycastResult>();
        rayCaster.Raycast(pointerEventData, hits);

        // Go through all hits in a hierarhical order
        // 1. Item Slot
        // 2. UI

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }
            if (hit.gameObject.layer == LayerMask.NameToLayer("ItemSlot"))
            {
                var slot = hit.gameObject.GetComponent<InventorySlot>();
                AssignToSlot(slot);
                isSelected = false;
                return;
            }
        }

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                continue;
            }
            if (hit.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                CancelSelection();
                return;
            }
        }

        var args = new MouseDetachArgs
        {
            ItemContainer = this
        };
        OnMouseDetach?.Invoke(args);

        Drop(playerChar, CalculateForwardPositionUsingCamera(playerChar.transform.position));
    }

    public void Drop(Character fromWho, Vector3 where)
    {
        if (equippedBy == fromWho)
        {
            fromWho.Equipment.TryUnequip(RepresentedItem);
        } else {
            fromWho.Backpack.TryRemoveItem(RepresentedItem);
        }
        SetActiveInWorld(true, where);
        var args = new DropArgs
        {
            Source = fromWho,
            Item = RepresentedItem,
            Position = where
        };
        OnDrop?.Invoke(args);
        DestroyItemContainter();
    }

    Vector3 CalculateForwardPositionUsingCamera(Vector3 fromWhere, float distance = 1f)
    {
        var mousePos = Input.mousePosition;
        var delta = Camera.main.transform.forward - Camera.main.transform.position;
        mousePos.z = delta.normalized.magnitude;
        var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        if (Vector3.Distance(fromWhere, worldPosition) > distance)
        {
            delta = worldPosition - Camera.main.transform.position;
            delta.Normalize();
            worldPosition = Camera.main.transform.position + delta;
        }
        return worldPosition;
    }

    public void CancelSelection()
    {
        if (isSelected == false)
        {
            return;
        }
        
        var player = GameObject.Find("Player");
        var playerChar = player.GetComponent<Character>();

        // If attached to slot, attach back, and if it's an equipment slot, equip the item back on
        // Else, try to add back to backpack, and if there is no space, drop in front of you
        
        bool itemReadded = false;
        if (slotAttachedTo != null)
        {
            if (slotAttachedTo.IsEquipmentSlot())
            {
                if (playerChar.Backpack.TryAddItem(RepresentedItem) == false)
                {
                    Drop(playerChar, CalculateForwardPositionUsingCamera(player.transform.position));
                } else {
                    itemReadded = true;   
                }
            } else {
                itemReadded = true;
            }
        } else {
            if (playerChar.Backpack.TryAddItem(RepresentedItem) == false)
            {
                Drop(playerChar, CalculateForwardPositionUsingCamera(player.transform.position));
            } else {
                itemReadded = true;
            }
        }

        var args = new CancelSelectionArgs
        {
            ItemContainer = this
        };
        OnCancelSelection?.Invoke(args); 

        if (itemReadded)
        {
            DestroyItemContainter();
        }
    }

    void AssignToSlot(InventorySlot slot)
    {
        if (slot != null)
        {
            slot.Assign(this);
        }
    }

    void UnassignFromSlot()
    {
        if (slotAttachedTo != null)
        {
            slotAttachedTo.Unassign();
        }
    }
}

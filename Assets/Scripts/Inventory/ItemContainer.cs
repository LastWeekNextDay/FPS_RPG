using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemContainer : MonoBehaviour
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

    public void AttachItemToInventoryItemContainer(Item item)
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

    public void DestroyInventoryItemContainter()
    {        
        if (RepresentedItem != null)
        {
            GameObject ownerOfItem = null;

            if (slotAttachedTo != null)
            {
                CharacterBackpack charB = null;
                ContainerBackpack contB = null;

                if (slotAttachedTo.slotType == SlotType.Container)
                {
                    var containerSlot = slotAttachedTo as ContainerInventorySlot;
                    contB = containerSlot.ContainerBackpack;
                } 
                else 
                {
                    var charSlot = slotAttachedTo as CharacterInventorySlot;
                    var backpack = charSlot.CharacterBackpack;
                    var equipment = charSlot.Equipment;

                    if (backpack != null)
                    {
                        charB = backpack;
                    } 
                    else if (equipment != null)
                    {
                        charB = equipment.Owner.CharacterBackpack;
                    }
                }

                if (charB != null)
                {
                    if (charB.TryGetItemIndex(RepresentedItem, out _) || equippedBy == charB.Owner)
                    {
                        ownerOfItem = charB.Owner.transform.gameObject;
                    }
                } 
                else if (contB != null)
                {
                    if (contB.TryGetItemIndex(RepresentedItem, out _))
                    {
                        ownerOfItem = contB.containerOf.gameObject;
                    }
                }

                slotAttachedTo.UnassignVisual();
            }

            if (ownerOfItem != null)
            {
                RepresentedItem.transform.SetParent(ownerOfItem.transform);
            } 
            else 
            {
                RepresentedItem.transform.SetParent(null);
            }

            RepresentedItem = null;
        }

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
                var weapon = playerChar.Equipment.WeaponItem;

                if (RepresentedItem == weapon)
                {
                    if (weapon.IsAttacking || weapon.IsReady)
                    {
                        return;
                    }
                }
            }
        }

        isSelected = true;

        if (slotType != SlotType.Container && slotType != SlotType.Backpack)
        {
            slotAttachedTo?.Unassign();
        }

        transform.SetParent(_canvasObject.transform);
        transform.SetSiblingIndex(transform.parent.childCount - 1);

        var args = new MouseAttachArgs
        {
            InventoryItemContainer = this
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
                var weapon = playerChar.Equipment.WeaponItem;

                if (RepresentedItem == weapon)
                {
                    if (weapon.IsAttacking || weapon.IsReady)
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
                var weapon = playerChar.Equipment.WeaponItem;

                if (RepresentedItem == weapon)
                {
                    if (weapon.IsAttacking || weapon.IsReady)
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
            InventoryItemContainer = this
        };
        OnMouseDetach?.Invoke(args);

        Drop(playerChar, CalculateForwardPositionUsingCamera(playerChar.transform.position));
    }

    public void Drop(Character fromWho, Vector3 where)
    {
        if (equippedBy == fromWho)
        {
            fromWho.Equipment.TryUnequip(RepresentedItem);
        } 
        else 
        {
            if (slotAttachedTo != null)
            {
                if (slotAttachedTo.slotType == SlotType.Container)
                {
                    var containerSlot = slotAttachedTo as ContainerInventorySlot;
                    containerSlot.ContainerBackpack.TryRemoveItem(RepresentedItem);
                } 
                else 
                {
                    var charSlot = slotAttachedTo as CharacterInventorySlot;
                    charSlot.CharacterBackpack.TryRemoveItem(RepresentedItem);
                }
            }
        }

        SetActiveInWorld(true, where);

        var args = new DropArgs
        {
            Source = fromWho,
            Item = RepresentedItem,
            Position = where
        };
        OnDrop?.Invoke(args);

        DestroyInventoryItemContainter();
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
            if (slotAttachedTo.slotType != SlotType.Container)
            {
                var charSlot = slotAttachedTo as CharacterInventorySlot;
                if (charSlot.IsEquipmentSlot())
                {
                    if (playerChar.Equipment.TryEquip(RepresentedItem) == false)
                    {
                        if (playerChar.CharacterBackpack.TryAddItem(RepresentedItem) == false)
                        {
                            Drop(playerChar, CalculateForwardPositionUsingCamera(player.transform.position));
                        } 
                        else 
                        {
                            itemReadded = true;  
                        }  
                    } 
                    else 
                    {
                        itemReadded = true;
                    }
                } 
                else 
                {
                    if (playerChar.CharacterBackpack.TryAddItem(RepresentedItem) == false)
                    {
                        Drop(playerChar, CalculateForwardPositionUsingCamera(player.transform.position));
                    } 
                    else 
                    {
                        itemReadded = true;  
                    }  
                }
            } 
            else
            {
                var containerSlot = slotAttachedTo as ContainerInventorySlot;
                if (containerSlot.ContainerBackpack.TryAddItem(RepresentedItem) == false)
                {
                    Drop(playerChar, CalculateForwardPositionUsingCamera(player.transform.position));
                } 
                else 
                {
                    itemReadded = true;
                }
            }  
        } 
        else 
        {
            if (playerChar.CharacterBackpack.TryAddItem(RepresentedItem) == false)
            {
                Drop(playerChar, CalculateForwardPositionUsingCamera(player.transform.position));
            } 
            else 
            {
                itemReadded = true;
            }
        }

        var args = new CancelSelectionArgs
        {
            InventoryItemContainer = this
        };   

        if (itemReadded)
        {
            slotAttachedTo?.UnassignVisual();
            OnCancelSelection?.Invoke(args);
            DestroyInventoryItemContainter();
        } 
        else 
        {
            OnCancelSelection?.Invoke(args);
        }
    }

    void AssignToSlot(InventorySlot slot)
    {
        slot?.Assign(this);
    }

    void UnassignFromSlot()
    {
        slotAttachedTo?.Unassign();
    }
}

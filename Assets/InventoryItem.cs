using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Item RepresentedItem { get; private set; }
    [NonSerialized] public bool isEquipped;
    [NonSerialized] public bool isSelected;

    [NonSerialized] public InventorySlot slotAttachedTo;
    public Image imageRenderer;
    public Sprite DefaultIcon;
    [NonSerialized] public SlotType slotType;

    public Action OnMouseAttach;
    public Action<InventorySlot> OnAttachToSlot;
    public Action OnCancelSelection;
    public Action<Character, Vector3> OnDrop;
    public Action<Character, InventorySlot> OnEquip;
    public Action<Character, InventorySlot> OnUnequip;

    private GameObject _canvasObject;
    private Character playerChar;

    void Start()
    {
        _canvasObject = GameObject.Find("Canvas");
        var Player = GameObject.Find("Player");
        playerChar = Player.GetComponent<Character>();

        OnMouseAttach += () => 
        {
            AudioManager.Instance.PlayPickupInventoryItemSound();
            var itemType = RepresentedItem.ItemType;
            switch(itemType)
            {
                case ItemType.Weapon:
                    if (playerChar.Equipment.WeaponInventoryItem != null)
                    {
                        if (playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsAttacking ||
                            playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsReady)
                        {
                            CancelSelection();
                            return;
                        }
                    }
                    break;    
            }
            if (slotAttachedTo != null)
            {
                if (slotAttachedTo.IsEquipmentSlot())
                {
                    UnequipFrom(playerChar, slotAttachedTo);
                }
            }
            transform.SetParent(_canvasObject.transform);
        };

        OnAttachToSlot += (slot) => 
        {
            AudioManager.Instance.PlayPickupInventoryItemSound();

            if (slot != null)
            {
                slot.Attach(this);
            }
        };

        OnCancelSelection += () => 
        {
            AudioManager.Instance.PlayCancelInventoryItemPickupSound();
            var player = GameObject.Find("Player");
            var playerChar = player.GetComponent<Character>();

            // If attached to slot, attach back, and if it's an equipment slot, equip the item back on
            // Else, try to add back to backpack, and if there is no space, drop in front of you
            if (slotAttachedTo != null)
            {
                slotAttachedTo.ForceAttach(this);
                if (slotAttachedTo.IsEquipmentSlot())
                {
                    EquipOn(slotAttachedTo.Equipment.Owner, slotAttachedTo);
                }
            } else {
                if (playerChar.Backpack.TryAddItem(this) == false)
                {
                    Drop(playerChar);
                    UIManager.Instance.RefreshBackpackItemsUI(UIManager.Instance.GetPlayerBackpackUI(),
                        playerChar.Backpack);
                } else {
                    UIManager.Instance.RefreshBackpackItemsUI(UIManager.Instance.GetPlayerBackpackUI(),
                        playerChar.Backpack);
                }
            }             
        };

        OnDrop += (character, pos) => 
        {
            AudioManager.Instance.PlayDropItemSound();
            if (slotAttachedTo != null)
            {
                if (slotAttachedTo.IsEquipmentSlot())
                {
                    UnequipFrom(character, slotAttachedTo);
                }
            }

            var rb = RepresentedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
            var collider = RepresentedItem.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }

            SetActiveInWorld(true, pos);
            DestroyInventoryItem();
            character.Backpack.TryRemoveItem(this);
        }; 

        OnEquip += (character, slot) => 
        {
            if (character.Equipment.TryEquip(this) == false)
            {
                CancelSelection();
                return;
            }
            slot.ForceAttach(this);
            character.Backpack.TryRemoveItem(this);
        };

        OnUnequip += (character, slot) => 
        {
            if (character.Equipment.TryUnequip(this) == false)
            {
                CancelSelection();
                return;
            }
            slotAttachedTo.ForceDetach();
        };
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

    public void AttachItemAsInventoryItem(Item item)
    {
        RepresentedItem = item;
        item.transform.SetParent(transform);
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        item.transform.localScale = Vector3.one;
        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        var collider = item.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        item.gameObject.SetActive(false);

        Sprite image = null;
        if (item.GetComponent<Item>() != null)
        {
            image = item.GetComponent<Item>().itemIcon;
        }
        if (image == null)
        {
            image = DefaultIcon;
        }
        imageRenderer.sprite = image;

        switch (item.ItemType)
        {
            case ItemType.Weapon:
                slotType = SlotType.EquipmentWeaponPrimary;
                break;
            default:
                slotType = SlotType.Backpack;
                break;
        }
    }

    public void SetActiveInWorld(bool active, Vector3 pos = default, Transform parent = null)
    {
        if (RepresentedItem != null)
        {
            if (active)
            {
                RepresentedItem.transform.position = pos;
            }
            else
            {
                RepresentedItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
            RepresentedItem.gameObject.SetActive(active);
            RepresentedItem.transform.SetParent(parent);
        }   
    }

    public void DestroyInventoryItem()
    {
        if (RepresentedItem != null)
        {
            RepresentedItem.transform.SetParent(null);
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
            if (playerChar.Equipment.WeaponInventoryItem != null)
            {
                if (RepresentedItem == playerChar.Equipment.WeaponInventoryItem){
                    if (playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsAttacking ||
                    playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsReady)
                    {
                        return;
                    }
                }
            }
        }
        isSelected = true;
        transform.SetParent(_canvasObject.transform);
        transform.SetSiblingIndex(transform.parent.childCount - 1);
        OnMouseAttach?.Invoke();
    }

    public void FollowMouse()
    {
        if (RepresentedItem.IsToBeReplaced() || RepresentedItem.IsMovableFromSlot() == false)
        {
            return;
        }
        if (slotType == SlotType.EquipmentWeaponPrimary)
        {
            if (playerChar.Equipment.WeaponInventoryItem != null)
            {
                if (RepresentedItem == playerChar.Equipment.WeaponInventoryItem){
                    if (playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsAttacking ||
                    playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsReady)
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
            if (playerChar.Equipment.WeaponInventoryItem != null)
            {
                if (RepresentedItem == playerChar.Equipment.WeaponInventoryItem){
                    if (playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsAttacking ||
                    playerChar.Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().IsReady)
                    {
                        return;
                    }
                }
            }
        }

        isSelected = false;
        transform.SetParent(_canvasObject.transform);
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
            if (hit.gameObject.layer == LayerMask.NameToLayer("ItemSlot")){
                var slot = hit.gameObject.GetComponent<InventorySlot>();
                OnAttachToSlot?.Invoke(slot);
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

        var Player = GameObject.Find("Player");
        Drop(Player.GetComponent<Character>());
    }

    public void Drop(Character fromWho)
    {
        OnDrop?.Invoke(fromWho, CalculatePositionForDropOfItem(fromWho.transform.position));
    }

    Vector3 CalculatePositionForDropOfItem(Vector3 fromWhere)
    {
        var mousePos = Input.mousePosition;
        var delta = Camera.main.transform.forward - Camera.main.transform.position;
        mousePos.z = delta.normalized.magnitude;
        var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        if (Vector3.Distance(fromWhere, worldPosition) > 1f)
        {
            delta = worldPosition - Camera.main.transform.position;
            delta.Normalize();
            worldPosition = Camera.main.transform.position + delta;
        }
        return worldPosition;
    }

    public void CancelSelection()
    {
        OnCancelSelection?.Invoke();
    }

    public void EquipOn(Character character, InventorySlot slot)
    {
        OnEquip?.Invoke(character, slot);
    }

    public void UnequipFrom(Character character, InventorySlot slot)
    {
        OnUnequip?.Invoke(character, slot);
    }
}

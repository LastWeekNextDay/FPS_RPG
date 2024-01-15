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

    public Action OnMouseAttach;
    public Action<InventorySlot> OnAttachToSlot;
    public Action OnCancelSelection;
    public Action<Character, Vector3> OnDrop;
    public Action<InventorySlot> OnEquip;
    public Action<InventorySlot> OnUnequip;

    private GameObject _canvasObject;

    void Start()
    {
        _canvasObject = GameObject.Find("Canvas");
        OnMouseAttach += () => 
        {
            AudioManager.Instance.PlayPickupInventoryItemSound();
            if (slotAttachedTo != null)
            {
                if (slotAttachedTo.isEquipmentSlot)
                {
                    OnUnequip?.Invoke(slotAttachedTo);
                }
            }
            transform.SetParent(_canvasObject.transform);
        };

        OnAttachToSlot += (slot) => 
        {
            AudioManager.Instance.PlayPickupInventoryItemSound();
            var Player = GameObject.Find("Player");
            if (slot != null)
            {
                if (slot.invItem == null || slot.invItem.RepresentedItem.IsToBeReplaced())
                {
                    if (slot.isEquipmentSlot)
                    {
                        switch (slot.name)
                        {
                            case "Weapon1Holder":
                                if (RepresentedItem.itemType != ItemType.Weapon)
                                {
                                    OnCancelSelection?.Invoke();
                                    return;
                                }
                                var character = Player.GetComponent<Character>();
                                if (character.Equipment.WeaponInventoryItem != null){
                                    if (character.Equipment.WeaponInventoryItem.RepresentedItem.IsToBeReplaced())
                                    {
                                        Destroy(Player.GetComponent<Character>().Equipment.WeaponInventoryItem.RepresentedItem);
                                        Destroy(Player.GetComponent<Character>().Equipment.WeaponInventoryItem);  
                                    }
                                }
                                if (RepresentedItem.GetComponent<Weapon>().IsTwoHanded == false)
                                {
                                    // TODO: Special checkif two handed.
                                }
                                if (character.Equipment.WeaponInventoryItem == null)
                                {
                                    if (slotAttachedTo != null){
                                        slotAttachedTo.Detach();
                                    }
                                    OnEquip?.Invoke(slot);
                                    return;
                                }
                                return; 
                            default:
                                OnCancelSelection?.Invoke();
                                return;
                        }
                    } else {
                        if (slotAttachedTo != null){
                            if (slotAttachedTo.isEquipmentSlot)
                            {
                                switch (slotAttachedTo.name)
                                {
                                    case "Weapon1Holder":
                                        var invItem = Player.GetComponent<Character>().Equipment.WeaponInventoryItem;
                                        OnUnequip?.Invoke(slotAttachedTo);
                                        if (invItem.RepresentedItem.IsToBeReplaced())
                                        {
                                            Destroy(invItem.RepresentedItem);
                                            Destroy(invItem);
                                            return;  
                                        }
                                        break;
                                }
                            } else {
                                slotAttachedTo.Detach();
                            }
                        }
                        slot.Attach(this);
                        Player.GetComponent<Character>().Backpack.TryChangeItemPosition(
                            this, UIManager.Instance.GetBackpackSlotUINumber(UIManager.Instance.GetPlayerBackpackUI(), slot));
                    }
                    return; 
                } else {
                    OnCancelSelection?.Invoke();
                }
            }
        };

        OnCancelSelection += () => 
        {
            AudioManager.Instance.PlayCancelInventoryItemPickupSound();
            var Player = GameObject.Find("Player");
            if (slotAttachedTo != null)
            {
                slotAttachedTo.Attach(this);
                if (slotAttachedTo.isEquipmentSlot)
                {
                    OnEquip?.Invoke(slotAttachedTo);
                }
            } else {
                if (Player.GetComponent<Character>().Backpack.TryAddItem(this) == false)
                {
                    Drop(Player);
                    UIManager.Instance.RefreshBackpackItemsUI(UIManager.Instance.GetPlayerBackpackUI(),
                        Player.GetComponent<Character>().Backpack);
                } else {
                    UIManager.Instance.RefreshBackpackItemsUI(UIManager.Instance.GetPlayerBackpackUI(),
                        Player.GetComponent<Character>().Backpack);
                }
            }             
        };

        OnDrop += (character, pos) => 
        {
            AudioManager.Instance.PlayDropItemSound();
            if (slotAttachedTo != null)
            {
                if (slotAttachedTo.isEquipmentSlot)
                {
                    OnUnequip?.Invoke(slotAttachedTo);
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

        OnEquip += (slot) => 
        {
            var Player = GameObject.Find("Player");
            var character = Player.GetComponent<Character>();
            if (RepresentedItem.GetComponent<Weapon>() != null)
            {
                if (character.Equipment.TryEquipWeapon(this) == false)
                {
                    OnCancelSelection?.Invoke();
                    return;
                }
            }
            slot.Attach(this);
            Player.GetComponent<Character>().Backpack.TryRemoveItem(this);
        };

        OnUnequip += (slot) => 
        {
            var Player = GameObject.Find("Player");
            var character = Player.GetComponent<Character>();
            if (RepresentedItem.GetComponent<Weapon>() != null)
            {
                if (character.Equipment.TryUnequipWeapon(this) == false)
                {
                    OnCancelSelection?.Invoke();
                    return;
                }
            }
            slotAttachedTo.Detach();
        };
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
        var mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }

    public void DetachFromMouse()
    {
        if (RepresentedItem.IsToBeReplaced() || RepresentedItem.IsMovableFromSlot() == false)
        {
            return;
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
                OnCancelSelection?.Invoke();
                return;
            }
        }

        var Player = GameObject.Find("Player");
        Drop(Player);
    }

    public void Drop(GameObject fromWho)
    {
        OnDrop?.Invoke(fromWho.GetComponent<Character>(), CalculatePositionForDropOfItem(fromWho.transform.position));
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
}

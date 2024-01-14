using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Miscellaneous
}

public class InventoryItem : MonoBehaviour
{
    [NonSerialized] public ItemType itemType;
    public GameObject RepresentedItem { get; private set; }
    [NonSerialized] public bool isEquipped;
    [NonSerialized] public bool isSelected;
    [NonSerialized] public InventorySlot slotAttachedTo;
    public Image imageRenderer;
    public Sprite DefaultIcon;
    private GameObject _canvasObject;
    public Action OnMouseAttach;
    public Action<InventorySlot> OnAttachToSlot;
    public Action OnCancelSelection;
    public Action<Vector3> OnDrop;
    public Action<InventorySlot> OnEquip;
    public Action<InventorySlot> OnUnequip;

    private GameObject _player;
    private GameObject Player {
        get {
            if (_player == null)
            {
                _player = GameObject.Find("Player");
            }
            return _player;
        }
    }

    void Start()
    {
        _canvasObject = GameObject.Find("Canvas");
        OnMouseAttach += () => AudioManager.Instance.PlayPickupInventoryItemSound();

        OnAttachToSlot += (slot) => 
        {
            AudioManager.Instance.PlayPickupInventoryItemSound();
            if (slot != null)
            {
                if (slot.item == null || slot.item.RepresentedItem.GetComponent<Weapon>().weaponName == "Fists")
                {
                    if (slot.isEquipmentSlot){
                        if (slot.name == "Weapon1Holder")
                        {
                            var weapon = RepresentedItem.GetComponent<Weapon>();
                            if (weapon == null)
                            {
                                OnCancelSelection?.Invoke();
                                return;
                            }
                            if (Player.GetComponent<Character>().Equipment.WeaponInventoryItem != null){
                                if (Player.GetComponent<Character>().Equipment.WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().weaponName == "Fists")
                                {
                                    Destroy(Player.GetComponent<Character>().Equipment.WeaponInventoryItem.RepresentedItem);
                                    Destroy(Player.GetComponent<Character>().Equipment.WeaponInventoryItem);
                                }
                            }
                            if (Player.GetComponent<Character>().Equipment.WeaponInventoryItem == null)
                            {
                                OnEquip?.Invoke(slot);
                                return;
                            }
                            if (weapon.IsTwoHanded == false)
                            {
                                // TODO: Special checkif two handed.
                            }
                            return; 
                        }  
                    } else {
                        if (slotAttachedTo.isEquipmentSlot)
                        {
                            OnUnequip?.Invoke(slotAttachedTo);
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
            slotAttachedTo.Attach(this);
        };

        OnDrop += (pos) => 
        {
            AudioManager.Instance.PlayDropItemSound();
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
            Player.GetComponent<Character>().Backpack.TryRemoveItem(this);
        }; 

        OnEquip += (slot) => 
        {
            var character = Player.GetComponent<Character>();
            if (character.Equipment.TryEquipWeapon(this) == false)
            {
                OnCancelSelection?.Invoke();
                return;
            }
            slotAttachedTo.Detach();
            slot.Attach(this);
            Player.GetComponent<Character>().Backpack.TryRemoveItem(this);
        };

        OnUnequip += (slot) => 
        {
            var character = Player.GetComponent<Character>();
            if (character.Equipment.TryUnequipWeapon(this) == false)
            {
                OnCancelSelection?.Invoke();
                return;
            }
            slotAttachedTo.Detach();
        };
    }

    public void AttachObjectAsInventoryItem(GameObject itemObject)
    {
        RepresentedItem = itemObject;
        itemObject.transform.SetParent(transform);
        itemObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        itemObject.transform.localScale = Vector3.one;
        var rb = itemObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        var collider = itemObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        itemObject.SetActive(false);

        Sprite image = null;
        if (itemObject.GetComponent<Weapon>() != null)
        {
            itemType = ItemType.Weapon;
            image = itemObject.GetComponent<Weapon>().itemIcon;
        } else {
            itemType = ItemType.Miscellaneous;
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
            RepresentedItem.SetActive(active);
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
        isSelected = true;
        transform.SetParent(_canvasObject.transform);
        transform.SetSiblingIndex(transform.parent.childCount - 1);
        OnMouseAttach?.Invoke();
    }

    public void FollowMouse()
    {
        var mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }

    public void DetachFromMouse()
    {
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
            if (hit.gameObject.layer == LayerMask.NameToLayer("UI")){
                OnCancelSelection?.Invoke();
                return;
            }
        }

        var mousePos = Input.mousePosition;
        var delta = Camera.main.transform.forward - Camera.main.transform.position;
        mousePos.z = delta.normalized.magnitude;
        var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        if (Vector3.Distance(Player.transform.position, worldPosition) > 1f)
        {
            delta = worldPosition - Camera.main.transform.position;
            delta.Normalize();
            worldPosition = Camera.main.transform.position + delta;
        }

        OnDrop?.Invoke(worldPosition);
    }
}

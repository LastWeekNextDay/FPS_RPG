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
    public GameObject ItemObject {get; private set;}
    [NonSerialized] public bool isEquipped;
    [NonSerialized] public bool isSelected;
    [NonSerialized] public ItemSlot slotAttachedTo;
    public Image imageRenderer;
    public Sprite DefaultIcon;
    private GameObject _canvasObject;
    public Action OnMouseAttach;
    public Action OnAttachToSlot;
    public Action OnCancelSelection;
    public Action OnDrop;

    void Start()
    {
        _canvasObject = GameObject.Find("Canvas");
        OnMouseAttach += () => AudioManager.Instance.PlayPickupInventoryItemSound();
        OnAttachToSlot += () => AudioManager.Instance.PlayPickupInventoryItemSound();
        OnCancelSelection += () => AudioManager.Instance.PlayCancelInventoryItemPickupSound();
        OnDrop += () => AudioManager.Instance.PlayDropItemSound();
    }

    public void AttachObjectAsInventoryItem(GameObject itemObject)
    {
        ItemObject = itemObject;
        itemObject.transform.SetParent(transform);
        itemObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        itemObject.transform.localScale = Vector3.one;
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

    public void SetActiveInWorld(bool active, Vector3 pos = default)
    {
        if (ItemObject != null)
        {
            if (active)
            {
                ItemObject.transform.position = pos;
            }
            else
            {
                ItemObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
            ItemObject.SetActive(active);
        }   
    }

    public void DestroyInventoryItem()
    {
        if (ItemObject != null)
        {
            ItemObject.transform.SetParent(null);
            ItemObject = null;
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
        var player = GameObject.Find("Player");
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
                OnAttachToSlot?.Invoke();
                var slot = hit.gameObject.GetComponent<ItemSlot>();
                if (slot != null)
                {
                    if (slot.item == null)
                    {
                        slotAttachedTo.Detach();
                        slot.Attach(this);
                        player.GetComponent<Character>().backpack.TryChangeItemPosition(
                            this, UIManager.Instance.GetSlotUINumber(UIManager.Instance.GetPlayerBackpackUI(), slot));
                        return;
                    } else {
                        slotAttachedTo.Attach(this);
                    }
                }
                return;
            }
            if (hit.gameObject.layer == LayerMask.NameToLayer("UI")){
                OnCancelSelection?.Invoke();
                slotAttachedTo.Attach(this);
                return;
            }
        }
        var mousePos = Input.mousePosition;
        var delta = Camera.main.transform.forward - Camera.main.transform.position;
        mousePos.z = delta.normalized.magnitude;
        var worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        if (Vector3.Distance(player.transform.position, worldPosition) > 1f)
        {
            delta = worldPosition - Camera.main.transform.position;
            delta.Normalize();
            worldPosition = Camera.main.transform.position + delta;
        }
        OnDrop?.Invoke();
        SetActiveInWorld(true, worldPosition);
        DestroyInventoryItem();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject inventoryItemPrefab;

    private UnityEngine.UI.Slider _healthBar;
    private UnityEngine.UI.Slider _invHealthBar;
    private UnityEngine.UI.Slider _manaBar;
    private UnityEngine.UI.Slider _invManaBar;
    private UnityEngine.UI.Slider _energyBar;
    private UnityEngine.UI.Slider _invEnergyBar;
    private GameObject _inventoryPanel;
    private TMPro.TextMeshProUGUI _invStrengthNumber;
    private TMPro.TextMeshProUGUI _invAgilityNumber;
    private TMPro.TextMeshProUGUI _invEnduranceNumber;
    private TMPro.TextMeshProUGUI _invIntelligenceNumber;
    private TMPro.TextMeshProUGUI _invHealthNumber;
    private TMPro.TextMeshProUGUI _invManaNumber;
    private TMPro.TextMeshProUGUI _invEnergyNumber;
    private TMPro.TextMeshProUGUI _invMaxHealthNumber;
    private TMPro.TextMeshProUGUI _invMaxManaNumber;
    private TMPro.TextMeshProUGUI _invMaxEnergyNumber;
    private TMPro.TextMeshProUGUI _invSpeedNumber;
    private TMPro.TextMeshProUGUI _invJumpForceNumber;
    private TMPro.TextMeshProUGUI _invHealthRegenNumber;
    private TMPro.TextMeshProUGUI _invManaRegenNumber;
    private TMPro.TextMeshProUGUI _invEnergyRegenNumber;

    private Character _character;
    public bool IsInventoryOpen {
        get {
            if (UnityEssential.TryFindObject("Inventory", out _inventoryPanel))
            {
                return _inventoryPanel.activeSelf;
            } else {
                return false;
            }
        } 
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (_character == null)
        {
            _character = GameObject.FindWithTag("Player").GetComponent<Character>();
        } else {
            UpdateInventoryStats();
            UpdateHealthBar();
            UpdateManaBar();
            UpdateEnergyBar();
        }
    }

    void Start()
    {
        AllowCursor(false);
    }

    public void AllowCursor(bool b)
    {
        if (b) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void CursorVisible(bool b)
    {
        Cursor.visible = b;
    }

    public void ResetCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }

    void UpdateInventoryStats()
    {
        if(IsInventoryOpen)
        {
            if (_invStrengthNumber == null) 
            {
                if (UnityEssential.TryFindObject("StrengthNumber", out GameObject strengthNumber, false))
                {
                    _invStrengthNumber = strengthNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invStrengthNumber.text = _character.Strength.ToString();
            }

            if (_invAgilityNumber == null)
            {
                if (UnityEssential.TryFindObject("AgilityNumber", out GameObject agilityNumber, false))
                {
                    _invAgilityNumber = agilityNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invAgilityNumber.text = _character.Agility.ToString();
            }

            if (_invEnduranceNumber == null)
            {
                if (UnityEssential.TryFindObject("EnduranceNumber", out GameObject enduranceNumber, false))
                {
                    _invEnduranceNumber = enduranceNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invEnduranceNumber.text = _character.Endurance.ToString();
            }

            if (_invIntelligenceNumber == null)
            {
                if (UnityEssential.TryFindObject("IntelligenceNumber", out GameObject intelligenceNumber, false))
                {
                    _invIntelligenceNumber = intelligenceNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invIntelligenceNumber.text = _character.Intelligence.ToString();
            }

            if (_invHealthNumber == null)
            {
                if (UnityEssential.TryFindObject("HealthNumber", out GameObject healthNumber, false))
                {
                    _invHealthNumber = healthNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invHealthNumber.text = _character.Health.ToString();
            }

            if (_invManaNumber == null)
            {
                if (UnityEssential.TryFindObject("ManaNumber", out GameObject manaNumber, false))
                {
                    _invManaNumber = manaNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invManaNumber.text = _character.Mana.ToString();
            }

            if (_invEnergyNumber == null)
            {
                if (UnityEssential.TryFindObject("EnergyNumber", out GameObject energyNumber, false))
                {
                    _invEnergyNumber = energyNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invEnergyNumber.text = _character.Energy.ToString();
            }

            if (_invMaxHealthNumber == null)
            {
                if (UnityEssential.TryFindObject("MaxHealthNumber", out GameObject maxHealthNumber, false))
                {
                    _invMaxHealthNumber = maxHealthNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invMaxHealthNumber.text = _character.MaxHealth.ToString();
            }

            if (_invMaxManaNumber == null)
            {
                if (UnityEssential.TryFindObject("MaxManaNumber", out GameObject maxManaNumber, false))
                {
                    _invMaxManaNumber = maxManaNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invMaxManaNumber.text = _character.MaxMana.ToString();
            }

            if (_invMaxEnergyNumber == null)
            {
                if (UnityEssential.TryFindObject("MaxEnergyNumber", out GameObject maxEnergyNumber, false))
                {
                    _invMaxEnergyNumber = maxEnergyNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invMaxEnergyNumber.text = _character.MaxEnergy.ToString();
            }

            if (_invSpeedNumber == null)
            {
                if (UnityEssential.TryFindObject("SpeedNumber", out GameObject speedNumber, false))
                {
                    _invSpeedNumber = speedNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invSpeedNumber.text = _character.Speed.ToString();
            }

            if (_invJumpForceNumber == null)
            {
                if (UnityEssential.TryFindObject("JumpForceNumber", out GameObject jumpForceNumber, false))
                {
                    _invJumpForceNumber = jumpForceNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invJumpForceNumber.text = _character.JumpForce.ToString();
            }

            if (_invHealthRegenNumber == null)
            {
                if (UnityEssential.TryFindObject("HealthRegenNumber", out GameObject healthRegenNumber, false))
                {
                    _invHealthRegenNumber = healthRegenNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invHealthRegenNumber.text = _character.HealthRegen.ToString();
            }

            if (_invManaRegenNumber == null)
            {
                if (UnityEssential.TryFindObject("ManaRegenNumber", out GameObject manaRegenNumber, false))
                {
                    _invManaRegenNumber = manaRegenNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invManaRegenNumber.text = _character.ManaRegen.ToString();
            }

            if (_invEnergyRegenNumber == null)
            {
                if (UnityEssential.TryFindObject("EnergyRegenNumber", out GameObject energyRegenNumber, false))
                {
                    _invEnergyRegenNumber = energyRegenNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invEnergyRegenNumber.text = _character.EnergyRegen.ToString();
            }
        }
    }

    void UpdateHealthBar()
    {
        if (_healthBar == null)
        {
            if (UnityEssential.TryFindObject("HealthBar", out GameObject healthBar, false))
            {
                _healthBar = healthBar.GetComponentInChildren<UnityEngine.UI.Slider>();
            }
        } else {
            _healthBar.value = _character.Health / _character.MaxHealth;
        }
        if (IsInventoryOpen)
        {
            if (_invHealthBar == null)
            {
                if (UnityEssential.TryFindObject("InvHealthBar", out GameObject invHealthBar, false))
                {
                    _invHealthBar = invHealthBar.GetComponentInChildren<UnityEngine.UI.Slider>();
                }
            } else {
                _invHealthBar.value = _character.Health / _character.MaxHealth;
            }
        }
    }

    void UpdateManaBar()
    {
        if (_manaBar == null)
        {
            if (UnityEssential.TryFindObject("ManaBar", out GameObject manaBar, false))
            {
               _manaBar = manaBar.GetComponentInChildren<UnityEngine.UI.Slider>();
            }
        } else {
            _manaBar.value = _character.Mana / _character.MaxMana;
        }
        if (IsInventoryOpen)
        {
            if (_invManaBar == null)
            {
                if (UnityEssential.TryFindObject("InvManaBar", out GameObject invManaBar, false))
                {
                    _invManaBar = invManaBar.GetComponentInChildren<UnityEngine.UI.Slider>();
                }
            } else {
                _invManaBar.value = _character.Mana / _character.MaxMana;
            }
        }
    }

    void UpdateEnergyBar()
    {
        if (_energyBar == null)
        {
            if (UnityEssential.TryFindObject("EnergyBar", out GameObject EnergyBar, false))
            {
                _energyBar = EnergyBar.GetComponentInChildren<UnityEngine.UI.Slider>();
            }
        } else {
            _energyBar.value = _character.Energy / _character.MaxEnergy;
        }
        if (IsInventoryOpen)
        {
            if (_invEnergyBar == null)
            {
                if (UnityEssential.TryFindObject("InvEnergyBar", out GameObject invEnergyBar, false))
                {
                    _invEnergyBar = invEnergyBar.GetComponentInChildren<UnityEngine.UI.Slider>();
                }
            } else {
                _invEnergyBar.value = _character.Energy / _character.MaxEnergy;
            }
        }
    }

    public void ToggleInventory()
    {
        if (_character == null)
        {
            if (UnityEssential.TryFindObject("Player", out GameObject charObj, false))
            {
                _character = charObj.GetComponent<Character>();
            }
        } else {
            if (_inventoryPanel == null)
            {
                if (UnityEssential.TryFindObject("Inventory", out _inventoryPanel))
                {
                    InventoryActivate(GetPlayerBackpackUI(), !_inventoryPanel.activeSelf);
                }
            } else {
                InventoryActivate(GetPlayerBackpackUI(), !_inventoryPanel.activeSelf);
            }
        }
    }

    void InventoryActivate(GameObject backpack, bool t)
    {
        if (t)
        {
            _inventoryPanel.SetActive(t);
            RefreshBackpackItemsUI(backpack, _character.backpack);
        } else {
            DetachAllItemsFromSlots(backpack);
            _inventoryPanel.SetActive(t);
        }
    }

    public GameObject GetPlayerBackpackUI()
    {
        if (_inventoryPanel == null)
        {
            if (UnityEssential.TryFindObject("Inventory", out _inventoryPanel))
            {
                if (UnityEssential.TryFindObjectInChildren(_inventoryPanel, "BackpackBase", out GameObject backpack))
                {
                    return backpack;
                }
            }
        } else {
            if (UnityEssential.TryFindObjectInChildren(_inventoryPanel, "BackpackBase", out GameObject backpack))
            {
                return backpack;
            }
        }
        return null;
    }

    public ItemSlot GetFirstAvailableSlotUI(GameObject backpackUI)
    {
        if (backpackUI == null)
        {
            return null;
        }
        var rows = new List<GameObject>();
        foreach (Transform child in backpackUI.transform)
        {
            rows.Add(child.gameObject);
        }
        foreach (var row in rows)
        {
            foreach (Transform child in row.transform)
            {
                var slot = child.gameObject.GetComponent<ItemSlot>();
                if (slot.item == null)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    public void RefreshBackpackItemsUI(GameObject backpackUI, Backpack backpack)
    {
        if (backpackUI == null || backpack == null)
        {
            return;
        }
        var rows = new List<GameObject>();
        int UIIndex = 0;
        foreach (Transform child in backpackUI.transform)
        {
            rows.Add(child.gameObject);
        }
        foreach (var row in rows)
        {
            foreach (Transform child in row.transform)
            {
                var slot = child.gameObject.GetComponent<ItemSlot>();
                slot.backpack = backpack;
                if (backpack.items[UIIndex] != null)
                {
                    if (slot.item == null)
                    {
                        slot.Attach(backpack.items[UIIndex]);
                    }
                }
                else if (backpack.items[UIIndex] == null)
                {
                    if (slot.item != null)
                    {
                        slot.Detach();
                    }
                }
                UIIndex++;
            }
        }
    }

    void DetachAllItemsFromSlots(GameObject backpackUI)
    {
        if (backpackUI == null)
        {
            return;
        }
        var rows = new List<GameObject>();
        foreach (Transform child in backpackUI.transform)
        {
            rows.Add(child.gameObject);
        }
        foreach (var row in rows)
        {
            foreach (Transform child in row.transform)
            {
                var slot = child.gameObject.GetComponent<ItemSlot>();
                if (slot.item != null)
                {
                    slot.Detach();
                }
            }
        }
    }

    public ItemSlot GetSlotUI(GameObject backpackUI, int num)
    {
        if (backpackUI == null)
        {
            return null;
        }
        int i = 0;
        var rows = new List<GameObject>();
        foreach (Transform child in backpackUI.transform)
        {
            rows.Add(child.gameObject);
        }
        foreach (var row in rows)
        {
            foreach (Transform child in row.transform)
            {
                if (i == num)
                {
                    return child.gameObject.GetComponent<ItemSlot>();
                }
                i++;
            }
        }
        return null;
    }

    public int GetSlotUINumber(GameObject backpackUI, ItemSlot itemSlot)
    {
        if (backpackUI == null || itemSlot == null)
        {
            return -1;
        }
        int i = 0;
        var rows = new List<GameObject>();
        foreach (Transform child in backpackUI.transform)
        {
            rows.Add(child.gameObject);
        }
        foreach (var row in rows)
        {
            foreach (Transform child in row.transform)
            {
                if (child.gameObject == itemSlot.gameObject)
                {
                    return i;
                }
                i++;
            }
        }
        return -1;
    }

    public bool TryToAddToSlotUI(ItemSlot itemSlot, InventoryItem item)
    {
        if (itemSlot == null || item == null)
        {
            return false;
        }
        if (itemSlot.item == null)
        {
            itemSlot.Attach(item);
            return true;
        }
        return false;
    }

    public bool TryToRemoveFromSlotUI(ItemSlot itemSlot)
    {
        if (itemSlot == null)
        {
            return false;
        }
        if (itemSlot.item != null)
        {
            itemSlot.Detach();
            return true;
        }
        return false;
    }

    public InventoryItem MakeItemIntoInventoryItem(GameObject itemObject)
    {
        if (itemObject == null)
        {
            return null;
        }
        var invItem = Instantiate(inventoryItemPrefab, Vector3.zero, Quaternion.identity).GetComponent<InventoryItem>();
        invItem.AttachObjectAsInventoryItem(itemObject);
        return invItem;
    }
}

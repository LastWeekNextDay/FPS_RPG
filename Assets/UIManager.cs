using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public GameObject itemContainerPrefab;

    private UnityEngine.UI.Slider _healthBar;
    private UnityEngine.UI.Slider _invHealthBar;
    private UnityEngine.UI.Slider _manaBar;
    private UnityEngine.UI.Slider _invManaBar;
    private UnityEngine.UI.Slider _energyBar;
    private UnityEngine.UI.Slider _invEnergyBar;
    private GameObject _container;
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

    private Character _player;
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

    public bool IsContainerOpen {
        get {
            if (UnityEssential.TryFindObject("ContainerInventory", out _container))
            {
                return _container.activeSelf;
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
        if (_player == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                return;
            } else {
                _player = player.GetComponent<Character>();
            }
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

        PlayerViewController.OnMoveAround += () => {
            CursorVisible(false);
        };

        PlayerViewController.OnReset += () => {
            CursorVisible(true);
            ResetCursor();
        };

        ItemContainer.OnItemContainerDestruction += (_) => {
            if (IsInventoryOpen || IsContainerOpen)
            {
                RefreshBackpackItemsUI(GetPlayerInventoryBackpackUI(), _player.Backpack);
                RefreshEquipmentItemsUI(GetPlayerInventoryEquipmentUI(), _player.Equipment);
            }
        };

        Character.OnPickupItem += (args) => {
            if (args.Source.GetComponent<PlayerController>() == null)
            {
                return;
            }
            if (IsInventoryOpen)
            {
                RefreshBackpackItemsUI(GetPlayerInventoryBackpackUI(), args.Source.Backpack);
            }   
        };

        Character.OnWeaponPullout += (args) => {
            if (args.Source.GetComponent<PlayerController>() == null)
            {
                return;
            }
            SetCombatReadyIndicatorColor(Color.red);
        };

        Character.OnWeaponPutaway += (args) => {
            if (args.Source.GetComponent<PlayerController>() == null)
            {
                return;
            }
            SetCombatReadyIndicatorColor(Color.white);
        };

        PlayerController.OnSecondaryAction += (args) => {
            if (IsContainerOpen)
            {
                ToggleContainer(null);
                AllowCursor(false);
                return;
            }
            ToggleInventory();
            AllowCursor(IsInventoryOpen);
        };

        PlayerController.OnContainerOpen += (args) => {
            ToggleContainer(args.Container);
            AllowCursor(IsContainerOpen);
        };
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
                _invStrengthNumber.text = _player.Strength.ToString();
            }

            if (_invAgilityNumber == null)
            {
                if (UnityEssential.TryFindObject("AgilityNumber", out GameObject agilityNumber, false))
                {
                    _invAgilityNumber = agilityNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invAgilityNumber.text = _player.Agility.ToString();
            }

            if (_invEnduranceNumber == null)
            {
                if (UnityEssential.TryFindObject("EnduranceNumber", out GameObject enduranceNumber, false))
                {
                    _invEnduranceNumber = enduranceNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invEnduranceNumber.text = _player.Endurance.ToString();
            }

            if (_invIntelligenceNumber == null)
            {
                if (UnityEssential.TryFindObject("IntelligenceNumber", out GameObject intelligenceNumber, false))
                {
                    _invIntelligenceNumber = intelligenceNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invIntelligenceNumber.text = _player.Intelligence.ToString();
            }

            if (_invHealthNumber == null)
            {
                if (UnityEssential.TryFindObject("HealthNumber", out GameObject healthNumber, false))
                {
                    _invHealthNumber = healthNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invHealthNumber.text = _player.Health.ToString();
            }

            if (_invManaNumber == null)
            {
                if (UnityEssential.TryFindObject("ManaNumber", out GameObject manaNumber, false))
                {
                    _invManaNumber = manaNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invManaNumber.text = _player.Mana.ToString();
            }

            if (_invEnergyNumber == null)
            {
                if (UnityEssential.TryFindObject("EnergyNumber", out GameObject energyNumber, false))
                {
                    _invEnergyNumber = energyNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invEnergyNumber.text = _player.Energy.ToString();
            }

            if (_invMaxHealthNumber == null)
            {
                if (UnityEssential.TryFindObject("MaxHealthNumber", out GameObject maxHealthNumber, false))
                {
                    _invMaxHealthNumber = maxHealthNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invMaxHealthNumber.text = _player.MaxHealth.ToString();
            }

            if (_invMaxManaNumber == null)
            {
                if (UnityEssential.TryFindObject("MaxManaNumber", out GameObject maxManaNumber, false))
                {
                    _invMaxManaNumber = maxManaNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invMaxManaNumber.text = _player.MaxMana.ToString();
            }

            if (_invMaxEnergyNumber == null)
            {
                if (UnityEssential.TryFindObject("MaxEnergyNumber", out GameObject maxEnergyNumber, false))
                {
                    _invMaxEnergyNumber = maxEnergyNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invMaxEnergyNumber.text = _player.MaxEnergy.ToString();
            }

            if (_invSpeedNumber == null)
            {
                if (UnityEssential.TryFindObject("SpeedNumber", out GameObject speedNumber, false))
                {
                    _invSpeedNumber = speedNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invSpeedNumber.text = _player.Speed.ToString();
            }

            if (_invJumpForceNumber == null)
            {
                if (UnityEssential.TryFindObject("JumpForceNumber", out GameObject jumpForceNumber, false))
                {
                    _invJumpForceNumber = jumpForceNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invJumpForceNumber.text = _player.JumpForce.ToString();
            }

            if (_invHealthRegenNumber == null)
            {
                if (UnityEssential.TryFindObject("HealthRegenNumber", out GameObject healthRegenNumber, false))
                {
                    _invHealthRegenNumber = healthRegenNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invHealthRegenNumber.text = _player.HealthRegen.ToString();
            }

            if (_invManaRegenNumber == null)
            {
                if (UnityEssential.TryFindObject("ManaRegenNumber", out GameObject manaRegenNumber, false))
                {
                    _invManaRegenNumber = manaRegenNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invManaRegenNumber.text = _player.ManaRegen.ToString();
            }

            if (_invEnergyRegenNumber == null)
            {
                if (UnityEssential.TryFindObject("EnergyRegenNumber", out GameObject energyRegenNumber, false))
                {
                    _invEnergyRegenNumber = energyRegenNumber.GetComponent<TMPro.TextMeshProUGUI>();
                }
            } else {
                _invEnergyRegenNumber.text = _player.EnergyRegen.ToString();
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
            _healthBar.value = _player.Health / _player.MaxHealth;
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
                _invHealthBar.value = _player.Health / _player.MaxHealth;
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
            _manaBar.value = _player.Mana / _player.MaxMana;
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
                _invManaBar.value = _player.Mana / _player.MaxMana;
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
            _energyBar.value = _player.Energy / _player.MaxEnergy;
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
                _invEnergyBar.value = _player.Energy / _player.MaxEnergy;
            }
        }
    }

    public void ToggleInventory()
    {
        if (_inventoryPanel == null)
        {
            if (UnityEssential.TryFindObject("Inventory", out _inventoryPanel))
            {
                InventoryActivate(!_inventoryPanel.activeSelf);
            }
        } else {
            InventoryActivate(!_inventoryPanel.activeSelf);
        }
    }

    public void ToggleContainer(Backpack container)
    {
        if(_container == null)
        {
            if (UnityEssential.TryFindObject("ContainerInventory", out _container))
            {
                ContainerActive(!_container.activeSelf, container);
            }
        } else {
            ContainerActive(!_container.activeSelf, container);
        }
    }

    void ContainerActive(bool t, Backpack container)
    {
        if (t)
        {
            _container.SetActive(t);
            InitializeBackpackItemsUI(GetContainerRightSideBackpackUI(), _player.Backpack);
            InitializeBackpackItemsUI(GetContainerLeftSideBackpackUI(), container);
        } else {
            DetachAllItemsFromBackpackSlots(GetContainerRightSideBackpackUI());
            DetachAllItemsFromBackpackSlots(GetContainerLeftSideBackpackUI());
            _container.SetActive(t);
        }
    }

    void InventoryActivate(bool t)
    {
        if (t)
        {
            _inventoryPanel.SetActive(t);
            InitializeBackpackItemsUI(GetPlayerInventoryBackpackUI(), _player.Backpack);
            InitializeEquipmentItemsUI(GetPlayerInventoryEquipmentUI(), _player.Equipment);
        } else {
            DetachAllItemsFromBackpackSlots(GetPlayerInventoryBackpackUI());
            DetachAllItemsFromEquipmentSlots(GetPlayerInventoryEquipmentUI());
            _inventoryPanel.SetActive(t);
        }
    }

    public GameObject GetContainerRightSideBackpackUI()
    {
        if (UnityEssential.TryFindObject("BackpackBaseRight", out GameObject backpack))
        {
            return backpack;
        }
        return null;
    }

    public GameObject GetContainerLeftSideBackpackUI()
    {
        if (UnityEssential.TryFindObject("BackpackBaseLeft", out GameObject backpack))
        {
            return backpack;
        }
        return null;
    }

    public GameObject GetPlayerInventoryEquipmentUI()
    {
        if (UnityEssential.TryFindObject("EquipmentBase", out GameObject equipment))
        {
            return equipment;
        }
        return null;
    }

    public GameObject GetCombatReadyIndicator()
    {
        if (UnityEssential.TryFindObject("CombatReadyIndicator", out GameObject combatReadyIndicator))
        {
            return combatReadyIndicator;
        }
        return null;
    }

    public void SetCombatReadyIndicatorColor(Color color)
    {
        var indicator = GetCombatReadyIndicator();
        if (indicator != null)
        {
            indicator.GetComponent<UnityEngine.UI.Image>().color = color;
        }
    }

    public InventorySlot GetPrimaryWeaponSlotUI(GameObject equipmentUI)
    {
        if (equipmentUI == null)
        {
            return null;
        }
        foreach (Transform child in equipmentUI.transform)
        {
            if (child.gameObject.name == "Weapon1Holder")
            {
                return child.gameObject.GetComponent<InventorySlot>();
            }
        }
        return null;
    }

    public GameObject GetPlayerInventoryBackpackUI()
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

    public InventorySlot GetFirstAvailableBackpackSlotUI(GameObject backpackUI)
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
                var slot = child.gameObject.GetComponent<InventorySlot>();
                if (slot.attachedItemContainer == null)
                {
                    return slot;
                }
            }
        }
        return null;
    }

    public void InitializeEquipmentItemsUI(GameObject equipmentUI, Equipment equipment)
    {
        if (equipmentUI == null || equipment == null)
        {
            return;
        }
        foreach (Transform child in equipmentUI.transform)
        {
            var slot = child.gameObject.GetComponent<InventorySlot>();
            slot.Equipment = equipment;
            slot.Backpack = equipment.Owner.Backpack;
            switch (slot.slotType)
            {
                case SlotType.EquipmentWeaponPrimary:
                    if (equipment.WeaponItem != null && equipment.WeaponItem.IsToBeReplaced() == false)
                    {
                        TryToAddToSlotUI(slot, MakeItemIntoItemContainerUI(equipment.WeaponItem));
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void InitializeBackpackItemsUI(GameObject backpackUI, Backpack backpack)
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
                var slot = child.gameObject.GetComponent<InventorySlot>();
                slot.Backpack = backpack;
                slot.backpackSlotIndex = UIIndex;
                if (backpack.Items[UIIndex] != null)
                {
                    TryToAddToSlotUI(slot, MakeItemIntoItemContainerUI(backpack.Items[UIIndex]));
                }
                UIIndex++;
            }
        }
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
                var slot = child.gameObject.GetComponent<InventorySlot>();
                if (backpack.Items[UIIndex] != null)
                {
                    if (slot.attachedItemContainer == null)
                    {
                        TryToAddToSlotUI(slot, MakeItemIntoItemContainerUI(backpack.Items[UIIndex]));
                    }   
                }
                else
                {
                    if (slot.attachedItemContainer != null)
                    {
                        TryToRemoveFromSlotUI(slot);
                    }
                }
                UIIndex++;
            }
        }
    }

    public void RefreshEquipmentItemsUI(GameObject equipmentUI, Equipment equipment)
    {
        if (equipmentUI == null || equipment == null)
        {
            return;
        }
        foreach (Transform child in equipmentUI.transform)
        {
            var slot = child.gameObject.GetComponent<InventorySlot>();
            switch (slot.slotType)
            {
                case SlotType.EquipmentWeaponPrimary:
                    if (equipment.WeaponItem != null && equipment.WeaponItem.IsToBeReplaced() == false)
                    {
                        if (slot.attachedItemContainer == null)
                        {
                            TryToAddToSlotUI(slot, MakeItemIntoItemContainerUI(equipment.WeaponItem));
                        }
                    }
                    else if (equipment.WeaponItem == null || equipment.WeaponItem.IsToBeReplaced() == true)
                    {
                        if (slot.attachedItemContainer != null)
                        {
                            TryToRemoveFromSlotUI(slot);
                        }
                    }
                    break;
            }
        }
    }

    void DetachAllItemsFromBackpackSlots(GameObject backpackUI)
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
                var slot = child.gameObject.GetComponent<InventorySlot>();
                TryToRemoveFromSlotUI(slot);
            }
        }
    }

    void DetachAllItemsFromEquipmentSlots(GameObject equipmentUI)
    {
        if (equipmentUI == null)
        {
            return;
        }
        foreach (Transform child in equipmentUI.transform)
        {
            var slot = child.gameObject.GetComponent<InventorySlot>();
            TryToRemoveFromSlotUI(slot);
        }
    }

    public InventorySlot GetBackpackSlotUI(GameObject backpackUI, int num)
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
                    return child.gameObject.GetComponent<InventorySlot>();
                }
                i++;
            }
        }
        return null;
    }

    public int GetBackpackSlotUINumber(GameObject backpackUI, InventorySlot itemSlot)
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

    public bool TryToAddToSlotUI(InventorySlot slot, ItemContainer item)
    {
        if (slot == null || item == null)
        {
            return false;
        }
        if (slot.attachedItemContainer == null)
        {
            slot.AssignVisual(item);
            return true;
        }
        return false;
    }

    public bool TryToRemoveFromSlotUI(InventorySlot slot)
    {
        if (slot == null)
        {
            return false;
        }
        if (slot.attachedItemContainer != null)
        {
            var itemContainer = slot.attachedItemContainer;
            itemContainer.DestroyItemContainter();
            return true;
        }
        return false;
    }

    public ItemContainer MakeItemIntoItemContainerUI(Item item)
    {
        if (item == null)
        {
            return null;
        }
        var itemContainer = Instantiate(itemContainerPrefab, Vector3.zero, Quaternion.identity).GetComponent<ItemContainer>();
        itemContainer.AttachItemToItemContainer(item);
        return itemContainer;
    }
}

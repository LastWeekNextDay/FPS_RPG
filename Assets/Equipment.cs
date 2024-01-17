public class Equipment
{
    public InventoryItemUI WeaponInventoryItem;
    public InventoryItemUI HelmetInventoryItem;
    public InventoryItemUI ChestInventoryItem;
    public InventoryItemUI LegsInventoryItem;
    public InventoryItemUI BootsInventoryItem;
    public Character Owner;

    public Equipment(Character owner)
    {
        Owner = owner;
    }

    public bool TryEquip(InventoryItemUI item)
    {
        switch (item.RepresentedItem.ItemType)
        {
            case ItemType.Weapon:
                return TryEquipWeapon(item);
            default:
                return false;
        }
    }

    public bool TryUnequip(InventoryItemUI item)
    {
        switch (item.RepresentedItem.ItemType)
        {
            case ItemType.Weapon:
                return TryUnequipWeapon(item);
            default:
                return false;
        }
    }
    
    public bool TryEquipWeapon(InventoryItemUI item)
    {
        if (item.RepresentedItem.ItemType != ItemType.Weapon) return false;
        if (WeaponInventoryItem == null || WeaponInventoryItem.RepresentedItem.IsToBeReplaced() ||
        WeaponInventoryItem == item)
        {
            WeaponInventoryItem = item;
            return true;
        }
        return false;
    }

    public bool TryUnequipWeapon(InventoryItemUI item)
    {
        if (item.RepresentedItem.ItemType != ItemType.Weapon) return false;
        if (WeaponInventoryItem == item) 
        {
            WeaponInventoryItem = null;
            return true;
        }
        return false;
    }
}
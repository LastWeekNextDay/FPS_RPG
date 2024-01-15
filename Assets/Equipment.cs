public class Equipment
{
    public InventoryItemUI WeaponInventoryItem;

    public Equipment()
    {
        WeaponInventoryItem = null;
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
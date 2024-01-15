public class Equipment
{
    public InventoryItemUI WeaponInventoryItem;

    public Equipment()
    {
        WeaponInventoryItem = null;
    }

    public bool TryEquipWeapon(InventoryItemUI item)
    {
        if (item.RepresentedItem.GetComponent<Weapon>() == null) return false;
        if (WeaponInventoryItem == null || WeaponInventoryItem.RepresentedItem.GetComponent<Weapon>().weaponName == "Fists" ||
        WeaponInventoryItem == item)
        {
            WeaponInventoryItem = item;
            return true;
        }
        return false;
    }

    public bool TryUnequipWeapon(InventoryItemUI item)
    {
        if (item.RepresentedItem.GetComponent<Weapon>() == null) return false;
        if (WeaponInventoryItem == item) 
        {
            WeaponInventoryItem = null;
            return true;
        }
        return false;
    }
}
public class Equipment
{
    public Weapon WeaponItem;
    public Item HelmetItem;
    public Item ChestItem;
    public Item LegsItem;
    public Item BootsItem;
    public Character Owner;

    public Equipment(Character owner)
    {
        Owner = owner;
    }

    public bool TryEquip(Item item)
    {
        switch (item.ItemType)
        {
            case ItemType.Weapon:
                return TryEquipWeapon(item.GetComponent<Weapon>());
            default:
                throw new System.NotImplementedException();
        }
    }

    public bool TryUnequip(Item item)
    {
        switch (item.ItemType)
        {
            case ItemType.Weapon:
                return TryUnequipWeapon(item.GetComponent<Weapon>());
            default:
                throw new System.NotImplementedException();
        }
    }
    
    public bool TryEquipWeapon(Weapon weapon)
    {
        if (WeaponItem == null || WeaponItem.IsToBeReplaced() || WeaponItem == weapon)
        {
            WeaponItem = weapon;
            return true;
        }

        return false;
    }

    public bool TryUnequipWeapon(Weapon weapon)
    {
        if (WeaponItem == weapon) 
        {
            WeaponItem = null;
            return true;
        }
        
        return false;
    }

    public bool TryEquipHelmet(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryUnequipHelmet(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryEquipChest(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryUnequipChest(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryEquipLegs(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryUnequipLegs(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryEquipBoots(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool TryUnequipBoots(Item item)
    {
        throw new System.NotImplementedException();
    }
}
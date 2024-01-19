public class Armor : Item
{
    public SlotType armorSlotType;
    
    protected override void Start()
    {
        base.Start();
        ItemType = ItemType.Armor;
    }
}

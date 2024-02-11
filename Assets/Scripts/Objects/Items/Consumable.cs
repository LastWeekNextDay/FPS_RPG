public class Consumable : Item
{
    protected override void Start()
    {
        base.Start();
        ItemType = ItemType.Consumable;
    }
}

public class Backpack
{
    public InventoryItemUI[] InventoryItems;

    public Backpack()
    {
        InventoryItems = new InventoryItemUI[28];
    }

    public bool TryAddItem(InventoryItemUI item)
    {
        for (int i = 0; i < InventoryItems.Length; i++)
        {
            if (InventoryItems[i] == null)
            {
                InventoryItems[i] = item;
                return true;
            }
        }
        return false;
    }

    public bool TryChangeItemPosition(InventoryItemUI item, int toPosition)
    {
        if (InventoryItems[toPosition] == null)
        {
            InventoryItems[toPosition] = item;
            return true;
        }
        InventoryItemUI itemToSwap;
        for (int i = 0; i < InventoryItems.Length; i++)
        {
            if (InventoryItems[i] == item)
            {
                itemToSwap = InventoryItems[i];
                InventoryItems[toPosition] = itemToSwap;
                InventoryItems[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool TryRemoveItem(InventoryItemUI item)
    {
        for (int i = 0; i < InventoryItems.Length; i++)
        {
            if (InventoryItems[i] == item)
            {
                InventoryItems[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool TryGetItemIndex(InventoryItemUI item, out int index)
    {
        for (int i = 0; i < InventoryItems.Length; i++)
        {
            if (InventoryItems[i] == item)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }
}

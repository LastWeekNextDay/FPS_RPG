public class Backpack
{
    public Item[] Items;
    public Character Owner;
    public Container containerOf;

    public Backpack(Character owner, Container container = null)
    {
        Owner = owner;
        Items = new Item[28];
        containerOf = container;
    }

    public bool TryAddItem(Item item)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null)
            {
                Items[i] = item;
                return true;
            }
        }
        return false;
    }

    public bool TryReplaceItem(Item item, Item newItem)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == item)
            {
                Items[i] = newItem;
                return true;
            }
        }
        return false;
    }

    public bool TryReplaceItemAtPosition(Item newItem, int position)
    {
        if (Items[position] == null)
        {
            Items[position] = newItem;
            return true;
        }
        return false;
    }

    public bool TryChangeItemPosition(Item item, int toPosition)
    {
        if (Items[toPosition] == null)
        {
            Items[toPosition] = item;
            return true;
        }
        Item itemToSwap;
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == item)
            {
                itemToSwap = Items[i];
                Items[toPosition] = itemToSwap;
                Items[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool TryRemoveItem(Item item)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == item)
            {
                Items[i] = null;
                return true;
            }
        }
        return false;
    }

    public bool TryGetItemIndex(Item item, out int index)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == item)
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }
}

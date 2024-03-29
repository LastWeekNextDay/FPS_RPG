using System.Collections.Generic;

public struct DamageArgs 
{
    public Character Source;
    public Character Target;
    public float Damage;
    public List<StatusEffect> StatusEffects;
}

public struct JumpArgs
{
    public Character Source;
}

public struct ContainerArgs
{
    public ContainerBackpack itemsContainer;
}
public struct PickupItemArgs
{
    public Character Source;
    public Item Item;
}

public struct StatusEffectArgs
{
    public Character Target;
    public StatusEffect StatusEffect;
}

public struct AppliedStatusEffectArgs
{
    public Character Target;
    public AppliedStatusEffect StatusEffect;
}

public struct WeaponPulloutArgs
{
    public Character Source;
    public Weapon Weapon;
}

public struct WeaponPutawayArgs
{
    public Character Source;
    public Weapon Weapon;
}

public struct PrimaryActionArgs
{
    public Character Source;
}

public struct SecondaryActionArgs
{
    public Character Source;
}

public struct UseActionArgs
{
    public Character Source;
}

public struct ReadyActionArgs
{
    public Character Source;
}

public struct DropArgs
{
    public Character Source;
    public Item Item;
    public UnityEngine.Vector3 Position;
}

public struct EquipArgs
{
    public Character Source;
    public InventoryItemContainer InventoryItemContainer;
    public InventorySlot Slot;
}

public struct UnequipArgs
{
    public Character Source;
    public InventoryItemContainer InventoryItemContainer;
    public InventorySlot Slot;
}

public struct MouseAttachArgs
{
    public InventoryItemContainer InventoryItemContainer;
}

public struct MouseDetachArgs
{
    public InventoryItemContainer InventoryItemContainer;
}

public struct CancelSelectionArgs
{
    public InventoryItemContainer InventoryItemContainer;
}

public struct InventoryItemContainerSlotAttachmentArgs
{
    public InventorySlot Slot;
    public InventoryItemContainer InventoryItemContainer;
}

public struct HittableObjectHitArgs
{
    public MaterialType MaterialType;
    public UnityEngine.Vector3 Direction;
    public UnityEngine.Vector3 HitPoint;
}
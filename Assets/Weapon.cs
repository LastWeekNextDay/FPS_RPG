using System;
using Unity.VisualScripting;
using UnityEngine;

public enum MainAttackType
{
    Melee
}

public enum WeaponBaseType
{
    Fists,
    ShortSword
}

public class Weapon : Item
{
    public string weaponName;
    public MainAttackType mainAttackType;
    public WeaponBaseType weaponBaseType;
    public float baseDamage;
    public float baseAttackSpeed;
    [NonSerialized] public float AttackSpeed;
    public float baseRange;
    public float baseWindupTime;
    public float basePullOutTime;
    public float baseAttackTime;
    public bool IsTwoHanded;
    [NonSerialized] public Character Wielder;
    [NonSerialized] public bool IsReady;
}

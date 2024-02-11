using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBackpack : Backpack
{
    public Character Owner;
    public CharacterBackpack(Character owner) : base()
    {
        Owner = owner;
    }
}

using System;
using UnityEngine;

public enum MaterialType
{
    Generic
}

public abstract class Object : MonoBehaviour
{
    public MaterialType MaterialType;
    public static Action<ObjectHitArgs> OnHit;

    public abstract void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint);    
}

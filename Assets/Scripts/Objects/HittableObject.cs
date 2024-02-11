using System;
using UnityEngine;

public enum MaterialType
{
    Generic
}

public abstract class HittableObject : MonoBehaviour
{
    public MaterialType MaterialType;
    public static Action<HittableObjectHitArgs> OnHit;

    public abstract void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint);    
}

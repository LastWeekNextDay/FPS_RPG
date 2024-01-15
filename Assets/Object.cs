using System;
using UnityEngine;

public enum MaterialType
{
    Generic
}

public abstract class Object : MonoBehaviour
{
    public MaterialType MaterialType;
    public Action<Vector3, Vector3> OnHit;

    public abstract void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint);    
}

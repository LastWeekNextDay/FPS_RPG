using UnityEngine;

public class Wall : HittableObject
{
    public override void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint)
    {
        var args = new HittableObjectHitArgs{
            MaterialType = MaterialType,
            Direction = dirFromWhereHit,
            HitPoint = hitPoint
        };
        OnHit?.Invoke(args);
    }
}

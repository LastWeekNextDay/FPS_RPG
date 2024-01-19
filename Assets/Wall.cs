using UnityEngine;

public class Wall : Object
{
    public override void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint)
    {
        var args = new ObjectHitArgs{
            MaterialType = MaterialType,
            Direction = dirFromWhereHit,
            HitPoint = hitPoint
        };
        OnHit?.Invoke(args);
    }
}

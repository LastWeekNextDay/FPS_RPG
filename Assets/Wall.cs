using UnityEngine;

public class Wall : Object
{
    void Start()
    {
        OnHit += (dir, hitPoint) => ParticleManager.Instance.PlayHit(MaterialType, hitPoint, Quaternion.LookRotation(dir));
    }
    public override void GetHit(Vector3 dirFromWhereHit, Vector3 hitPoint)
    {
        OnHit?.Invoke(dirFromWhereHit, hitPoint);
    }
}

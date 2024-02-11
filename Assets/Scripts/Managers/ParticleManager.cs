using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        HittableObject.OnHit += (args) => PlayHit(args.MaterialType, args.HitPoint, Quaternion.LookRotation(args.Direction));
    }

    public void PlayParticle(string particleName, Vector3 pos, Quaternion rot)
    {
        if (PrefabContainer.Instance.TryGetPrefab(particleName, out GameObject particle))
        {
            Instantiate(particle, pos, rot);
        }
        else
        {
            Debug.LogError($"Particle {particleName} not found in PrefabContainer");
        }
    }

    public void PlayHit(MaterialType materialType, Vector3 pos, Quaternion rot)
    {
        switch (materialType)
        {
            case MaterialType.Generic:
                PlayParticle("Hit_01", pos, rot);
                break;
            default:
                Debug.LogError($"MaterialType {materialType} not implemented");
                break;
        }
    }
}

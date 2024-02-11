using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabContainer : MonoBehaviour
{
    private static PrefabContainer _instance;
    public static PrefabContainer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PrefabContainer>();
            }
            return _instance;
        }
    }
    public List<GameObject> prefabs;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Multiple PrefabContainers found in scene!");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public bool TryGetPrefab(string name, out GameObject prefab)
    {
        foreach (var p in prefabs)
        {
            if (p.name == name)
            {
                prefab = p;
                return true;
            }
        }

        prefab = null;
        return false;
    }
}

using UnityEngine;

public class UnityEssential
{
    static public Transform FindChild(Transform parent, string name)
    {
        Transform child = null;

        foreach (Transform t in parent)
        {
            if (t.name == name)
            {
                child = t;
                break;
            } 
            else 
            {
                child = FindChild(t, name);

                if (child != null)
                {
                    break;
                }
            }
        }
        
        return child;
    }

    static public GameObject FindObject(string name, bool includeInactive = true)
    {
        FindObjectsInactive mode = includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;
        GameObject[] objs = UnityEngine.Object.FindObjectsByType<GameObject>(mode, FindObjectsSortMode.None);

        foreach (GameObject obj in objs)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }

        return null;
    }

    static public GameObject FindObjectInChildren(GameObject parent, string name, bool includeInactive = true)
    {
        FindObjectsInactive mode = includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;
        GameObject[] objs = UnityEngine.Object.FindObjectsByType<GameObject>(mode, FindObjectsSortMode.None);

        foreach (GameObject obj in objs)
        {
            if (obj.name == name && obj.transform.IsChildOf(parent.transform))
            {
                return obj;
            }
        }
        
        return null;
    }

    static public bool TryFindObject(string name, out GameObject obj, bool includeInactive = true)
    {
        obj = FindObject(name, includeInactive);
        return obj != null;
    }

    static public bool TryFindObjectInChildren(GameObject parent, string name, out GameObject obj, bool includeInactive = true)
    {
        obj = FindObjectInChildren(parent, name, includeInactive);
        return obj != null;
    }
}

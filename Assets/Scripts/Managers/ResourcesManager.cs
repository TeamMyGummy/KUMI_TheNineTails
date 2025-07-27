using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = PoolManager.Instance.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"[ResourcesManager] Failed to load prefab: {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return PoolManager.Instance.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }
    
    public GameObject Instantiate(GameObject original, Transform parent = null)
    {
        if (original == null)
        {
            Debug.Log($"[ResourcesManager] Failed to instantiate prefab: {original.name}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return PoolManager.Instance.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go, float delay = 0f)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            PoolManager.Instance.Push(poolable, delay).Forget();
            return;
        }

        Object.Destroy(go, delay);
    }
}
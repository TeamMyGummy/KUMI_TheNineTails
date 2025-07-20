using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class PoolManager : SceneSingleton<PoolManager>
{
	#region Pool Inner Class
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject($"{original.name}_Root").transform;
            Root.SetParent(Instance._root);

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null) return;

            poolable.transform.SetParent(Root);
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable = _poolStack.Count > 0 ? _poolStack.Pop() : Create();

            poolable.gameObject.SetActive(true);
            poolable.transform.SetParent(parent);
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion

    private readonly Dictionary<string, Pool> _pools = new();
    private Transform _root;

    private void Awake()
    {
        if (_root == null)
        {
            _root = new GameObject("@Pool_Root").transform;
        }
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        if (_pools.ContainsKey(original.name)) return;

        Pool pool = new Pool();
        pool.Init(original, count);
        _pools.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Push(poolable);
        }
        else
        {
            Destroy(poolable.gameObject);
        }
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pools.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pools[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        return _pools.TryGetValue(name, out var pool) ? pool.Original : null;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            Destroy(child.gameObject);

        _pools.Clear();
    }
}

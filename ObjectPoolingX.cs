using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingX<T> : MonoBehaviour where T: MonoBehaviour
{

    private static ObjectPoolingX<T> _instant;
    public static ObjectPoolingX<T> Instant => _instant;

    private void Awake()
    {
        if (_instant == null)
            _instant = this;
        if (_instant.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
        {
            Destroy(this.gameObject);
        }
    }


    Dictionary<T, List<T>> _poolObjects = new Dictionary<T, List<T>>();
    Dictionary<GameObject, List<GameObject>> _poolObjects2 = new Dictionary<GameObject, List<GameObject>>();

    public T GetObjectType(T key)
    {
        List<T> _itemPool = new List<T>();
        if (!_poolObjects.ContainsKey(key))
        {
            _poolObjects.Add(key, _itemPool);
        }
        else
        {
            _itemPool = _poolObjects[key];
        }


        foreach (T g in _itemPool)
        {
            if (g.gameObject.activeSelf)
                continue;
            return g;
        }

        T g2 = Instantiate(key, this.transform.position, Quaternion.identity);
        _poolObjects[key].Add(g2);
        return g2;

    }

    public GameObject GetObject(GameObject key)
    {
        List<GameObject> _itemPool = new List<GameObject>();
        if (!_poolObjects2.ContainsKey(key))
        {
            _poolObjects2.Add(key, _itemPool);
        }
        else
        {
            _itemPool = _poolObjects2[key];
        }


        foreach (GameObject g in _itemPool)
        {
            if (g.gameObject.activeSelf)
                continue;
            return g;
        }

        GameObject g2 = Instantiate(key, this.transform.position, Quaternion.identity);
        _poolObjects2[key].Add(g2);
        return g2;
    }
}

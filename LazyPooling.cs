using System.Collections.Generic;
using UnityEngine;

namespace HuynnLib{
public class LazyPooling : MonoBehaviour
{

    private static LazyPooling _instant;
    public static LazyPooling Instant  {
            get
            {
                if (_instant == null)
                {
                   if (FindObjectOfType<LazyPooling>() != null)
                        _instant = FindObjectOfType<LazyPooling>();
                   else
                    new GameObject().AddComponent<LazyPooling>().name = "Singleton_"+  typeof(LazyPooling).ToString();
                }

                return _instant;
            }
        }
       
        void Awake()
        {
            if (_instant != null && _instant.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                Debug.LogError("Singleton already exist "+ _instant.gameObject.name);
                Destroy(this.gameObject);
            }
            else
                _instant = this.GetComponent<LazyPooling>();
        }


    Dictionary<GameObject, List<GameObject>> _poolObjects2 = new Dictionary<GameObject, List<GameObject>>();

    public GameObject GetObj(GameObject objKey, bool isKeepParent = false){
         if (!_poolObjects2.ContainsKey(objKey))
        {
            _poolObjects2.Add(objKey, new List<GameObject>());
        }

        foreach (var g in _poolObjects2[objKey])
        {
            if (g.gameObject.activeSelf)
                continue;
            return g;
        }

        GameObject g2 = Instantiate(objKey);
        _poolObjects2[objKey].Add(g2);

        if (isKeepParent)
            g2.transform.SetParent(objKey.transform.parent);
        return g2;
    }

    Dictionary<Component, List<Component>> _poolObjt = new Dictionary<Component, List<Component>>();
   
    public T getObj<T>(T objKey, bool isKeepParent = false) where T : Component 
    {
        if (!_poolObjt.ContainsKey(objKey))
        {
            _poolObjt.Add(objKey, new List<Component>());
        }

        foreach (T g in _poolObjt[objKey])
        {
            if (g.gameObject.activeSelf)
                continue;
            return g;
        }

        T g2 = Instantiate(objKey);
        _poolObjt[objKey].Add(g2);

        if (isKeepParent)
            g2.transform.SetParent(objKey.transform.parent);
        return g2;
    }

    public void resetObj<T>( T objKey) where T:Component
    {
        if (!_poolObjt.ContainsKey(objKey))
        {
            return;
        }

        foreach (T g in _poolObjt[objKey])
        { 
            g.gameObject.SetActive(false);
        }
    }



    public void CreatePool<T>(T keyObj, int size) where T : Component
        {
            if (!_poolObjt.ContainsKey(keyObj))
            {
                _poolObjt.Add(keyObj, new List<Component>());
            }
            for (int i = 0; i < size; i++)
            {
                this.getObj<T>(keyObj,true).gameObject.SetActive(false);
            }
        }
}
}

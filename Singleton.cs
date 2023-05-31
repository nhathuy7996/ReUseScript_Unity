using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuynnLib
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
    {

        private static T _instant = null;
        public static T Instant
        {
            get
            {
                if (_instant == null)
                {
                   if (FindObjectOfType<T>() != null)
                        _instant = FindObjectOfType<T>();
                   else
                    new GameObject().AddComponent<T>().name = "Singleton_"+  typeof(T).ToString();
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
                _instant = this.GetComponent<T>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}


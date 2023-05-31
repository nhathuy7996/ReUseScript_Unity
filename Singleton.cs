using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DVAH
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instant = null;
        public static T Instant => instant;
         
        void Awake()
        {
            if (instant == null)
            {
                instant = this.GetComponent<T>();
            } else if (!instant.GetInstanceID().Equals(this.GetInstanceID())) {
                Debug.LogErrorFormat("Duplicated singleton between {0} - {1}!, destroy {1}",instant.gameObject.name,this.gameObject.name);
            }
 
        }
        
    }

}


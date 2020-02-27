using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instant = null;
    public static T Instant{
        get{
            if(instant ==null){
                T[] Ts = FindObjectsOfType<T>();
                if(Ts.Length > 0)
                    instant = Ts[0];
            }

            return instant;
        }
    }

    void Awake(){
        T[] Ts = FindObjectsOfType<T>();
        if(Ts.Length > 1)
            Destroy(this);
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

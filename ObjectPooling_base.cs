using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling_base<T> : MonoBehaviour where T : MonoBehaviour
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

    [SerializeField]
    GameObject Obj_prefab;
    List<GameObject> List_obj = new List<GameObject>();


    protected GameObject Get_Obj(){
        foreach(GameObject G in List_obj){
            if(G.activeSelf){
                continue;
            }
            return G;
        }

        GameObject G2 = Instantiate(Obj_prefab,this.transform.position,Quaternion.identity,this.transform);
        List_obj.Add(G2);
        G2.SetActive(false);
        return G2;
    }
}

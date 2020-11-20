using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling_base<T,L> : MonoBehaviour where T : MonoBehaviour where L: MonoBehaviour
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
            Destroy(this);
    }

    [SerializeField]
    GameObject Obj_prefab;
    List<L> List_obj = new List<L>();


    protected L Get_Obj(){
        foreach(L G in List_obj){
            if( G.gameObject.activeSelf){
                continue;
            }
            return G;
        }

        L G2 = Instantiate(Obj_prefab,this.transform.position,Quaternion.identity,this.transform).GetComponent<L>();
        List_obj.Add(G2);
        G2.gameObject.SetActive(false);
        return G2;
    }
}

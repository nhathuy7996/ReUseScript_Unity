using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPathGizmos : MonoBehaviour
{
    [SerializeField]
    float SpeedMovement = 1f;
    [SerializeField]
    int Num_Path = 2;

    List<GameObject> ShowPos = new List<GameObject>();

    List<Vector2> Path = new List<Vector2>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected(){
        if(ShowPos.Count < Num_Path){
            GameObject g = new GameObject();
            ShowPos.Add(g);
            return;
        }
        
        Gizmos.color = Color.yellow;
        for(int i = 0; i<ShowPos.Count; i++){
            if(ShowPos[i] == null)
                ShowPos[i] = new GameObject();
            ShowPos[i].transform.position = Path[i];
            Gizmos.DrawWireSphere(ShowPos[i].transform.position,0.5f);
        }
        
    }

    void OnDrawGizmos(){
        if(Path.Count <= 0){
            for(int i = 0; i<Num_Path; i++){
                Path.Add((Vector2)this.transform.position + Vector2.up);
            }
        }

        Gizmos.color = Color.yellow;

        for(int i = 0; i<Path.Count-1; i++){
            Gizmos.DrawLine(Path[i],Path[i+1]);
        }
        if (CheckSelect()){
  
            for(int i = 0; i<ShowPos.Count; i++){
                if (ShowPos[i] == null )
                    continue;
                if (UnityEditor.Selection.activeGameObject == ShowPos[i]){
                    Path[i] = ShowPos[i].transform.position;                   
                }
                Gizmos.DrawWireSphere(ShowPos[i].transform.position,0.5f);
            }
            return;
        }

        if(ShowPos.Count == 0)
                return;
        for(int i = 0; i<ShowPos.Count; i++){
            DestroyImmediate(ShowPos[i]);
        }
        
    }

    bool CheckSelect(){
        if (UnityEditor.Selection.activeGameObject == this.gameObject)
            return true;
        
        if(ShowPos.Count == 0)
            return false;
        foreach(GameObject G in ShowPos){
            if(UnityEditor.Selection.activeGameObject == G)
                return true;
        }

        return false;
    }
}

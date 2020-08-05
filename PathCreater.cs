using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreater : MonoBehaviour
{
    [SerializeField]
    int Num_Path = 2;
    [SerializeField]
    List<GameObject> ShowPos = new List<GameObject>();
    [SerializeField]
    List<Vector2> Path = new List<Vector2>(); 
    public int Count => Path.Count;
    
    public Vector2 GetPath(int ID = 0){
        if(ID < 0 || ID >= Path.Count){
            Debug.LogError("ID invalid! "+this.gameObject.name);
            return Vector2.zero;
        }
        return Path[ID];
    }

    public void SetPath(int ID,Vector2 pos){
        if(ID < 0 || ID >= Path.Count){
            Debug.LogError("ID invalid!");
            return;
        }

        Path[ID] = pos;
    }
    #if UNITY_EDITOR
    
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
                }else{
                    ShowPos[i].transform.position = Path[i];    
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
        ShowPos = new List<GameObject>();
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
    #endif
}

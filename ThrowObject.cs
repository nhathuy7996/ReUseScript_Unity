using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    [SerializeField]
    Transform StartPoint = null,EndPoint = null;
    [SerializeField]
    int Angle = 0,Trajectory_num = 100;
    [SerializeField]
    float V = 0,Config = 0.1f;
    float Angle_Rad = 0;
    [SerializeField]
    GameObject Bullet = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(){
        CalV();
        GameObject B = Instantiate(Bullet,StartPoint.transform.position,Quaternion.identity);
        Vector3 Force = Vector3.zero;
        Force.x = V * 50 * Mathf.Cos(Angle_Rad);
        Force.y = V * 50 * Mathf.Sin(Angle_Rad);
        B.GetComponent<Rigidbody2D>().AddForce(Force);
    }

    void CalV(){
        
        float Y = EndPoint.transform.position.y - StartPoint.transform.position.y;
        float X = EndPoint.transform.position.x - StartPoint.transform.position.x;

        if(X < 0){
            Angle_Rad = -Mathf.Abs(Angle) * Mathf.Deg2Rad;
            Config = -Mathf.Abs(Config);
        }else{
            Angle_Rad = Mathf.Abs(Angle) * Mathf.Deg2Rad;
            Config = Mathf.Abs(Config);
        }


        float v2 = ( 10 / ( (Mathf.Tan(Angle_Rad) * X - Y)/ (X*X) ) ) / (2 * Mathf.Cos(Angle_Rad) * Mathf.Cos(Angle_Rad));
        v2 = Mathf.Abs(v2);
        V = Mathf.Sqrt(v2);
    }

    void OnDrawGizmosSelected(){
        
        CalV();

        Gizmos.color = Color.red;
        
        for(int i = 0; i< Trajectory_num; i++){
            float time = i * Config;
            float X = V * Mathf.Cos(Angle_Rad) * time;
            float Y =  V * Mathf.Sin(Angle_Rad) * time - 0.5f * (10 * time * time);

            Vector3 pos1 = StartPoint.transform.position + new Vector3(X,Y,0);

            time = (i+1) * Config;
            X = V * Mathf.Cos(Angle_Rad) * time;
            Y =  V * Mathf.Sin(Angle_Rad) * time - 0.5f * (10 * time * time);

            Vector3 pos2 = StartPoint.transform.position + new Vector3(X,Y,0);

            Gizmos.DrawLine(pos1,pos2);
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffect : MonoBehaviour
{
    float rota = 0;
    public float Rota => rota;
    [SerializeField]
    float _SpeedRotZ = 50,_SpeedRotY = 50;
    public float SpeedRotZ {
        get{return _SpeedRotZ;}
        set{_SpeedRotZ = value;}}

    // Start is called before the first frame update

    void OnEnable(){
        this.transform.eulerAngles = new Vector3(0,0,0);
        rota = 0;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.rota += Time.deltaTime*_SpeedRotZ;
        this.rota %= 360;
        if(this.rota < 0)
            this.rota += 360;
        if(GameController.Instant.STATE == GameController.GameState.RUN)
            this.transform.eulerAngles = new Vector3(0,0,this.rota);
    }
}

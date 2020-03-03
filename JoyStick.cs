using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStick : Singleton<JoyStick>
{
    [SerializeField]
    Transform Root,Pad;
    [SerializeField]
    float MaxR = 1;

    Vector2 Origin = new Vector2(0,0);
    [SerializeField]
    bool _IsOriginSet = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ListenJoyStick();
        ReleaseTouch();
    }

    void ListenJoyStick(){

        if(!Input.GetMouseButton(0))
            return;
        if(!_IsOriginSet){
            _IsOriginSet = true;
            Origin = Input.mousePosition;
            Root.position = Origin;
            Pad.transform.position = Origin;
            Root.gameObject.SetActive(true);
            return;
        }
        Vector2 currentTouch = (Vector2)Input.mousePosition-Origin;
        if(currentTouch == Vector2.zero)
            return;
        if(currentTouch.magnitude <= MaxR){
            Pad.transform.position = Input.mousePosition;
            return;
        }

        float currentAngle = Mathf.Atan2(currentTouch.y,currentTouch.x);
        float X = Origin.x + MaxR * Mathf.Cos(currentAngle);
        float Y = Origin.y + MaxR * Mathf.Sin(currentAngle);
        Pad.transform.position = new Vector2(X,Y);
    }

    void ReleaseTouch(){
        if(!_IsOriginSet)
            return;
        if(Input.GetMouseButtonUp(0)){
            _IsOriginSet = false;
            Root.gameObject.SetActive(false);
        }
    }

    public Vector2 GetJoyVector(){
        if(!_IsOriginSet)
            return Vector2.zero;
        Vector2 tmp = (Vector2)Input.mousePosition-Origin;
        return tmp.normalized;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    List<string> Pressed = new List<string>();

    public float GetAxis(string name){
        return Input.GetAxis(name);
    }

    public float GetAxisRaw(string name){
        return Input.GetAxisRaw(name);
    }

    public bool GetAxisPressed(string name){
        if(Pressed.IndexOf(name) >= 0){
            if(Input.GetAxisRaw(name) == 0)
                Pressed.Remove(name);
            return false;
        }
            
        
        if(Input.GetAxisRaw(name) != 0){
            Pressed.Add(name);
            return true;
        }else{
            return false;
        }

    }
}

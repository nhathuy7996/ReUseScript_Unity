using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : Singleton<FacebookManager>
{
    void Awake ()
    {
        
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
            //Handle FB.Init
            FB.Init( () => {
                FB.ActivateApp();

            });
        }
       
    }


    void Start()
    {
        DontDestroyOnLoad(this);
    }



    public void LogEvent(string EventName){
    
        try{
            if (FB.IsInitialized) {
                FB.LogAppEvent(EventName,1f);
            } else {
                //Handle FB.Init
                FB.Init( () => {
                    FB.ActivateApp();
                    FB.LogAppEvent(EventName,1f);
                });
            }
        }catch(System.Exception e){
            Debug.LogError(e);
        }
     
        
    }
}

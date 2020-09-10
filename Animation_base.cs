using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_base : MonoBehaviour
{
    Animation Anim = null;
    void OnEnable(){
        StartCoroutine(Wait());
    }
    // Start is called before the first frame update
    void Awake()
    {
        Anim = this.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Wait(){
        yield return new WaitUntil(()=> !Anim.isPlaying);
        this.gameObject.SetActive(false);
    }
}

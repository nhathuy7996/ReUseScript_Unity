using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticalControll : MonoBehaviour
{
    ParticleSystem[] sub = null;
    void Awake(){
        sub = GetComponentsInChildren<ParticleSystem>();
    }
    // Start is called before the first frame update

    void OnEnable(){
        StartCoroutine(CheckIfAlive());
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CheckIfAlive ()
	{
		ParticleSystem ps = this.GetComponent<ParticleSystem>();

        yield return new WaitUntil(()=>!ps.IsAlive(true));
        foreach(ParticleSystem p in sub){
            yield return new WaitUntil(()=>!p.IsAlive(true));
        }
        this.gameObject.SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySound : MonoBehaviour
{
    [SerializeField]
    AudioSource _Audio = null;
    public AudioSource Audio => _Audio;
    // Start is called before the first frame update
    void Awake()
    {
        _Audio = this.GetComponent<AudioSource>();
        
    }

    void Start(){
        StartCoroutine(WaitDone());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitDone(){
        yield return new WaitUntil(()=>!_Audio.isPlaying);
        this.gameObject.SetActive(false);
    }
}

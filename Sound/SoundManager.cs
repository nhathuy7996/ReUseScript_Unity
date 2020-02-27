using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioListener))]
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    DestroySound Audio_Prefab = null;
    [SerializeField]
    List<DestroySound> AudioS = new List<DestroySound>();
    [SerializeField]
    List<AudioClip> SoundFiles = new List<AudioClip>();

    void Start(){
        Object[] file = Resources.LoadAll("Sound", typeof(AudioClip));
        foreach(Object o in file){
            SoundFiles.Add((AudioClip)o);
        }
    }

    public void PlaySound(string nameSound){
        foreach(AudioClip A in SoundFiles){
            if(A.name.ToLower() != nameSound.ToLower())
                continue;
            AudioSource source = GetAudioSource();

            source.clip = A;
            source.gameObject.SetActive(true);
        }
    }
    AudioSource GetAudioSource(){
        foreach(DestroySound D in AudioS){
            if(D.gameObject.activeSelf)
                continue;
            return D.Audio;
        }

        DestroySound D2 = Instantiate(Audio_Prefab,this.transform.position,Quaternion.identity,this.transform).GetComponent<DestroySound>();
        AudioS.Add(D2);
        D2.gameObject.SetActive(false);

        return D2.Audio;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource audio;

    public static SFXManager sfxInstance;

    public AudioClip menuOk;
    
    public AudioClip menuCancel;
    
    public AudioClip menuCant;
    
    public AudioClip menuMove;
    
    public AudioClip pause;
    private void Awake()
    {
        if (sfxInstance != null & sfxInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        sfxInstance = this;
        DontDestroyOnLoad(this);
        
    }

    public void PlayCantSound()
    {
        audio.PlayOneShot(menuCant);
    }

    public void PlayOkSound()
    {
        audio.PlayOneShot(menuOk);  
    }
    
    public void PlayMoveSound()
    {
        audio.PlayOneShot(menuMove);  
    }
    
    public void PlayCancelSound()
    {
        audio.PlayOneShot(menuCancel);  
    }
    
    public void PlayPauseSound()
    {
        audio.PlayOneShot(pause);  
    }
}

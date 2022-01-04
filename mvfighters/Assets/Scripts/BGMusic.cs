using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public AudioSource audio;

    public static BGMusic bgmInstance;

    private void Awake()
    {
        if (bgmInstance != null & bgmInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        bgmInstance = this;
        DontDestroyOnLoad(this);
        
    }
}

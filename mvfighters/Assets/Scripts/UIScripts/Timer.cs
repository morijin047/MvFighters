using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float current;
    public Timer(float timerStart)
    {
        current = timerStart;
    }

    public void DecreaseTimer(float delay)
    {
        current -= Time.deltaTime * delay;
        //current = (int) current;
        if (current < 0)
        {
            current = 0;
            MainScript.instance.um.inGame.DisplayRoundText("TIMES UP!");
            MainScript.instance.um.inGame.StartCoroutineRoundText(2f);
            SFXManager.sfxInstance.audio.PlayOneShot(MainScript.instance.um.inGame.narratorVoices[3]);
            MainScript.instance.fm.CheckWinner();
        }
    }
}

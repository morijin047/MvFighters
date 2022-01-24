using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VsScreen : MonoBehaviour
{

    private bool twoPlayer;

    private GameState previousState;

    public AudioClip vsBGM;
    
    private IEnumerator vsScreenCoroutine;
    
    public Image cp1;
    
    public Image cp2;
    
    public List<Sprite> characterPortrait;
    
    public void VsScreenAppear(bool isTwoplayer, string cp1Name, string cp2Name)
    {
        this.twoPlayer = isTwoplayer;
        gameObject.SetActive(true);
        previousState = MainScript.instance.state;
        MainScript.instance.state = GameState.VsScreen;
        cp1.sprite = FindCharacterPortrait(cp1Name);
        cp2.sprite = FindCharacterPortrait(cp2Name);
        BGMusic.bgmInstance.audio.Stop();
        BGMusic.bgmInstance.audio.PlayOneShot(vsBGM);
        MainScript.instance.DisableMenuControls();
        vsScreenCoroutine = VsScreenFinish();
        StartCoroutine(vsScreenCoroutine);
        //VsScreenFinish();
    }

    private IEnumerator VsScreenFinish()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            MainScript.instance.GameStart(twoPlayer, previousState);
            MainScript.instance.um.css.LoadPrefabsInScene();
            MainScript.instance.um.inGame.InitiateInGameUI();
            MainScript.instance.fm.RoundStart();
            gameObject.SetActive(false); 
            
        }
    }
    
    public Sprite FindCharacterPortrait(string name)
    {
        name = name.Split("(Clone")[0];
        foreach (var c in characterPortrait)
        {
            if (c.name.Contains(name))
            {
                return c;
            }
        }

        return null;
    }
}

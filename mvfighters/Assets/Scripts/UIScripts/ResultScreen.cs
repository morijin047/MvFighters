using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    public TMP_Text playerWinPort;

    public TMP_Text playerWinName;

    public TMP_Text playerWinQuote;

    private string playerLostName;

    private bool quoteFinish;

    public GameState previousGameState;

    public AudioClip victoryBGM;

    public GameObject resultOptions;

    public List<Sprite> portraits;

    public Image winnerPortrait;

    [HideInInspector] private string lastSelection;

    public void IninitateResultScreen(FighterS playerWin, FighterS playerLost)
    {
        if(!MainS.instance.player1.UI.enabled)
            MainS.instance.player1.UI.Enable();
        previousGameState = MainS.instance.state;
        MainS.instance.state = GameState.ResultScreen;
        playerWinPort.text = playerWin.GetPort().ToString();
        playerWinName.text = playerWin.stats.name;
        playerLostName = playerLost.stats.name;
        playerWinQuote.text = playerWin.stats.winningQuotes[0];
        quoteFinish = false;
        winnerPortrait.sprite = FindCharacterPortrait(playerWinName.text);
        BGMusic.bgmInstance.audio.clip = victoryBGM;
        BGMusic.bgmInstance.audio.Play();
        quoteFinish = true;
    }
    public void UpdateResultScreen()
    {
        if (quoteFinish)
        {
            if (lastSelection == null)
                lastSelection = MainS.instance.um.eventSystem.currentSelectedGameObject.name;

            if (MainS.instance.um.eventSystem.currentSelectedGameObject.name != lastSelection)
            {
                lastSelection = MainS.instance.um.eventSystem.currentSelectedGameObject.name;
                SFXManager.sfxInstance.PlayMoveSound();
            }
        }
    }
    
    public Sprite FindCharacterPortrait(string name)
    {
        
        foreach (var c in portraits)
        {
            if (c.name.Contains(name))
            {
                return c;
            }
        }

        return null;
    }
}

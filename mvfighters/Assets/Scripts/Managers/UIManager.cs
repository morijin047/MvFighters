using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas canvas;

    [HideInInspector] public MainMenu mainMenu;

    public GameObject mainMenuUI;

    [HideInInspector] public Css css;

    public GameObject cssUI;
    
    [HideInInspector] public VsScreen vsScreen;

    public GameObject vsScreenUI;

    [HideInInspector] public InGame inGame;

    public GameObject inGameUI;

    [HideInInspector] public PauseMenu pause;

    public GameObject pauseUI;
    
    [HideInInspector] public ResultScreen result;

    public GameObject resultUI;

    public EventSystem eventSystem;

    private bool was2Player;

    public void StartMenuUpdating()
    {
        mainMenuUI.SetActive(true);
        mainMenu = mainMenuUI.GetComponent<MainMenu>();
        css = cssUI.GetComponent<Css>();
        pause = pauseUI.GetComponent<PauseMenu>();
        inGame = inGameUI.GetComponent<InGame>();
        vsScreen = vsScreenUI.GetComponent<VsScreen>();
        result = resultUI.GetComponent<ResultScreen>();
        MainS.instance.player1.UI.Enable();
        if (cssUI.activeInHierarchy)
        {
            cssUI.SetActive(false);
            MainS.instance.player1.MenuMovement1.Disable();
            if (MainS.instance.player2.MenuMovement2.enabled)
                MainS.instance.player2.MenuMovement2.Disable();
        }
        eventSystem.SetSelectedGameObject(mainMenu.mainMenu.GetComponentInChildren<Button>().gameObject);
        BGMusic.bgmInstance.audio.clip = mainMenu.mainMenuBGM;
        BGMusic.bgmInstance.audio.Play();
    }

    public void UpdateMenu()
    {
        mainMenu.UpdateMainMenu();
    }

    public void UpdateCSS()
    {
        css.UpdateCss();
    }

    public void UpdateInGameUI()
    {
        inGame.UpdateInGameUI();
    }

    public void UpdateResultScreen()
    {
        result.UpdateResultScreen();
    }

    public void GoToResultScreen(FighterS playerWin, FighterS playerLost)
    {
        inGameUI.SetActive(false);
        resultUI.SetActive(true);
        eventSystem.SetSelectedGameObject(result.resultOptions.GetComponentInChildren<Button>().gameObject);
        result.IninitateResultScreen(playerWin, playerLost);
    }

    public void ClickOption(String optionSelected)
    {
        MenuSelection newMenu;
        if (MenuSelection.TryParse(optionSelected, out newMenu))
        {
            mainMenu.GoTo(newMenu, false);
        }

        VersusSelection vs;
        if (VersusSelection.TryParse(optionSelected, out vs))
        {
            mainMenu.GoToCss(vs.ToString());
            mainMenuUI.SetActive(false);
            cssUI.SetActive(true);
        }

        OnlineSelection online;
        if (OnlineSelection.TryParse(optionSelected, out online))
        {
            if (online == OnlineSelection.RankedMode)
            {
                mainMenu.GoToCss(online.ToString());
                mainMenuUI.SetActive(false);
                cssUI.SetActive(true);
            }
        }

        TrainingSelection training;
        if (TrainingSelection.TryParse(optionSelected, out training))
        {
            if (training == TrainingSelection.FreeTraining)
            {
                mainMenu.GoToCss(training.ToString());
                mainMenuUI.SetActive(false);
                cssUI.SetActive(true);
            }
        }

        OptionSelection setting;
        if (OptionSelection.TryParse(optionSelected, out setting))
        {
            //mainMenu.SetActive(false);
            //activate PopMenu SETTING Menu
        }

        PauseSelection pause;
        if (PauseSelection.TryParse(optionSelected, out pause))
        {
            if (pause == PauseSelection.Resume)
            {
                MainS.instance.SetPause(false);
            }

            if (pause == PauseSelection.CharacterSelect)
            {
                MainS.instance.SetPause(false);
                inGame.TurnOffIngameUI();
                switch (MainS.instance.state)
                {
                    case GameState.Combat :
                        MainS.instance.fm.MatchOver();
                        MainS.instance.fm.DeleteCurrentFighter();
                        MainS.instance.SetPause(false);
                        MainS.instance.um.DeactivatePauseMenu();
                        MainS.instance.state = GameState.Css;
                        MainS.instance.um.cssUI.SetActive(true);
                        MainS.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        if (MainS.instance.fm.twoPlayer)
                        {
                            MainS.instance.um.mainMenu.GoToCss(VersusSelection.VsPlayer.ToString());
                            MainS.instance.player2.MenuMovement2.Enable();
                        }
                        else
                        {
                            MainS.instance.um.mainMenu.GoToCss(VersusSelection.VsCom.ToString());
                        }
                        break;
                }
            }

            if (pause == PauseSelection.MainMenu)
            {
                MainS.instance.fm.MatchOver(); 
                MainS.instance.fm.DeleteCurrentFighter();
                MainS.instance.SetPause(false);
                DeactivatePauseMenu();
                inGame.TurnOffIngameUI();
                MainS.instance.state = GameState.Menu;
                StartMenuUpdating();
            }
        }

        ResultSelection result;
        if (ResultSelection.TryParse(optionSelected, out result))
        {
            if (result == ResultSelection.Retry)
            {
                MainS.instance.state = this.result.previousGameState;
                MainS.instance.fm.RoundStart();
                inGame.InitiateInGameUI();
                this.resultUI.SetActive(false);
            }

            if (result == ResultSelection.RCharacterSelect)
            {
                resultUI.SetActive(false);
                switch (this.result.previousGameState)
                {
                    case GameState.Combat :
                        MainS.instance.fm.MatchOver();
                        MainS.instance.fm.DeleteCurrentFighter();
                        MainS.instance.state = GameState.Css;
                        MainS.instance.um.cssUI.SetActive(true);
                        MainS.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        if (MainS.instance.fm.twoPlayer)
                        {
                            MainS.instance.um.mainMenu.GoToCss(VersusSelection.VsPlayer.ToString());
                            MainS.instance.player2.MenuMovement2.Enable();
                        }
                        else
                        {
                            MainS.instance.um.mainMenu.GoToCss(VersusSelection.VsCom.ToString());
                        }
                        break;
                }
            }

            if (result == ResultSelection.RMainMenu)
            {
                MainS.instance.fm.MatchOver(); 
                MainS.instance.fm.DeleteCurrentFighter();
                resultUI.SetActive(false);
                MainS.instance.state = GameState.Menu;
                StartMenuUpdating();
                mainMenu.GoTo(MenuSelection.MainMenu, false);
            }
        }
    }

    public void Submit()
    {
        eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }

    public void ActivatePauseMenu()
    {
        if (!pauseUI.activeInHierarchy)
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0;
            MainS.instance.player1.Combat1.Disable();
            if (MainS.instance.player2.Combat2.enabled)
            {
                MainS.instance.player2.Combat2.Disable();
                was2Player = true;
            }

            MainS.instance.player1.UI.Enable();
            eventSystem.SetSelectedGameObject(pauseUI.GetComponentInChildren<Button>().gameObject);
            SFXManager.sfxInstance.PlayPauseSound();
        }
        else
        {
            pause.UpdatePause();
        }
    }

    public void DeactivatePauseMenu()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
        MainS.instance.player1.Combat1.Enable();
        if (was2Player)
        {
            MainS.instance.player2.Combat2.Enable();
        }

        MainS.instance.player1.UI.Disable();
        was2Player = false;
    }
}
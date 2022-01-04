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

    [HideInInspector] public MainMenu menu;

    public GameObject mainMenu;

    [HideInInspector] public Css css;

    public GameObject cssMenu;

    [HideInInspector] public InGame inGame;

    public GameObject inGameUI;

    [HideInInspector] public PauseMenu pause;

    public GameObject pauseMenu;

    public Button temp;

    public EventSystem eventSystem;

    private bool was2player;

    public void StartMenuUpdating()
    {
        mainMenu.SetActive(true);
        menu = mainMenu.GetComponent<MainMenu>();
        css = cssMenu.GetComponent<Css>();
        pause = pauseMenu.GetComponent<PauseMenu>();
        inGame = inGameUI.GetComponent<InGame>();
        MainS.instance.player1.UI.Enable();
        if (cssMenu.activeInHierarchy)
        {
            cssMenu.SetActive(false);
            MainS.instance.player1.MenuMovement1.Disable();
            if (MainS.instance.player2.MenuMovement2.enabled)
                MainS.instance.player2.MenuMovement2.Disable();
        }
        BGMusic.bgmInstance.audio.clip = menu.mainMenuBGM;
        BGMusic.bgmInstance.audio.Play();
    }

    public void UpdateMenu()
    {
        menu.UpdateMainMenu();
    }

    public void UpdateCSS()
    {
        css.UpdateCss();
    }

    public void ClickOption(String optionSelected)
    {
        MenuSelection newMenu;
        if (MenuSelection.TryParse(optionSelected, out newMenu))
        {
            menu.GoTo(newMenu);
        }

        VersusSelection vs;
        if (VersusSelection.TryParse(optionSelected, out vs))
        {
            menu.GoToCss(vs.ToString());
            mainMenu.SetActive(false);
            cssMenu.SetActive(true);
        }

        OnlineSelection online;
        if (OnlineSelection.TryParse(optionSelected, out online))
        {
            if (online == OnlineSelection.RankedMode)
            {
                menu.GoToCss(online.ToString());
                mainMenu.SetActive(false);
                cssMenu.SetActive(true);
            }
        }

        TrainingSelection training;
        if (TrainingSelection.TryParse(optionSelected, out training))
        {
            if (training == TrainingSelection.FreeTraining)
            {
                menu.GoToCss(training.ToString());
                mainMenu.SetActive(false);
                cssMenu.SetActive(true);
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
                switch (MainS.instance.state)
                {
                    case GameState.Combat :
                        MainS.instance.fm.MatchOver(true);
                        MainS.instance.SetPause(false);
                        MainS.instance.um.DeactivatePauseMenu();
                        MainS.instance.state = GameState.Css;
                        MainS.instance.um.cssMenu.SetActive(true);
                        MainS.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        if (MainS.instance.fm.twoPlayer)
                        {
                            MainS.instance.um.menu.GoToCss(VersusSelection.VsPlayer.ToString());
                            MainS.instance.player2.MenuMovement2.Enable();
                        }
                        else
                        {
                            MainS.instance.um.menu.GoToCss(VersusSelection.VsCom.ToString());
                        }
                        break;
                }
            }
        }
    }

    public void Submit()
    {
        eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }

    public void ActivatePauseMenu()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            MainS.instance.player1.Combat1.Disable();
            if (MainS.instance.player2.Combat2.enabled)
            {
                MainS.instance.player2.Combat2.Disable();
                was2player = true;
            }

            MainS.instance.player1.UI.Enable();
            eventSystem.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
        }
        else
        {
            pause.UpdatePause();
        }
    }

    public void DeactivatePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        MainS.instance.player1.Combat1.Enable();
        if (was2player)
        {
            MainS.instance.player2.Combat2.Enable();
        }

        MainS.instance.player1.UI.Disable();
        was2player = false;
    }
}
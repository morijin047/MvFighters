using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

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

    [HideInInspector] public PauseMenu pauseTraining;

    public GameObject pauseTrainingUI;

    [HideInInspector] public ResultScreen result;

    public GameObject resultUI;

    public EventSystem eventSystem;

    private bool was2Player;

    [HideInInspector] public TrainingMode training;

    public GameObject trainingWindow;

    public void StartMenuUpdating()
    {
        mainMenuUI.SetActive(true);
        mainMenu = mainMenuUI.GetComponent<MainMenu>();
        css = cssUI.GetComponent<Css>();
        pause = pauseUI.GetComponent<PauseMenu>();
        pauseTraining = pauseTrainingUI.GetComponent<PauseMenu>();
        inGame = inGameUI.GetComponent<InGame>();
        vsScreen = vsScreenUI.GetComponent<VsScreen>();
        result = resultUI.GetComponent<ResultScreen>();
        training = trainingWindow.GetComponent<TrainingMode>();
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
        //Debug.Log(eventSystem.currentSelectedGameObject.name);
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
        MenuSelectionTryParse(optionSelected);

        VersusSelectionTryParse(optionSelected);

        OnlineSelectionTryParse(optionSelected);

        TrainingSelectionTryParse(optionSelected);

        OptionSelectionTryParse(optionSelected);

        PauseSelectionTryParse(optionSelected);

        ResultSelectionTryParse(optionSelected);
    }

    public void MenuSelectionTryParse(string optionSelected)
    {
        MenuSelection newMenu;
        if (MenuSelection.TryParse(optionSelected, out newMenu))
        {
            mainMenu.GoTo(newMenu, false);
        }
    }

    public void VersusSelectionTryParse(string optionSelected)
    {
        VersusSelection vs;
        if (VersusSelection.TryParse(optionSelected, out vs))
        {
            mainMenu.GoToCss(vs.ToString());
            mainMenuUI.SetActive(false);
            cssUI.SetActive(true);
        }
    }

    public void OnlineSelectionTryParse(string optionSelected)
    {
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
    }

    public void TrainingSelectionTryParse(string optionSelected)
    {
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
    }

    public void OptionSelectionTryParse(string optionSelected)
    {
        OptionSelection setting;
        if (OptionSelection.TryParse(optionSelected, out setting))
        {
            //mainMenu.SetActive(false);
            //activate PopMenu SETTING Menu
        }
    }

    public void PauseSelectionTryParse(string optionSelected)
    {
        PauseSelection pause;
        if (PauseSelection.TryParse(optionSelected, out pause))
        {
            if (pause == PauseSelection.Resume)
            {
                MainS.instance.SetPause(false);
            }

            if (pause == PauseSelection.Reset)
            {
                training.ResetPositions();
                MainS.instance.SetPause(false);
            }

            if (pause == PauseSelection.TrainingSetting)
            {
                eventSystem.SetSelectedGameObject(trainingWindow.GetComponentInChildren<Button>().gameObject);
                //pauseTrainingUI.SetActive(false);
                trainingWindow.SetActive(true);
            }

            if (pause == PauseSelection.CharacterSelect)
            {
                MainS.instance.SetPause(false);
                inGame.TurnOffIngameUI();
                switch (MainS.instance.state)
                {
                    case GameState.Combat:
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
                    case GameState.TrainingCombat:
                        MainS.instance.fm.MatchOver();
                        MainS.instance.fm.DeleteCurrentFighter();
                        MainS.instance.SetPause(false);
                        MainS.instance.um.DeactivatePauseMenu();
                        MainS.instance.state = GameState.TrainingCss;
                        MainS.instance.um.cssUI.SetActive(true);
                        MainS.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        MainS.instance.um.mainMenu.GoToCss(TrainingSelection.FreeTraining.ToString());
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
    }

    public void ResultSelectionTryParse(string optionSelected)
    {
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
                    case GameState.Combat:
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
        if (eventSystem.currentSelectedGameObject.GetComponent<Button>() != null)
        {
            eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            return;
        }
            
        if (eventSystem.currentSelectedGameObject.GetComponent<TMP_Dropdown>() != null)
        {
            eventSystem.currentSelectedGameObject.GetComponent<TMP_Dropdown>().Show();
            return;
        }
            
        if (training.enemyState.IsExpanded)
        {
            training.DropDownClick(eventSystem.currentSelectedGameObject.name);
            eventSystem.SetSelectedGameObject(trainingWindow.GetComponentInChildren<TMP_Dropdown>().gameObject);
            training.enemyState.Hide();
            return;
        }
    }

    public void ActivatePauseMenu()
    {
        if (!pauseUI.activeInHierarchy && !pauseTrainingUI.activeInHierarchy)
        {
            if (MainS.instance.state == GameState.TrainingCombat)
            {
                pauseTrainingUI.SetActive(true);
                eventSystem.SetSelectedGameObject(pauseTrainingUI.GetComponentInChildren<Button>().gameObject);
            }
            else
            {
                pauseUI.SetActive(true);
                eventSystem.SetSelectedGameObject(pauseUI.GetComponentInChildren<Button>().gameObject);
            }

            Time.timeScale = 0;
            MainS.instance.player1.Combat1.Disable();
            if (MainS.instance.player2.Combat2.enabled)
            {
                MainS.instance.player2.Combat2.Disable();
                was2Player = true;
            }

            MainS.instance.player1.UI.Enable();
            SFXManager.sfxInstance.PlayPauseSound();
        }
        else
        {
            if (MainS.instance.state == GameState.TrainingCombat)
            {
                pauseTraining.UpdatePause();
            }
            else
            {
                pause.UpdatePause();
            }
        }
    }

    public void DeactivatePauseMenu()
    {
        if (pauseUI.activeInHierarchy)
            pauseUI.SetActive(false);
        if (pauseTrainingUI.activeInHierarchy)
            pauseTrainingUI.SetActive(false);
        if (trainingWindow.activeInHierarchy)
            trainingWindow.SetActive(false);
        Time.timeScale = 1;
        MainS.instance.player1.Combat1.Enable();
        if (was2Player)
        {
            MainS.instance.player2.Combat2.Enable();
        }

        MainS.instance.player1.UI.Disable();
        was2Player = false;
    }

    public void CancelSelection()
    {
        if (MainS.instance.state == GameState.Menu)
            mainMenu.GoBack();
        if (MainS.instance.state is GameState.Combat or GameState.NetworkCombat or GameState.TrainingCombat)
        {
            if (training.enemyState.IsExpanded)
            {
                training.enemyState.Hide();
                eventSystem.SetSelectedGameObject(trainingWindow.GetComponentInChildren<TMP_Dropdown>().gameObject);
            }
            else if (trainingWindow.activeInHierarchy)
            {
                trainingWindow.SetActive(false);
                eventSystem.SetSelectedGameObject(pauseTrainingUI.GetComponentInChildren<Button>().gameObject);
            }
            else if (pauseUI.activeInHierarchy || pauseTrainingUI.activeInHierarchy)
            {
                MainS.instance.SetPause(false);
            }
        }
    }
}
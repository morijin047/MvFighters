using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

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

    public GameObject soundUI;

    public GameObject displayUI;

    public GameObject buttonUI;


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
        pauseTraining.training = pauseTraining.trainingWindow.GetComponent<TrainingMode>();
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
        if (soundUI.activeInHierarchy)
            MainS.instance.settings.soundSettings.UpdateSoundTextUI();
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
            if (setting == OptionSelection.SoundSetting)
            {
                Debug.Log("Invoked");
                soundUI.SetActive(true);
                mainMenu.DisableTemporaryMenu();
                mainMenu.Transition(false);
                eventSystem.SetSelectedGameObject(soundUI.GetComponentInChildren<Button>().gameObject);
                MainS.instance.settings.soundSettings.OpenSoundSettings();
            }

            if (setting == OptionSelection.SaveVolume)
            {
                MainS.instance.settings.soundSettings.ApplySoundSettings();
            }

            if (setting == OptionSelection.MuteVolume)
            {
                MainS.instance.settings.soundSettings.MuteSoundSettings();
            }

            if (setting == OptionSelection.DisplaySetting)
            {
                displayUI.SetActive(true);
                mainMenu.DisableTemporaryMenu();
                mainMenu.Transition(false);
                eventSystem.SetSelectedGameObject(displayUI.GetComponentInChildren<Button>().gameObject);
            }

            if (setting == OptionSelection.ResetResolution)
            {
                MainS.instance.settings.resolutionSettings.DefaultSettings();
            }

            if (setting == OptionSelection.ApplyResolution)
            {
                MainS.instance.settings.resolutionSettings.ApplySetting();
            }

            if (setting == OptionSelection.ButtonSetting)
            {
                buttonUI.SetActive(true);
                mainMenu.DisableTemporaryMenu();
                mainMenu.Transition(false);
                eventSystem.SetSelectedGameObject(buttonUI.GetComponentInChildren<Button>().gameObject);
            }
        }
    }

    public void PauseSelectionTryParse(string optionSelected)
    {
        PauseSelection pause;
        if (PauseSelection.TryParse(optionSelected, out pause))
        {
            SFXManager.sfxInstance.PlayOkSound();
            if (pause == PauseSelection.Resume)
            {
                MainS.instance.SetPause(false);
            }

            if (pause == PauseSelection.Reset)
            {
                pauseTraining.training.ResetPositions();
                MainS.instance.SetPause(false);
            }

            if (pause == PauseSelection.TrainingSetting)
            {
                eventSystem.SetSelectedGameObject(pauseTraining.trainingWindow.GetComponentInChildren<Button>()
                    .gameObject);
                //pauseTrainingUI.SetActive(false);
                pauseTraining.trainingWindow.SetActive(true);
            }

            if (pause == PauseSelection.SoundSettingPause)
            {
                if (MainS.instance.state == GameState.TrainingCombat)
                {
                    eventSystem.SetSelectedGameObject(
                        pauseTraining.soundPauseUI.GetComponentInChildren<Button>().gameObject
                    );
                    pauseTraining.soundPauseUI.SetActive(true);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(
                        this.pause.soundPauseUI.GetComponentInChildren<Button>().gameObject
                    );
                    this.pause.soundPauseUI.SetActive(true);
                }
                MainS.instance.settings.soundPauseSettings.OpenSoundSettings();
            }

            if (pause == PauseSelection.SaveVolumePause)
            {
                MainS.instance.settings.soundPauseSettings.ApplySoundSettings();
            }

            if (pause == PauseSelection.MuteVolumePause)
            {
                MainS.instance.settings.soundPauseSettings.MuteSoundSettings();
            }

            if (pause == PauseSelection.ButtonSettingPause)
            {
                if (MainS.instance.state == GameState.TrainingCombat)
                {
                    eventSystem.SetSelectedGameObject(pauseTraining.buttonPauseUI.GetComponentInChildren<Button>()
                        .gameObject);
                    pauseTraining.buttonPauseUI.SetActive(true);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(this.pause.buttonPauseUI.GetComponentInChildren<Button>()
                        .gameObject);
                    this.pause.buttonPauseUI.SetActive(true);
                }
               
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

    public void Submit(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }

       // Debug.Log("Submit");
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

        if (pauseTraining.training.enemyState.IsExpanded)
        {
            pauseTraining.training.DropDownClick(eventSystem.currentSelectedGameObject.name);
            eventSystem.SetSelectedGameObject(pauseTraining.trainingWindow.GetComponentInChildren<TMP_Dropdown>()
                .gameObject);
            pauseTraining.training.enemyState.Hide();
            return;
        }

        if (eventSystem.currentSelectedGameObject.GetComponent<Toggle>() != null)
        {
            var currentToggle = eventSystem.currentSelectedGameObject.GetComponent<Toggle>();
            currentToggle.isOn = !currentToggle.isOn;
            return;
        }

        if (MainS.instance.settings.resolutionSettings.resolution.IsExpanded)
        {
            MainS.instance.settings.resolutionSettings.ClickDropdown();
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
        if (pauseTraining.trainingWindow.activeInHierarchy)
            pauseTraining.trainingWindow.SetActive(false);
        if (pauseTraining.buttonPauseUI.activeInHierarchy)
            pauseTraining.buttonPauseUI.SetActive(false);
        if (pause.buttonPauseUI.activeInHierarchy)
            pause.buttonPauseUI.SetActive(false);
        if (pauseTraining.soundPauseUI.activeInHierarchy)
            pauseTraining.soundPauseUI.SetActive(false);
        if (pause.soundPauseUI.activeInHierarchy)
            pause.soundPauseUI.SetActive(false);
        Time.timeScale = 1;
        MainS.instance.player1.Combat1.Enable();
        if (was2Player)
        {
            MainS.instance.player2.Combat2.Enable();
        }

        MainS.instance.player1.UI.Disable();
        was2Player = false;
    }

    public void CancelSelection(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }

        if (MainS.instance.state == GameState.Menu)
        {
            if (soundUI.activeInHierarchy)
            {
                mainMenu.GoTo(MenuSelection.Options, true);
                soundUI.SetActive(false);
            }
            else if (displayUI.activeInHierarchy)
            {
                mainMenu.GoTo(MenuSelection.Options, true);
                displayUI.SetActive(false);
            }
            else if (buttonUI.activeInHierarchy)
            {
                mainMenu.GoTo(MenuSelection.Options, true);
                buttonUI.SetActive(false);
            }
            else
            {
                mainMenu.GoBack();
            }
        }

        if (MainS.instance.state is GameState.Combat or GameState.NetworkCombat or GameState.TrainingCombat)
        {
            if (pauseTraining.training.enemyState.IsExpanded)
            {
                pauseTraining.training.enemyState.Hide();
                eventSystem.SetSelectedGameObject(pauseTraining.trainingWindow.GetComponentInChildren<TMP_Dropdown>()
                    .gameObject);
            }
            else if (pauseTraining.trainingWindow.activeInHierarchy)
            {
                pauseTraining.trainingWindow.SetActive(false);
                eventSystem.SetSelectedGameObject(pauseTrainingUI.GetComponentInChildren<Button>().gameObject);
            }
            else if (pause.buttonPauseUI.activeInHierarchy)
            {
                pause.buttonPauseUI.SetActive(false);
                eventSystem.SetSelectedGameObject(pauseUI.GetComponentInChildren<Button>().gameObject);
            }
            else if (pauseTraining.buttonPauseUI.activeInHierarchy)
            {
                pauseTraining.buttonPauseUI.SetActive(false);
                eventSystem.SetSelectedGameObject(pauseTrainingUI.GetComponentInChildren<Button>().gameObject);
            }
            else if (pause.soundPauseUI.activeInHierarchy)
            {
                pause.soundPauseUI.SetActive(false);

                eventSystem.SetSelectedGameObject(pauseUI.GetComponentInChildren<Button>().gameObject);
            }
            else if (pauseTraining.soundPauseUI.activeInHierarchy)
            {
                pauseTraining.soundPauseUI.SetActive(false);
                eventSystem.SetSelectedGameObject(pauseTrainingUI.GetComponentInChildren<Button>().gameObject);
            }
            else if (pauseUI.activeInHierarchy || pauseTrainingUI.activeInHierarchy)
            {
                MainS.instance.SetPause(false);
            }
        }
    }
}
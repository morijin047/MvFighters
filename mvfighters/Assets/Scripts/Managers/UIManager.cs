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


    //fUNCTION that prepares all the object necessary to navigate the UI of the game
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
        MainScript.instance.player1.UI.Enable();
        if (cssUI.activeInHierarchy)
        {
            cssUI.SetActive(false);
            MainScript.instance.player1.MenuMovement1.Disable();
            if (MainScript.instance.player2.MenuMovement2.enabled)
                MainScript.instance.player2.MenuMovement2.Disable();
        }

        eventSystem.SetSelectedGameObject(mainMenu.mainMenu.GetComponentInChildren<Button>().gameObject);
        BGMusic.bgmInstance.audio.clip = mainMenu.mainMenuBGM;
        BGMusic.bgmInstance.audio.Play();
    }

    //Update function for the menu
    public void UpdateMenu()
    {
        mainMenu.UpdateMainMenu();
        if (soundUI.activeInHierarchy)
            MainScript.instance.settings.soundSettings.UpdateSoundTextUI();
    }

    //Update function for the css
    public void UpdateCSS()
    {
        css.UpdateCss();
    }

    //Update function for the InGame UI
    public void UpdateInGameUI()
    {
        inGame.UpdateInGameUI();
        //Debug.Log(eventSystem.currentSelectedGameObject.name);
    }

    //Update function for the Result Screen
    public void UpdateResultScreen()
    {
        result.UpdateResultScreen();
    }

    //Function to transition to the Result Screen
    public void GoToResultScreen(FighterS playerWin, FighterS playerLost)
    {
        inGameUI.SetActive(false);
        resultUI.SetActive(true);
        eventSystem.SetSelectedGameObject(result.resultOptions.GetComponentInChildren<Button>().gameObject);
        result.IninitateResultScreen(playerWin, playerLost);
    }

    /*
     *Main method that handle all the button UI OnClick Method. Each button click pass a string that get passed to this function. This string can be any Option Selection from any menu. Since each menu has 
     *different options, Multiple enums had to be created. This function tryparse the string for each enum of menu Selection that exist and find one that matches the string. Note that all the ooptions across the
     * enums  need to have unique names
     */
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

    //Menu selection logic
    public void MenuSelectionTryParse(string optionSelected)
    {
        MenuSelection newMenu;
        if (MenuSelection.TryParse(optionSelected, out newMenu))
        {
            mainMenu.GoTo(newMenu, false);
        }
    }

    //Versus selection logic
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

    //Online selection logic
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

    //Training selection logic
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

    //Option selection logic
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
                MainScript.instance.settings.soundSettings.OpenSoundSettings();
            }

            if (setting == OptionSelection.SaveVolume)
            {
                MainScript.instance.settings.soundSettings.ApplySoundSettings();
            }

            if (setting == OptionSelection.MuteVolume)
            {
                MainScript.instance.settings.soundSettings.MuteSoundSettings();
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
                MainScript.instance.settings.resolutionSettings.DefaultSettings();
            }

            if (setting == OptionSelection.ApplyResolution)
            {
                MainScript.instance.settings.resolutionSettings.ApplySetting();
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

    //Pause selection logic
    public void PauseSelectionTryParse(string optionSelected)
    {
        PauseSelection pause;
        if (PauseSelection.TryParse(optionSelected, out pause))
        {
            SFXManager.sfxInstance.PlayOkSound();
            if (pause == PauseSelection.Resume)
            {
                MainScript.instance.SetPause(false);
            }

            if (pause == PauseSelection.Reset)
            {
                pauseTraining.training.ResetPositions();
                MainScript.instance.SetPause(false);
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
                if (MainScript.instance.state == GameState.TrainingCombat)
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
                MainScript.instance.settings.soundPauseSettings.OpenSoundSettings();
            }

            if (pause == PauseSelection.SaveVolumePause)
            {
                MainScript.instance.settings.soundPauseSettings.ApplySoundSettings();
            }

            if (pause == PauseSelection.MuteVolumePause)
            {
                MainScript.instance.settings.soundPauseSettings.MuteSoundSettings();
            }

            if (pause == PauseSelection.ButtonSettingPause)
            {
                if (MainScript.instance.state == GameState.TrainingCombat)
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
                MainScript.instance.SetPause(false);
                inGame.TurnOffIngameUI();
                switch (MainScript.instance.state)
                {
                    case GameState.Combat:
                        MainScript.instance.fm.MatchOver();
                        MainScript.instance.fm.DeleteCurrentFighter();
                        MainScript.instance.SetPause(false);
                        MainScript.instance.um.DeactivatePauseMenu();
                        MainScript.instance.state = GameState.Css;
                        MainScript.instance.um.cssUI.SetActive(true);
                        MainScript.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        if (MainScript.instance.fm.twoPlayer)
                        {
                            MainScript.instance.um.mainMenu.GoToCss(VersusSelection.VsPlayer.ToString());
                            MainScript.instance.player2.MenuMovement2.Enable();
                        }
                        else
                        {
                            MainScript.instance.um.mainMenu.GoToCss(VersusSelection.VsCom.ToString());
                        }

                        break;
                    case GameState.TrainingCombat:
                        MainScript.instance.fm.MatchOver();
                        MainScript.instance.fm.DeleteCurrentFighter();
                        MainScript.instance.SetPause(false);
                        MainScript.instance.um.DeactivatePauseMenu();
                        MainScript.instance.state = GameState.TrainingCss;
                        MainScript.instance.um.cssUI.SetActive(true);
                        MainScript.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        MainScript.instance.um.mainMenu.GoToCss(TrainingSelection.FreeTraining.ToString());
                        break;
                }
            }

            if (pause == PauseSelection.MainMenu)
            {
                MainScript.instance.fm.MatchOver();
                MainScript.instance.fm.DeleteCurrentFighter();
                MainScript.instance.SetPause(false);
                DeactivatePauseMenu();
                inGame.TurnOffIngameUI();
                MainScript.instance.state = GameState.Menu;
                StartMenuUpdating();
            }
        }
    }

    //Result selection logic
    public void ResultSelectionTryParse(string optionSelected)
    {
        ResultSelection result;
        if (ResultSelection.TryParse(optionSelected, out result))
        {
            if (result == ResultSelection.Retry)
            {
                MainScript.instance.state = this.result.previousGameState;
                MainScript.instance.fm.RoundStart();
                inGame.InitiateInGameUI();
                this.resultUI.SetActive(false);
            }

            if (result == ResultSelection.RCharacterSelect)
            {
                resultUI.SetActive(false);
                switch (this.result.previousGameState)
                {
                    case GameState.Combat:
                        MainScript.instance.fm.MatchOver();
                        MainScript.instance.fm.DeleteCurrentFighter();
                        MainScript.instance.state = GameState.Css;
                        MainScript.instance.um.cssUI.SetActive(true);
                        MainScript.instance.player1.MenuMovement1.Enable();
                        //MainS.instance.um.StartMenuUpdating();
                        if (MainScript.instance.fm.twoPlayer)
                        {
                            MainScript.instance.um.mainMenu.GoToCss(VersusSelection.VsPlayer.ToString());
                            MainScript.instance.player2.MenuMovement2.Enable();
                        }
                        else
                        {
                            MainScript.instance.um.mainMenu.GoToCss(VersusSelection.VsCom.ToString());
                        }

                        break;
                }
            }

            if (result == ResultSelection.RMainMenu)
            {
                MainScript.instance.fm.MatchOver();
                MainScript.instance.fm.DeleteCurrentFighter();
                resultUI.SetActive(false);
                MainScript.instance.state = GameState.Menu;
                StartMenuUpdating();
                mainMenu.GoTo(MenuSelection.MainMenu, false);
            }
        }
    }

    //Submit event performed that triggers the event that needs to be called depending on the object selected by the event system. An example would be the OnClick() method from a button
    public void Submit(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainScript.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2)
        {
            if (!MainScript.instance.portController.CheckID(context, 2))
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

        if (MainScript.instance.settings.resolutionSettings.resolution.IsExpanded)
        {
            MainScript.instance.settings.resolutionSettings.ClickDropdown();
            return;
        }
    }

    //All the logic that happens when the pause menu is activated
    public void ActivatePauseMenu()
    {
        if (!pauseUI.activeInHierarchy && !pauseTrainingUI.activeInHierarchy)
        {
            if (MainScript.instance.state == GameState.TrainingCombat)
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
            MainScript.instance.player1.Combat1.Disable();
            if (MainScript.instance.player2.Combat2.enabled)
            {
                MainScript.instance.player2.Combat2.Disable();
                was2Player = true;
            }

            MainScript.instance.player1.UI.Enable();
            SFXManager.sfxInstance.PlayPauseSound();
        }
        else
        {
            if (MainScript.instance.state == GameState.TrainingCombat)
            {
                pauseTraining.UpdatePause();
            }
            else
            {
                pause.UpdatePause();
            }
        }
    }

    //All the logic that happens when the pause menu is deactivated
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
        MainScript.instance.player1.Combat1.Enable();
        if (was2Player)
        {
            MainScript.instance.player2.Combat2.Enable();
        }

        MainScript.instance.player1.UI.Disable();
        was2Player = false;
    }

    //Cancel event performed that goes the correct menu that preceded the current location.
    public void CancelSelection(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1)
        {
            if (!MainScript.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2)
        {
            if (!MainScript.instance.portController.CheckID(context, 2))
                return;
        }

        if (MainScript.instance.state == GameState.Menu)
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

        if (MainScript.instance.state is GameState.Combat or GameState.NetworkCombat or GameState.TrainingCombat)
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
                MainScript.instance.SetPause(false);
            }
        }
    }
}
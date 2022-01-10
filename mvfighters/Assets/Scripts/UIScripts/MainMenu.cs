using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public List<Options> options;

    public MenuSelection currentMenu;

    public GameObject mainMenu;

    public GameObject versusMenu;
    
    public GameObject onlineMenu;
    
    public GameObject trainingMenu;
    
    public GameObject optionMenu;

    public EventSystem eventSystem;

    public AudioClip mainMenuBGM;

    public string lastSelection;

    public void Start()
    {
        currentMenu = MenuSelection.MainMenu;
    }

    public void UpdateMainMenu()
    {
        switch (currentMenu)
        {
            case MenuSelection.MainMenu:
                if (!mainMenu.activeInHierarchy)
                    mainMenu.SetActive(true);
                break;
            case MenuSelection.Versus:
                if (!versusMenu.activeInHierarchy)
                    versusMenu.SetActive(true);
                break;
            case MenuSelection.Online:
                if (!onlineMenu.activeInHierarchy)
                    onlineMenu.SetActive(true);
                break;
            case MenuSelection.Training:
                if (!trainingMenu.activeInHierarchy)
                    trainingMenu.SetActive(true);
                break;
            case MenuSelection.Gallery:
                if (!mainMenu.activeInHierarchy)
                    mainMenu.SetActive(true);
                break;
            case MenuSelection.Options:
                if (!optionMenu.activeInHierarchy)
                    optionMenu.SetActive(true);
                break;
        }

        if (lastSelection == null)
            lastSelection = eventSystem.currentSelectedGameObject.name;

        if (eventSystem.currentSelectedGameObject.name != lastSelection)
        {
            lastSelection = eventSystem.currentSelectedGameObject.name;
            SFXManager.sfxInstance.PlayMoveSound();
        }
    }

    public void GoBack()
    {
        switch (currentMenu)
        {
            case MenuSelection.Versus :
                GoTo(MenuSelection.MainMenu, true);
                break;
            case MenuSelection.Online :
                GoTo(MenuSelection.MainMenu, true);
                break;
            case MenuSelection.Training :
                GoTo(MenuSelection.MainMenu, true);
                break;
            case MenuSelection.Gallery :
                GoTo(MenuSelection.MainMenu, true);
                break;
            case MenuSelection.Options :
                GoTo(MenuSelection.MainMenu, true);
                break;
        }
        
    }
    
    public void GoTo(MenuSelection newMenu, bool cancel)
    {
        switch (newMenu)
        {
            case MenuSelection.MainMenu :
                if (currentMenu != MenuSelection.MainMenu)
                    eventSystem.SetSelectedGameObject(mainMenu.GetComponentInChildren<Button>().gameObject);
                break;
            case MenuSelection.Versus :
                eventSystem.SetSelectedGameObject(versusMenu.GetComponentInChildren<Button>().gameObject);
                break;
            case MenuSelection.Online :
                eventSystem.SetSelectedGameObject(onlineMenu.GetComponentInChildren<Button>().gameObject);
                break;
            case MenuSelection.Training :
                eventSystem.SetSelectedGameObject(trainingMenu.GetComponentInChildren<Button>().gameObject);
                break;
            case MenuSelection.Options :
                eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
                break;
        }
        currentMenu = newMenu;
        Transition(cancel);
    }
    
    public void Transition(bool cancel)
    {
        if (mainMenu.activeInHierarchy)
            mainMenu.SetActive(false);
        if (versusMenu.activeInHierarchy)
            versusMenu.SetActive(false);
        if (onlineMenu.activeInHierarchy)
            onlineMenu.SetActive(false);
        if (trainingMenu.activeInHierarchy)
            trainingMenu.SetActive(false);
        if (optionMenu.activeInHierarchy)
            optionMenu.SetActive(false);
       if (cancel)
           SFXManager.sfxInstance.PlayCancelSound();
       else 
           SFXManager.sfxInstance.PlayOkSound();
    }
    
    public void GoToCss(String optionChoosed)
    {
        MainS.instance.player1.UI.Disable();
        MainS.instance.player1.MenuMovement1.Enable();
        VersusSelection vs;
        if (VersusSelection.TryParse(optionChoosed, out vs))
        {
            if (vs == VersusSelection.VsPlayer)
            {
                MainS.instance.player2.MenuMovement2.Enable();
            }
            MainS.instance.state = GameState.Css;
        }
        OnlineSelection online;
        if (OnlineSelection.TryParse(optionChoosed, out online))
        {
            if (online == OnlineSelection.RankedMode)
            {
                //MainS.instance.player2.MenuMovement2.Enable();
                MainS.instance.state = GameState.NetworkCss;
            }
        }
        TrainingSelection training;
        if (TrainingSelection.TryParse(optionChoosed, out training))
        {
            if (training == TrainingSelection.FreeTraining)
            {
                //MainS.instance.player2.MenuMovement2.Enable();
                MainS.instance.state = GameState.TrainingCss;
            }
        }
        Transition(false);
    }
}
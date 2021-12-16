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

    public EventSystem eventSystem;

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
                if (!mainMenu.activeInHierarchy)
                    mainMenu.SetActive(true);
                break;
            case MenuSelection.Training:
                if (!mainMenu.activeInHierarchy)
                    mainMenu.SetActive(true);
                break;
            case MenuSelection.Gallery:
                if (!mainMenu.activeInHierarchy)
                    mainMenu.SetActive(true);
                break;
            case MenuSelection.Options:
                if (!mainMenu.activeInHierarchy)
                    mainMenu.SetActive(true);
                break;
        }
    }

    public void GoBack()
    {
        switch (currentMenu)
        {
            case MenuSelection.Versus :
                GoTo(MenuSelection.MainMenu);
                break;
            case MenuSelection.Online :
                GoTo(MenuSelection.MainMenu);
                break;
            case MenuSelection.Training :
                GoTo(MenuSelection.MainMenu);
                break;
            case MenuSelection.Gallery :
                GoTo(MenuSelection.MainMenu);
                break;
            case MenuSelection.Options :
                GoTo(MenuSelection.MainMenu);
                break;
        }
        
    }
    
    public void GoTo(MenuSelection newMenu)
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
        }
        currentMenu = newMenu;
        Transition();
    }
    
    public void Transition()
    {
        if (mainMenu.activeInHierarchy)
            mainMenu.SetActive(false);
        if (versusMenu.activeInHierarchy)
            versusMenu.SetActive(false);
    }
    
    public void GoToCss(String optionChoosed)
    {
        MainS.instance.state = GameState.Css;
        MainS.instance.player1.UI.Disable();
        MainS.instance.player1.MenuMovement1.Enable();
        VersusSelection vs;
        if (VersusSelection.TryParse(optionChoosed, out vs))
        {
            if (vs == VersusSelection.VsPlayer)
            {
                MainS.instance.player2.MenuMovement2.Enable();
            }
                
        }
        Transition();
    }
}
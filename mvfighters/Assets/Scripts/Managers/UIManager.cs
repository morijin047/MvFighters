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
    
    [HideInInspector]
    public MainMenu menu;

    public GameObject mainMenu;
    
    [HideInInspector]
    public Css css;
    
    public GameObject cssMenu;

    private InGame inGame;
    
    public Canvas pauseMenu;
    
    public Button temp;

    public EventSystem eventSystem;
    
    

    public void StartMenuUpdating()
    {
        mainMenu.SetActive(true);
        menu = mainMenu.GetComponent<MainMenu>();
        css = cssMenu.GetComponent<Css>();
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
    }
    
    public void Submit()
    {
        eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class Css : MonoBehaviour
{
    public GameObject css;

    public MultiplayerEventSystem player1;
    
    public MultiplayerEventSystem player2;

    private bool initiated = false;
    
    private bool twoPlayer = false;

    public List<GameObject> characterPrefab;

    public List<Sprite> characterPortrait;

    private GameObject currentCharacter1;

    private bool c1Selected;

    private GameObject currentCharacter2;
    
    private bool c2Selected;

    public Image cp1;
    
    public Image cp2;

    public TMP_Text name1;
    
    public TMP_Text name2;
    public void ActivateCSS()
    {
        initiated = true;
        player1.enabled = true;
        if (MainS.instance.player2.MenuMovement2.enabled)
        {
            player2.enabled = true;
            twoPlayer = true;
        }
        else
        {
            twoPlayer = false;
        }

        c1Selected = false;
        c2Selected = false;
    }

    public void UpdateCss()
    {
        if(!initiated)
            ActivateCSS();
        if (player1.currentSelectedGameObject != null)
        {
            if (!player1.currentSelectedGameObject.name.Contains("Mystery"))
            {
                string name = player1.currentSelectedGameObject.name;
                cp1.sprite = FindCharacterPortrait(name);
                name1.text = name;
            }

            else
            {
                cp1.sprite = FindCharacterPortrait("RandomCSS");
                name1.text = "???";
            }
                
        }
        if (player2.currentSelectedGameObject != null)
        {
            if (!player2.currentSelectedGameObject.name.Contains("Mystery"))
            {
                string name = player2.currentSelectedGameObject.name;
                cp2.sprite = FindCharacterPortrait(name);
                name2.text = name;
            }
            else
            {
                cp2.sprite = FindCharacterPortrait("RandomCSS");
                name2.text = "???";
            }
                
        }
        if (c1Selected && c2Selected)
        {
            MainS.instance.GameStart(twoPlayer);
            MainS.instance.fm.StarGame(currentCharacter1, currentCharacter2, twoPlayer);
            gameObject.SetActive(false);
        }
        
    }

    public Sprite FindCharacterPortrait(string name)
    {
        
        foreach (var c in characterPortrait)
        {
            if (c.name.Contains(name))
            {
                return c;
            }
        }

        return null;
    }
    

    public void SelectCharacter(string name)
    {
        foreach (var c in characterPrefab)
        {
            if (c.name == name)
            {
                if (player1.currentSelectedGameObject.name == name)
                {
                    currentCharacter1 = c;
                    c1Selected = true;
                }
                if (twoPlayer)
                {
                    if (player2.currentSelectedGameObject.name == name)
                    {
                        currentCharacter2 = c;
                        c2Selected = true;
                    }
                }
            }
        }
    }
}

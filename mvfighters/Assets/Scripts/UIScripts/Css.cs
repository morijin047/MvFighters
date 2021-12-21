using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    public GameObject previewArea1;

    private GameObject prefabPreview1;

    public GameObject previewArea2;
    
    private GameObject prefabPreview2;

    private bool chooseForCPU;
    public void ActivateCSS()
    {
        initiated = true;
        player1.enabled = true;
        player1.GetComponent<InputSystemUIInputModule>().enabled = true;
        player1.SetSelectedGameObject(player1.firstSelectedGameObject);
        if (MainS.instance.player2.MenuMovement2.enabled)
        {
            player2.enabled = true;
            twoPlayer = true;
            player2.SetSelectedGameObject(player2.firstSelectedGameObject);
            player2.GetComponent<InputSystemUIInputModule>().enabled = true;
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
            
            foreach (var c in characterPrefab)
            {
                if (!chooseForCPU)
                {
                    if (c.name == player1.currentSelectedGameObject.name)
                    {
                        if (prefabPreview1 != null)
                        {
                            if (prefabPreview1.name != c.name + "(Clone)")
                            {
                                Destroy(prefabPreview1);
                                prefabPreview1 = Instantiate(c);
                                prefabPreview1.transform.parent = previewArea1.transform;
                                prefabPreview1.transform.localPosition = new Vector3(0, 0, 0);
                                prefabPreview1.GetComponent<Rigidbody>().isKinematic = true;
                            }
                        }
                        else
                        {
                            prefabPreview1 = Instantiate(c);
                            prefabPreview1.transform.parent = previewArea1.transform;
                            prefabPreview1.transform.localPosition = new Vector3(0, 0, 0);
                            prefabPreview1.GetComponent<Rigidbody>().isKinematic = true;
                        }
                    
                    }
                }
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
            
            foreach (var c in characterPrefab)
            {
                if (!chooseForCPU)
                {
                    if (c.name == player2.currentSelectedGameObject.name)
                    {
                        if (prefabPreview2 != null)
                        {
                            if (prefabPreview2.name != c.name + "(Clone)")
                            {
                                Destroy(prefabPreview2);
                                prefabPreview2 = Instantiate(c);
                                prefabPreview2.transform.parent = previewArea2.transform;
                                prefabPreview2.transform.localPosition = new Vector3(0, 0, 0);
                                prefabPreview2.GetComponent<Rigidbody>().isKinematic = true;

                            }
                        }
                        else
                        {
                            prefabPreview2 = Instantiate(c);
                            prefabPreview2.transform.parent = previewArea2.transform;
                            prefabPreview2.transform.localPosition = new Vector3(0, 0, 0);
                            prefabPreview2.GetComponent<Rigidbody>().isKinematic = true;

                        }
                    
                    }
                }
                else
                {
                    if (c.name == player1.currentSelectedGameObject.name)
                    {
                        if (prefabPreview2 != null)
                        {
                            if (prefabPreview2.name != c.name + "(Clone)")
                            {
                                Destroy(prefabPreview2);
                                prefabPreview2 = Instantiate(c);
                                prefabPreview2.transform.parent = previewArea2.transform;
                                prefabPreview2.transform.localPosition = new Vector3(0, 0, 0);
                                prefabPreview2.GetComponent<Rigidbody>().isKinematic = true;

                            }
                        }
                        else
                        {
                            prefabPreview2 = Instantiate(c);
                            prefabPreview2.transform.parent = previewArea2.transform;
                            prefabPreview2.transform.localPosition = new Vector3(0, 0, 0);
                            prefabPreview2.GetComponent<Rigidbody>().isKinematic = true;

                        }
                    
                    }
                }
                
            }
                
        }
        if (c1Selected && c2Selected)
        {
            MainS.instance.GameStart(twoPlayer);
            MainS.instance.fm.StarGame(currentCharacter1, currentCharacter2, twoPlayer);
            gameObject.SetActive(false);
            initiated = false;
            c1Selected = false;
            c2Selected = false;
            player1.enabled = false;
            if (player2.enabled)
                player2.enabled = false;
            twoPlayer = false;
            chooseForCPU = false;
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
                if (twoPlayer)
                {
                    if (player2.currentSelectedGameObject.name == name)
                    {
                        currentCharacter2 = c;
                        c2Selected = true;
                        player2.GetComponent<InputSystemUIInputModule>().enabled = false;
                    }
                }
                else
                {
                    if (c1Selected && chooseForCPU)
                    {
                        if (player1.currentSelectedGameObject.name == name)
                        {
                            currentCharacter2 = c;
                            c2Selected = true;
                            player1.GetComponent<InputSystemUIInputModule>().enabled = false;
                        }
                    }
                }
                if (player1.currentSelectedGameObject.name == name)
                {
                    currentCharacter1 = c;
                    c1Selected = true;
                    if (twoPlayer)
                        player1.GetComponent<InputSystemUIInputModule>().enabled = false;
                    else
                        chooseForCPU = true;
                }
            }
        }
    }

    public void CancelSelection(int playerPort)
    {
        if (c1Selected || c2Selected)
        {
            switch (playerPort)
            {
                case 1 :
                    DestroyImmediate(currentCharacter1);
                    c1Selected = false;
                    player1.GetComponent<InputSystemUIInputModule>().enabled = true;
                    break;
                case 2 :
                    DestroyImmediate(currentCharacter2);
                    c2Selected = false;
                    player2.GetComponent<InputSystemUIInputModule>().enabled = true;
                    break;
            }

            if (chooseForCPU)
                chooseForCPU = false;
        }
        else
        {
            switch (MainS.instance.state)
            {
                case GameState.Css :
                    MainS.instance.um.menu.GoTo(MenuSelection.Versus);
                    break;
                case GameState.NetworkCss :
                    MainS.instance.um.menu.GoTo(MenuSelection.Online);
                    break;
                case GameState.TrainingCss :
                    MainS.instance.um.menu.GoTo(MenuSelection.Training);
                    break;
            }
            MainS.instance.state = GameState.Menu;
            MainS.instance.um.StartMenuUpdating();
            initiated = false;
            c1Selected = false;
            c2Selected = false;
            player1.enabled = false;
            if (player2.enabled)
                player2.enabled = false;
            twoPlayer = false;
            chooseForCPU = false;
        }
    }
}

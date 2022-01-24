using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Css : MonoBehaviour
{
    public MultiplayerEventSystem player1;

    public MultiplayerEventSystem player2;

    private bool initiated = false;

    private bool twoPlayer = false;

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

    public AudioClip cssBGM;

    private bool chooseForCPU;

    [HideInInspector] public string lastSelection1;

    [HideInInspector] public string lastSelection2;

    private bool stageSelected;

    public GameObject stageUI;

    public List<GameObject> stagePrefabs;

    private GameObject currentStage;

    public GameObject characterIcons;

    public void ActivateCSS()
    {
        initiated = true;
        player1.enabled = true;
        MainS.instance.player1.MenuMovement1.Enable();
        //player1.SetSelectedGameObject(characterIcons.GetComponentInChildren<Button>().gameObject);
        player1.SetSelectedGameObject(player1.firstSelectedGameObject);
        if (MainS.instance.player2.MenuMovement2.enabled)
        {
            player2.enabled = true;
            twoPlayer = true;
            //player2.SetSelectedGameObject(characterIcons.GetComponentInChildren<Button>().FindSelectableOnLeft().gameObject);
            player2.SetSelectedGameObject(player2.firstSelectedGameObject);
            MainS.instance.player2.MenuMovement2.Enable();
        }
        else
        {
            twoPlayer = false;
        }

        c1Selected = false;
        c2Selected = false;
        stageSelected = false;
        BGMusic.bgmInstance.audio.clip = cssBGM;
        BGMusic.bgmInstance.audio.Play();
    }
    
    public void DisplayIcon(int port, MultiplayerEventSystem player, Image cp, TMP_Text name)
    {
        string nameToUse = port == 1 ? player1.currentSelectedGameObject.name : player2.currentSelectedGameObject.name;
        if (!player.currentSelectedGameObject.name.Contains("Mystery"))
        {
            cp.sprite = FindCharacterPortrait(nameToUse);
            name.text = nameToUse;
        }

        else
        {
            cp.sprite = FindCharacterPortrait("RandomCSS");
            name.text = "???";
        }
    }

    public void DisplayPreviewArea1()
    {
        foreach (var c in MainS.instance.characterPrefabs)
        {
            if (c.name == player1.currentSelectedGameObject.name)
            {
                if (prefabPreview1 != null)
                {
                    if (prefabPreview1.name != c.name + "(Clone)")
                    {
                        prefabPreview1.SetActive(false);
                        prefabPreview1 = MainS.instance.GetPooledFighter(c.name + "(Clone)");
                        prefabPreview1.SetActive(true);
                        prefabPreview1.transform.parent = previewArea1.transform;
                        prefabPreview1.transform.localPosition = new Vector3(0, 0, 0);
                        prefabPreview1.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                else
                {
                    prefabPreview1 = MainS.instance.GetPooledFighter(c.name + "(Clone)");
                    prefabPreview1.SetActive(true);
                    prefabPreview1.transform.parent = previewArea1.transform;
                    prefabPreview1.transform.localPosition = new Vector3(0, 0, 0);
                    prefabPreview1.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }

    public void DisplayPreviewArea2()
    {
        foreach (var c in MainS.instance.characterPrefabs)
        {
            if (c.name == player2.currentSelectedGameObject.name)
            {
                if (prefabPreview2 != null)
                {
                    if (prefabPreview2.name != c.name + "(Clone)")
                    {
                        prefabPreview2.SetActive(false);
                        prefabPreview2 = MainS.instance.GetPooledFighter2(c.name + "(Clone)");
                        prefabPreview2.SetActive(true);
                        prefabPreview2.transform.parent = previewArea2.transform;
                        prefabPreview2.transform.localPosition = new Vector3(0, 0, 0);
                        prefabPreview2.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                else
                {
                    prefabPreview2 = MainS.instance.GetPooledFighter2(c.name + "(Clone)");
                    prefabPreview2.SetActive(true);
                    prefabPreview2.transform.parent = previewArea2.transform;
                    prefabPreview2.transform.localPosition = new Vector3(0, 0, 0);
                    prefabPreview2.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }

    public void Move(int port, InputAction.CallbackContext context)
    {
        if (port == 1 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }

        if (port == 2 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }
        Vector2 direction = context.ReadValue<Vector2>();
        if (port == 1)
        {
            if (direction.x > 0)
            {
                player1.SetSelectedGameObject(player1.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnRight().gameObject); 
            }
            if (direction.x < 0)
            {
                player1.SetSelectedGameObject(player1.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnLeft().gameObject); 
            }
            if (direction.y > 0)
            {
                player1.SetSelectedGameObject(player1.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnUp().gameObject); 
            }
            if (direction.y < 0)
            {
                player1.SetSelectedGameObject(player1.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnDown().gameObject); 
            }
        }
        else
        {
            if (port == 2)
            {
                if (direction.x > 0)
                {
                    player2.SetSelectedGameObject(player2.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnRight().gameObject); 
                }
                if (direction.x < 0)
                {
                    player2.SetSelectedGameObject(player2.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnLeft().gameObject); 
                }
                if (direction.y > 0)
                {
                    player2.SetSelectedGameObject(player2.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnUp().gameObject); 
                }
                if (direction.y < 0)
                {
                    player2.SetSelectedGameObject(player2.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnDown().gameObject); 
                }
            }
        }
    }
    

    public void UpdateCss()
    {
        if (!initiated)
            ActivateCSS();
        if (player1.currentSelectedGameObject != null)
        {
            DisplayIcon(1, player1, cp1, name1);

            if (lastSelection1 == null)
                lastSelection1 = player1.currentSelectedGameObject.name;

            if (player1.currentSelectedGameObject.name != lastSelection1)
            {
                lastSelection1 = player1.currentSelectedGameObject.name;
                SFXManager.sfxInstance.PlayMoveSound();
                DisplayPreviewArea1();
            }
        }

        if (player2.currentSelectedGameObject != null)
        {
            DisplayIcon(2, player2, cp2, name2);
            
            if (lastSelection2 == null)
                lastSelection2 = player2.currentSelectedGameObject.name;

            if (player2.currentSelectedGameObject.name != lastSelection2)
            {
                lastSelection2 = player2.currentSelectedGameObject.name;
                SFXManager.sfxInstance.PlayMoveSound();
                DisplayPreviewArea2();
            }
        }

        if (c1Selected && c2Selected)
        {
            UpdateStageUI();
            if (stageSelected)
            {
                MainS.instance.um.vsScreen.VsScreenAppear(twoPlayer, currentCharacter1.name,
                    currentCharacter2.name);
                initiated = false;
                c1Selected = false;
                c2Selected = false;
                stageSelected = false;
                chooseForCPU = false;
                MainS.instance.player1.MenuMovement1.Disable();
                MainS.instance.player1.MenuMovement2.Disable();
                MainS.instance.player2.MenuMovement2.Disable();
            }
        }
    }

    public void LoadPrefabsInScene()
    {
        currentCharacter1.SetActive(false);
        currentCharacter2.SetActive(false);
        MainS.instance.fm.StarGame(currentCharacter1.name, currentCharacter2.name, currentStage, twoPlayer);
        stageUI.SetActive(false);
        twoPlayer = false;
        gameObject.SetActive(false);
    }

    public void UpdateStageUI()
    {
        if (!stageUI.activeInHierarchy)
        {
            stageUI.SetActive(true);
            player1.SetSelectedGameObject(stageUI.GetComponentInChildren<Button>().gameObject);
            MainS.instance.player1.MenuMovement1.Enable();
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

    public void SelectStage()
    {
        string name = player1.currentSelectedGameObject.name;
        foreach (var s in stagePrefabs)
        {
            if (s.name == name)
            {
                currentStage = s;
                stageSelected = true;
            }
        }
    }

    public void Select(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }
        if (c1Selected && c2Selected)
        {
            SelectStage();
        }
        else
        {
            SelectCharacter(playerPort, context);
        }
    }

    public void SelectCharacter(int port, InputAction.CallbackContext context)
    {
        string nameToUse = "";
        switch (port)
        {
            case 1:
                nameToUse = player1.currentSelectedGameObject.name;
                break;
            case 2:
                nameToUse = player2.currentSelectedGameObject.name;
                break;
        }

        foreach (var c in MainS.instance.characterPrefabs)
        {
            if (c.name == nameToUse)
            {
                if (twoPlayer && port == 2)
                {
                    //currentCharacter2 = MainS.instance.GetPooledFighter2(c.name + "(Clone)");
                    currentCharacter2 = prefabPreview2;
                    currentCharacter2.SetActive(false);
                    c2Selected = true;
                    MainS.instance.player2.MenuMovement2.Disable();
                }
                else if (c1Selected && chooseForCPU)
                {
                    //currentCharacter2 = MainS.instance.GetPooledFighter2(c.name + "(Clone)");
                    currentCharacter2 = prefabPreview2;
                    currentCharacter2.SetActive(false);
                    c2Selected = true;
                    MainS.instance.player2.MenuMovement1.Disable();
                }
                else
                {
                    //currentCharacter1 = MainS.instance.GetPooledFighter(c.name + "(Clone)");
                    currentCharacter1 = prefabPreview1;
                    currentCharacter1.SetActive(false);
                    c1Selected = true;
                    if (twoPlayer)
                        MainS.instance.player1.MenuMovement1.Disable();
                    else
                    {
                        chooseForCPU = true;
                        MainS.instance.player1.MenuMovement1.Disable();
                        MainS.instance.player2.MenuMovement1.Enable();
                    }
                }
            }

            SFXManager.sfxInstance.PlayOkSound();
        }
    }

    public void CancelSelection(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }

        if (stageUI.activeInHierarchy)
        {
        }
        else if (c1Selected || c2Selected)
        {
            switch (playerPort)
            {
                case 1:
                    currentCharacter1.SetActive(false);
                    c1Selected = false;
                    MainS.instance.player1.MenuMovement1.Disable();
                    break;
                case 2:
                    currentCharacter2.SetActive(false);
                    c2Selected = false;
                    MainS.instance.player2.MenuMovement2.Disable();
                    break;
            }

            if (chooseForCPU)
            {
                chooseForCPU = false;
                MainS.instance.player2.MenuMovement1.Disable();
                MainS.instance.player1.MenuMovement1.Enable();
            }
        }
        else
        {
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
            switch (MainS.instance.state)
            {
                case GameState.Css:
                    MainS.instance.um.mainMenu.GoTo(MenuSelection.Versus, true);
                    break;
                case GameState.NetworkCss:
                    MainS.instance.um.mainMenu.GoTo(MenuSelection.Online, true);
                    break;
                case GameState.TrainingCss:
                    MainS.instance.um.mainMenu.GoTo(MenuSelection.Training, true);
                    break;
            }
        }

        SFXManager.sfxInstance.PlayCancelSound();
    }

    public void RandomizeSelection(int playerPort, InputAction.CallbackContext context)
    {
        if (playerPort == 1 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 1))
                return;
        }

        if (playerPort == 2 && !chooseForCPU)
        {
            if (!MainS.instance.portController.CheckID(context, 2))
                return;
        }

        if (c1Selected && c2Selected)
        {
            int numberOfStage = stagePrefabs.Count;
            string newGOName = stagePrefabs[Random.Range(0, numberOfStage)].name;
            GameObject go = null;
            Button[] buttons = stageUI.GetComponentsInChildren<Button>();
            foreach (var c in buttons)
            {
                if (c.gameObject.name.Contains(newGOName))
                    go = c.gameObject;
            }

            player1.SetSelectedGameObject(go);
        }
        else
        {
            int numberOfCharacter = MainS.instance.characterPrefabs.Count;
            string newGOName = MainS.instance.characterPrefabs[Random.Range(0, numberOfCharacter)].name;
            GameObject go = null;
            Button[] buttons = characterIcons.GetComponentsInChildren<Button>();
            foreach (var c in buttons)
            {
                if (c.gameObject.name.Contains(newGOName))
                    go = c.gameObject;
            }

            if (playerPort == 1)
            {
                player1.SetSelectedGameObject(go);
            }

            if (playerPort == 2)
            {
                player2.SetSelectedGameObject(go);
            }
        }
    }
}
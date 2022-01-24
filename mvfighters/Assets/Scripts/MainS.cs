using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MainS : MonoBehaviour
{
    public FighterM fm;

    public UIManager um;

    public InputManager player1;

    public InputManager player2;

    public static MainS instance = null;

    public GameState state;

    [HideInInspector] public Settings settings;

    [HideInInspector] public PortControl portController;

    [SerializeField] private List<GameObject> pooledCharacters;

    [SerializeField] private List<GameObject> pooledCharacters2;

    public List<GameObject> characterPrefabs;

    [SerializeField] private InputActionAsset inputActions1;

    [SerializeField] private InputActionAsset inputActions2;

    //private PlayerInput controls;

    private bool paused = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        player1 = new InputManager();
        player2 = new InputManager();
        //Debug.Log(inputActions1.FindActionMap("UI").FindAction("Submit"));
        //player1.GamePadScheme.PickDevicesFrom(Gamepad.all);
        //player2.GamePadScheme.PickDevicesFrom(Gamepad.all);
        portController = GetComponent<PortControl>();
        portController.SetupGamepadID(Gamepad.all);
        PrepareInputManager(player1, 1);
        state = GameState.Menu;
        //controls = fm.player1.GetComponent<PlayerInput>();
        um.StartMenuUpdating();
        settings = GetComponent<Settings>();
        settings.resolutionSettings.DefaultSettings();
        settings.resolutionSettings.ApplySetting();
        CreateFighterPool();
        CreateFighterPool2();
    }

    public void CreateFighterPool()
    {
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[i]);
            obj.SetActive(false);
            pooledCharacters.Add(obj);
        }
    }

    public void CreateFighterPool2()
    {
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[i]);
            obj.SetActive(false);
            pooledCharacters2.Add(obj);
        }
    }

    public GameObject GetPooledFighter(string name)
    {
        for (int i = 0; i < pooledCharacters.Count; i++)
        {
            if (!pooledCharacters[i].activeInHierarchy && pooledCharacters[i].name == name)
            {
                return pooledCharacters[i];
            }
        }

        return null;
    }

    public GameObject GetPooledFighter2(string name)
    {
        for (int i = 0; i < pooledCharacters2.Count; i++)
        {
            if (!pooledCharacters2[i].activeInHierarchy && pooledCharacters2[i].name == name)
            {
                return pooledCharacters2[i];
            }
        }

        return null;
    }

    public void PrepareInputManager(InputManager player, int playerPort)
    {
        //player1.Combat1.Enable();

        player1.Combat1.A.performed += context => fm.UseMove(1, MoveType.A, context);
        player1.Combat1.B.performed += context => fm.UseMove(1, MoveType.B, context);
        player1.Combat1.C.performed += context => fm.UseMove(1, MoveType.C, context);
        player1.Combat1.MotionF.performed += context => fm.UseMove(1, MoveType.MotionF, context);
        player1.Combat1.MotionB.performed += context => fm.UseMove(1, MoveType.MotionB, context);
        player1.Combat1.Grab.performed += context => fm.UseMove(1, MoveType.Grab, context);
        player1.Combat1.Dash.performed += context => fm.PerformDash(1, context);
        player1.Combat1.Move.performed += context => fm.PlayerMove(1, context);
        player1.Combat1.Pause.performed += context => fm.PauseButton(1, context);

        //player2.Combat2.Enable();
        player2.Combat2.A.performed += context => fm.UseMove(2, MoveType.A, context);
        player2.Combat2.B.performed += context => fm.UseMove(2, MoveType.B, context);
        player2.Combat2.C.performed += context => fm.UseMove(2, MoveType.C, context);
        player2.Combat2.MotionF.performed += context => fm.UseMove(2, MoveType.MotionF, context);
        player2.Combat2.MotionB.performed += context => fm.UseMove(2, MoveType.MotionB, context);
        player2.Combat2.Grab.performed += context => fm.UseMove(2, MoveType.Grab, context);
        player2.Combat2.Dash.performed += context => fm.PerformDash(2, context);
        player2.Combat2.Move.performed += context => fm.PlayerMove(2, context);
        player2.Combat2.Pause.performed += context => fm.PauseButton(2, context);

        //player1.MenuMovement1.Enable();
        player1.MenuMovement1.Cancel.performed += context => um.css.CancelSelection(1, context);
        player1.MenuMovement1.Select.performed += context => um.css.Select(1, context);
        player1.MenuMovement1.ExtraButton1.performed += context => um.css.RandomizeSelection(1, context);
        player1.MenuMovement1.Move.performed += context => um.css.Move(1, context);
        //player2.MenuMovement2.Enable();
        player2.MenuMovement1.Cancel.performed += context => um.css.CancelSelection(2, context);
        player2.MenuMovement1.Select.performed += context => um.css.Select(2, context);
        player2.MenuMovement1.ExtraButton1.performed += context => um.css.RandomizeSelection(2, context);
        player2.MenuMovement1.Move.performed += context => um.css.Move(2, context);
        //player2.MenuMovement2.Enable();
        player2.MenuMovement2.Cancel.performed += context => um.css.CancelSelection(2, context);
        player2.MenuMovement2.Select.performed += context => um.css.Select(2, context);
        player2.MenuMovement2.ExtraButton1.performed += context => um.css.RandomizeSelection(2, context);
        player2.MenuMovement2.Move.performed += context => um.css.Move(2, context);

        player1.UI.Cancel.performed += context => um.CancelSelection(1, context);
        player1.UI.Submit.performed += context => um.Submit(1, context);
        // inputActions1.actionMaps[0].Enable();
        // inputActions1.actionMaps[0].actions[2].performed += context => um.Submit(1, context);
        player1.UI.Pause.performed += context => fm.PauseButton(1, context);
        //inputActionMaps = player1.UI;
    }

    public void GameStart(bool twoPlayer, GameState previousState)
    {
        DisableMenuControls();
        player1.Combat1.Enable();
        if (twoPlayer)
            player2.Combat2.Enable();
        switch (previousState)
        {
            case GameState.Css:
                state = GameState.Combat;
                break;
            case GameState.NetworkCss:
                state = GameState.NetworkCombat;
                break;
            case GameState.TrainingCss:
                state = GameState.TrainingCombat;
                break;
        }
    }

    public void DisableMenuControls()
    {
        if (player1.UI.enabled)
            player1.UI.Disable();
        if (player1.MenuMovement1.enabled)
            player1.MenuMovement1.Disable();
        if (player1.MenuMovement2.enabled)
            player1.MenuMovement2.Disable();
        if (player2.MenuMovement2.enabled)
            player2.MenuMovement2.Disable();
        if (player2.MenuMovement1.enabled)
            player2.MenuMovement1.Disable();
    }

    public void EnableMenuControls()
    {
        if (!player1.UI.enabled)
            player1.UI.Enable();
        if (!player1.MenuMovement1.enabled)
            player1.MenuMovement1.Enable();
        if (!player1.MenuMovement2.enabled)
            player1.MenuMovement2.Enable();
        if (!player2.MenuMovement2.enabled)
            player2.MenuMovement2.Enable();
        if (!player2.MenuMovement1.enabled)
            player2.MenuMovement1.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Menu)
        {
            if (!um.canvas.isActiveAndEnabled)
                um.canvas.enabled = true;
            um.UpdateMenu();
        }

        if (state is GameState.Css or GameState.NetworkCss or GameState.TrainingCss)
        {
            um.UpdateCSS();
        }

        if (state is GameState.Combat or GameState.TrainingCombat or GameState.NetworkCombat)
        {
            fm.UpdateObjects();

            um.UpdateInGameUI();

            if (paused)
            {
                um.ActivatePauseMenu();
            }
            else
            {
                um.DeactivatePauseMenu();
            }
        }

        if (state is GameState.ResultScreen)
        {
            um.UpdateResultScreen();
        }

        // if (paused)
        //um.temp.GetComponentInChildren<TMP_Text>().text = player1.Combat1.Grab.name + " = " + player1.Combat1.Grab;
    }

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in inputActions1.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        foreach (InputActionMap map in inputActions2.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        PlayerPrefs.DeleteKey("rebinds");
    }

    public void SetPause(bool boolean)
    {
        paused = boolean;
    }

    public bool GetPause()
    {
        return paused;
    }

    public void StartRebindProcess(string action)
    {
        bool reEnableMenu = false;
        if (player1.UI.enabled)
        {
            DisableMenuControls();
            reEnableMenu = true;
        }

        bool reEnableCombat = false;
        if (player1.Combat1.enabled)
        {
            fm.DisableControls();
            reEnableCombat = true;
        }

        player1.FindAction(action).PerformInteractiveRebinding().WithTimeout(5f).WithControlsExcluding("Mouse")
            .WithControlsExcluding("Pointer").Start();


        if (reEnableMenu)
        {
            EnableMenuControls();
        }

        if (reEnableCombat)
        {
            fm.EnableControls();
        }
    }
}
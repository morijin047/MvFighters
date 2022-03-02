using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MainScript : MonoBehaviour
{
    public FighterManager fm;

    public UIManager um;

    public InputManager player1;

    public InputManager player2;

    public static MainScript instance = null;

    public GameState state;

    [HideInInspector] public Settings settings;

    [HideInInspector] public PortControl portController;

    [SerializeField] private List<GameObject> pooledCharacters;

    [SerializeField] private List<GameObject> pooledCharacters2;

    public List<GameObject> characterPrefabs;

     private InputActionAsset inputActions1;

    [SerializeField] private InputActionAsset inputActions2;

    //private PlayerInput controls;

    private bool paused = false;

    private void Awake()
    {
        //Singleton instance
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        //Action map generation
        player1 = new InputManager();
        player2 = new InputManager();
        
        //The port controller is a script that will check the device ID for every action performed and avoid duplicate input
        portController = GetComponent<PortControl>();
        portController.SetupGamepadID(Gamepad.all);
        
        //Action event binding
        PrepareInputManager(player1, 1);
        
        //We start in the Main Menu
        state = GameState.Menu;
        um.StartMenuUpdating();
        
        //Settings script that can be changed during runtime. We need to apply the default setting in order to avoid nullexception
        settings = GetComponent<Settings>();
        settings.resolutionSettings.DefaultSettings();
        settings.resolutionSettings.ApplySetting();
        
        //Pool of Figthers
        CreateFighterPool();
        CreateFighterPool2();
    }

    //With a pool system, the fighter wont totally destroy themselves, the game run way smoother and conflicts doesnt happens anymore 
    public void CreateFighterPool()
    {
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[i]);
            obj.SetActive(false);
            pooledCharacters.Add(obj);
        }
    }

    // Get a fighter from the 1st generated pool 
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
    
    //A 2nd pool was added to avoid conflicts created by both script trying to take the same fighter from the pool
    public void CreateFighterPool2()
    {
        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[i]);
            obj.SetActive(false);
            pooledCharacters2.Add(obj);
        }
    }
    // Get a fighter from the 2nd generated pool 
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

    /*
     *To prepare all the control maps and their event function performed. The parameter where an attempt to format this code but it seems unoptimizable for me at least.
     * To add a new action or action map, refer to the input manager in the asset folder.
     */
    public void PrepareInputManager(InputManager player, int playerPort)
    {
        //player1 Combat Action Maps 1
        player1.Combat1.A.performed += context => fm.UseMove(1, MoveType.A, context);
        player1.Combat1.B.performed += context => fm.UseMove(1, MoveType.B, context);
        player1.Combat1.C.performed += context => fm.UseMove(1, MoveType.C, context);
        player1.Combat1.MotionF.performed += context => fm.UseMove(1, MoveType.MotionF, context);
        player1.Combat1.MotionB.performed += context => fm.UseMove(1, MoveType.MotionB, context);
        player1.Combat1.Grab.performed += context => fm.UseMove(1, MoveType.Grab, context);
        player1.Combat1.Dash.performed += context => fm.PerformDash(1, context);
        player1.Combat1.Move.performed += context => fm.PlayerMove(1, context);
        player1.Combat1.Pause.performed += context => fm.PauseButton(1, context);

        //player2 Combat Action Maps 2
        player2.Combat2.A.performed += context => fm.UseMove(2, MoveType.A, context);
        player2.Combat2.B.performed += context => fm.UseMove(2, MoveType.B, context);
        player2.Combat2.C.performed += context => fm.UseMove(2, MoveType.C, context);
        player2.Combat2.MotionF.performed += context => fm.UseMove(2, MoveType.MotionF, context);
        player2.Combat2.MotionB.performed += context => fm.UseMove(2, MoveType.MotionB, context);
        player2.Combat2.Grab.performed += context => fm.UseMove(2, MoveType.Grab, context);
        player2.Combat2.Dash.performed += context => fm.PerformDash(2, context);
        player2.Combat2.Move.performed += context => fm.PlayerMove(2, context);
        player2.Combat2.Pause.performed += context => fm.PauseButton(2, context);

        //player1 Character Select Action Maps 1
        player1.MenuMovement1.Cancel.performed += context => um.css.CancelSelection(1, context);
        player1.MenuMovement1.Select.performed += context => um.css.Select(1, context);
        player1.MenuMovement1.ExtraButton1.performed += context => um.css.RandomizeSelection(1, context);
        player1.MenuMovement1.Move.performed += context => um.css.Move(1, context);
        
        //player2 Character Select Action Maps 1 (For single Player Mode, this is how you will move the 2nd cursor as player 1)
        player2.MenuMovement1.Cancel.performed += context => um.css.CancelSelection(2, context);
        player2.MenuMovement1.Select.performed += context => um.css.Select(2, context);
        player2.MenuMovement1.ExtraButton1.performed += context => um.css.RandomizeSelection(2, context);
        player2.MenuMovement1.Move.performed += context => um.css.Move(2, context);
        
        //player2 Character Select Action Maps 2
        player2.MenuMovement2.Cancel.performed += context => um.css.CancelSelection(2, context);
        player2.MenuMovement2.Select.performed += context => um.css.Select(2, context);
        player2.MenuMovement2.ExtraButton1.performed += context => um.css.RandomizeSelection(2, context);
        player2.MenuMovement2.Move.performed += context => um.css.Move(2, context);

        //Player1 UI Action Maps
        player1.UI.Cancel.performed += context => um.CancelSelection(1, context);
        player1.UI.Submit.performed += context => um.Submit(1, context);
        player1.UI.Pause.performed += context => fm.PauseButton(1, context);
    }

    //Function called when the VS screen closes and the Game start. Note that the round still hasn't started at this point and the round call is gonna be played with the fighters intro.
    public void GameStart(bool twoPlayer, GameState previousState)
    {
        DisableMenuControls();
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

    //Disable ALL UI controls including CSS controls
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

    //Enable the last disabled menu controls. Incomplete for the css part. Never used in that script anyway
    public void EnableMenuControls()
    {
        if (state == GameState.Menu)
        {
            if (!player1.UI.enabled)
                player1.UI.Enable(); 
        }

        if (state is GameState.Css or GameState.NetworkCss or GameState.TrainingCss)
        {
            if (!player1.MenuMovement1.enabled)
                player1.MenuMovement1.Enable();
            if (!player1.MenuMovement2.enabled)
                player1.MenuMovement2.Enable();
            if (!player2.MenuMovement2.enabled)
                player2.MenuMovement2.Enable();
            if (!player2.MenuMovement1.enabled)
                player2.MenuMovement1.Enable();
        }
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
        
    }

    //Reset all the interactive binding that occured during the play session
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

    //Activate or Deactivate the pause menu
    public void SetPause(bool boolean)
    {
        paused = boolean;
    }

    //Check if the game is paused
    public bool GetPause()
    {
        return paused;
    }

    //Process to rebind the action map instance that is used in this script instead of the actual input action assets in the project
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
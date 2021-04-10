using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameMode
{
    LocalMultiplayer
}

public class GameManager : MonoBehaviour
{
    //Game Mode
    public GameMode currentGameMode;

    //Local Multiplayer
    public GameObject playerPrefab;
    public int numberOfPlayers;

    //Spawned Players
    private List<PlayerController> activePlayerControllers;
    private bool isPaused;
    private PlayerController focusedPlayerController;

    //General
    [SerializeField]
    private Transform[] spawns;

    private int _spawnIndex = 0;
    private int _adjustControllerCount = 0;
    private List<PlayerController> _activePlayerControllers;
    private PlayerInput _checkInput;

	private PlayerStates _currentPlayerState;
	private PlayerStates _perviousPlayerState;

    private bool _canStart = false;

    public static GameManager Instance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _checkInput = this.transform.GetComponent<PlayerInput>();
        _activePlayerControllers = new List<PlayerController>();
        isPaused = false;

        SetupBasedOnGameState();
        SetupUI();
    }

	private void Update()
    {
        _adjustControllerCount = 0;
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
			if (Input.GetKey(KeyCode.Space))
			{
                Debug.Log($"joystick {i}: {Input.GetJoystickNames()[i]}");
            }

            if (Input.GetJoystickNames()[i] == string.Empty)
            {
                _adjustControllerCount++;
            }
        }

        if (Input.GetJoystickNames().Length - _adjustControllerCount > _activePlayerControllers.Count)
		{
            Debug.Log("spawn player");
            SpawnPlayer();
		}
	}

	void SetupBasedOnGameState()
    {
        switch(currentGameMode)
        {
            case GameMode.LocalMultiplayer:
                SetupLocalMultiplayer();
                break;
        }
    }

    void SetupLocalMultiplayer()
    { 
        //SpawnPlayers();

        //SetupActivePlayers();
    }

    //void SpawnPlayers()
    //{
    //    activePlayerControllers = new List<PlayerController>();

    //    for(int i = 0; i < numberOfPlayers; i++)
    //    {
    //        Vector3 spawnPosition = CalculateSpawnPosition(i);
    //        Quaternion spawnRotation = CalculateSpawnRotation(i);

    //        GameObject spawnedPlayer = Instantiate(playerPrefab, spawnPosition, spawnRotation) as GameObject;
    //        AddPlayerToActivePlayerList(spawnedPlayer.GetComponent<PlayerController>());
    //    }
    //}

    void SpawnPlayer()
	{
        Vector3 spawnPosition = CalculateSpawnPosition(_spawnIndex);
        Quaternion spawnRotation = CalculateSpawnRotation(_spawnIndex);

        GameObject spawnedPlayer = Instantiate(playerPrefab, spawnPosition, spawnRotation) as GameObject;

        if (spawnedPlayer.GetComponent<PlayerController>().TrySetupPlayer(_spawnIndex))
        {
            _spawnIndex++;
            AddPlayerToActivePlayerList(spawnedPlayer.GetComponent<PlayerController>());
            TryActivateStart();
        }
    }

	public void TryActivateStart()
	{
		if (_activePlayerControllers.Count > 1)
		{
            UIManager uiManager = this.GetComponentInChildren<UIManager>();
			for (int i = 0; i < _activePlayerControllers.Count; i++)
			{
				if (_activePlayerControllers[i].IsDisconnected)
				{
                    _canStart = false;
                    uiManager.StartGame.SetActive(false);
                    return;
				}
            }

            uiManager.StartGame.SetActive(true);
            _canStart = true;
        }
	}

	void AddPlayerToActivePlayerList(PlayerController newPlayer)
    {
        _activePlayerControllers.Add(newPlayer);
    }

    void SetupActivePlayers()
    {
        for(int i = 0; i < activePlayerControllers.Count; i++)
        {
            activePlayerControllers[i].TrySetupPlayer(i);
        }
    }

    void SetupUI()
    {
        UIManager.Instance.SetupManager();
    }

    public void TogglePauseState(PlayerController newFocusedPlayerController)
    {
        focusedPlayerController = newFocusedPlayerController;

        isPaused = !isPaused;

        ToggleTimeScale();

        UpdateActivePlayerInputs();

        SwitchFocusedPlayerControlScheme(PlayerStates.Paused);

        UpdateUIMenu();
    }

    public void Submit(PlayerController newFocusedPlayerController)
    {
		if (_canStart)
		{
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "SelectionScreen")
            {
                Debug.Log("Load game scene");
                //SceneManager.LoadScene();
            }
        }
    }

    public void ToggleSpectatingState(PlayerController newFocusedPlayerController)
	{
		focusedPlayerController = newFocusedPlayerController;

		UpdateActivePlayerInputs();

		SwitchFocusedPlayerControlScheme(PlayerStates.Spectating);

		UpdateUIMenu();
	}

	public void ExitState(PlayerController newFocusedPlayerController)
	{
		Debug.Log("Exit state");
	}

	void UpdateActivePlayerInputs()
    {
        for(int i = 0; i < _activePlayerControllers.Count; i++)
        {
            if(_activePlayerControllers[i] != focusedPlayerController)
            {
                _activePlayerControllers[i].SetInputActiveState(isPaused);
            }
        }
    }

    void SwitchFocusedPlayerControlScheme(PlayerStates playerState)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        switch(playerState)
        {
            case PlayerStates.Paused:
				if(isPaused)
				{
					focusedPlayerController.EnablePauseMenuControls();
				}
				else
				{
					if (activeScene.name != "SelectionScreen")
					{
					    focusedPlayerController.EnableGameplayControls();
					}
				}
                break;

            case PlayerStates.Shooting:
                focusedPlayerController.EnableGameplayControls();
                break;

			case PlayerStates.Spectating:
				focusedPlayerController.EnableSpectatingControls();
				break;

			case PlayerStates.Placing:
				focusedPlayerController.EnablePlacementControls();
				break;
		}
    }

    void UpdateUIMenu()
    {
        UIManager.Instance.UpdateUIMenuState(isPaused);
    }

    //Get Data ----

    public List<PlayerController> GetActivePlayerControllers()
    {
        return _activePlayerControllers;
    }

    public PlayerController GetFocusedPlayerController()
    {
        return focusedPlayerController;
    }

    public int NumberOfConnectedDevices()
    {
        return InputSystem.devices.Count;
    }
    

    //Pause Utilities ----

    void ToggleTimeScale()
    {
        float newTimeScale = 0f;

        switch(isPaused)
        {
            case true:
                newTimeScale = 0f;
                break;

            case false:
                newTimeScale = 1f;
                break;
        }

        Time.timeScale = newTimeScale;
    }


    //Spawn Utilities
    private Vector3 CalculateSpawnPosition(int i)
    {
        return spawns[i].transform.position;
    }

    Quaternion CalculateSpawnRotation(int i)
    {
        return spawns[i].transform.rotation;
    }

    public void Restart()
    {
        for (int i = 0; i < _activePlayerControllers.Count; i++)
        {
            PlayerController controller = _activePlayerControllers[i];
            _activePlayerControllers.Remove(controller);
            Destroy(controller.gameObject);
        }

        _checkInput = this.transform.GetComponent<PlayerInput>();
        _activePlayerControllers = new List<PlayerController>();
        isPaused = false;
        _spawnIndex = 0;

        SetupBasedOnGameState();
        SetupUI();
    }

}

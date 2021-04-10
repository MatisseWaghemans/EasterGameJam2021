using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameMode
{
    LocalMultiplayer
}

public class GameManager : Singleton<GameManager>
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

        if (_checkInput.currentControlScheme != "Gamepad")
        {
            GameObject spawnedPlayer = Instantiate(playerPrefab, spawnPosition, spawnRotation) as GameObject;

            if (spawnedPlayer.GetComponent<PlayerController>().TrySetupPlayer(_spawnIndex))
            {
                _spawnIndex++;
                AddPlayerToActivePlayerList(spawnedPlayer.GetComponent<PlayerController>());
            }
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

        SwitchFocusedPlayerControlScheme();

        UpdateUIMenu();
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

    void SwitchFocusedPlayerControlScheme()
    {
        switch(isPaused)
        {
            case true:
                focusedPlayerController.EnablePauseMenuControls();
                break;

            case false:
                focusedPlayerController.EnableGameplayControls();
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

}

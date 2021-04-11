using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField]
    private int _maxNumberOfPlayers = 4;

    //Spawned Players
    private bool isPaused;
    private PlayerController focusedPlayerController;

    //General
    [SerializeField]
    private Transform[] spawns;

    [SerializeField]
    private Material[] _materials;
	public Material[] Materials { get => _materials; }

	private int _playerId = 0;
    private List<PlayerController> _activePlayerControllers;
	public List<PlayerController> ActivePlayerControllers { get => _activePlayerControllers; }
	private List<Gamepad> _usedGamepads;

	private PlayerStates _currentPlayerState;
	private PlayerStates _perviousPlayerState;

    private bool _canStart = false;

    private List<GameObject> _texts = new List<GameObject>();

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
            Destroy(Instance);
            Instance = this;
        }
        foreach(var text in spawns)
        {
            _texts.Add(text.GetComponentInChildren<TMP_Text>().gameObject);
            text.GetComponentInChildren<TMP_Text>().gameObject.SetActive(false);
        }
    }
    void Start()
    {
        _activePlayerControllers = new List<PlayerController>();
        _usedGamepads = new List<Gamepad>();
        isPaused = false;

        SetupUI();
    }

    public void ConnectGamepads()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i] != null)
            {
                _texts[i].SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"found gamepad: {Gamepad.all[i].name}");
            }
            if (Gamepad.all[i].buttonEast.isPressed)
            {
                Debug.Log($"pressed gamepad: {Gamepad.all[i].name}");
                if (_usedGamepads.Contains(Gamepad.all[i]))
                    return;
                else
                {
                    _texts[i].SetActive(false);
                    SpawnPlayer(i);
                    _usedGamepads.Add(Gamepad.all[i]);
                }
            }
        }
    }
    public void DisconnectGamepads()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonWest.isPressed)
            {
                Debug.Log($"pressed gamepad: {Gamepad.all[i].name}");
                if (_usedGamepads.Contains(Gamepad.all[i]))
                {
                    DespawnPlayer(i);
                    _usedGamepads.Remove(Gamepad.all[i]);
                }
            }
        }
    }

    private void DespawnPlayer(int id)
    {
        for(int i = 0; i<_activePlayerControllers.Count;i++)
        {
            if(_activePlayerControllers[i].ControllerID==id)
            {
                Destroy(_activePlayerControllers[i].gameObject);
                _activePlayerControllers.Remove(_activePlayerControllers[i]);
                _playerId--;

                var text = spawns[_playerId].GetComponentInChildren<TMPro.TMP_Text>();
                var image = spawns[_playerId].GetComponentInChildren<SpriteRenderer>();
                text.text = "Press        To Join";
                image.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
        }
    }

    public bool CanConnect()
    {
        return _activePlayerControllers.Count < _maxNumberOfPlayers;
    }
    public bool CanDisconnect()
    {
        return _activePlayerControllers.Count > 0;
    }

    void SpawnPlayer(int controllerId)
	{
        PlayerInput input = PlayerInput.Instantiate(playerPrefab, _playerId, "Gamepad", -1, Gamepad.all[controllerId]);
        GameObject spawnedPlayer = input.gameObject;

        spawnedPlayer.transform.position = CalculateSpawnPosition(_playerId);
        spawnedPlayer.transform.rotation = CalculateSpawnRotation(_playerId);

        spawnedPlayer.GetComponent<PlayerController>().SetupPlayer(_playerId, _materials[_playerId]);

        _playerId++;
        AddPlayerToActivePlayerList(spawnedPlayer.GetComponent<PlayerController>());
        TryActivateStart();
    }

	public void TryActivateStart()
	{
        Scene scene = SceneManager.GetActiveScene();
		if (scene.name != "SelectionScreen")
            return;

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
                UIManager uiManager = this.GetComponentInChildren<UIManager>();
                uiManager.StartGame.SetActive(false);
                Debug.Log("Load game scene");
                SceneManager.LoadScene("Level01");
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
        _texts[i].GetComponent<TMP_Text>().text = "Press        To Leave";
        //var text = spawns[i].GetComponentInChildren<TMP_Text>();
        var image = _texts[i].GetComponentInChildren<SpriteRenderer>();
        //text.text = "Press        To Leave";
        image.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0)); 
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

        _activePlayerControllers = new List<PlayerController>();
        isPaused = false;
        _playerId = 0;

        SetupUI();
    }

}

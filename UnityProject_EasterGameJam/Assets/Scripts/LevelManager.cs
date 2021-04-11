using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//public interface ILevelStates
//{
//    enum LevelState
//    {
//        Overview,
//        Placement,
//        SplitScreen
//    };
//}

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	private Transform[] _startTransforms;

	[SerializeField]
	private Transform _spectateTransform;

	[SerializeField]
	private SplitscreenManager _splitscreenManager;

    [SerializeField] 
    private PlacementController _placementController;

	[SerializeField]
	private CameraOverviewBehaviour _cameraOverviewBehaviour;

	[SerializeField]
	private string _nextSceneName;

	private GameManager _gameManager;
	private List<PlayerController> _activePlayerControllers;
	public List<PlayerController> PlayerControllers { get => _activePlayerControllers; }

	public List<PlayerController> FinishedPlayers = new List<PlayerController>();


	//public ILevelStates.LevelState LevelState { get; set; }

	private void Start()
	{
		_activePlayerControllers = new List<PlayerController>();
		_gameManager = FindObjectOfType<GameManager>();

        //LevelState = ILevelStates.LevelState.Overview;

        _cameraOverviewBehaviour.StartOverview();
        _cameraOverviewBehaviour.Event.AddListener(StartLevel);

		//instantiate balls
		PlaceActivePlayers(_gameManager.ActivePlayerControllers);

		//disable controls
		DisableInput();
	}

    void Update()
    {
    //    switch (LevelState)
    //    {
    //        case ILevelStates.LevelState.Overview:
				////Debug.Log("Overview mode");
    //            break;
    //        case ILevelStates.LevelState.Placement:
				//Debug.Log("Placement Mode");
    //            break;
    //        case ILevelStates.LevelState.SplitScreen:
				//Debug.Log("Splitscreen mode");
    //            break;
    //    }
	}

	private void PlaceActivePlayers(List<PlayerController> activePlayerControllers)
	{
		for (int i = 0; i < activePlayerControllers.Count; i++)
		{
			GameObject player = Instantiate(_gameManager.playerPrefab, _startTransforms[i].position, _startTransforms[i].rotation);
			CameraBehaviour cameraBehaviour = player.GetComponentInChildren<CameraBehaviour>();
			cameraBehaviour.SpectateTransform = _spectateTransform;

			PlayerController playerController = player.GetComponent<PlayerController>();
			playerController.SetupPlayer(i, _gameManager.Materials[i]);
			_activePlayerControllers.Add(playerController);
		}
	}

	public void CheckPlayersFinished()
	{
		Debug.Log("Checked Finished players");

		for (int i = 0; i < _activePlayerControllers.Count; i++)
		{
			if (!FinishedPlayers.Contains(_activePlayerControllers[i]) && _activePlayerControllers[i].PlayerGolfBallBehaviour.CurrentPlayerState == PlayerStates.Finished)
			{
				FinishedPlayers.Add(_activePlayerControllers[i]);
				Debug.Log("Added Finished player");

			}
		}

		if (FinishedPlayers.Count >= _activePlayerControllers.Count)
		{
			OnAllPlayersFinished();
		}
	}

	public void OnAllPlayersFinished()
	{
		FinishedPlayers.Clear();
		Debug.Log("All players finished");
		SceneManager.LoadScene(_nextSceneName);
	}

	private void StartLevel()
	{
		_cameraOverviewBehaviour.Event.RemoveListener(StartLevel);

		//LevelState = ILevelStates.LevelState.SplitScreen;

		//show splitscreen
		_splitscreenManager.InitCameras(_activePlayerControllers, _startTransforms, true);

		//enable controls
		EnableInput();
		for (int i = 0; i < _activePlayerControllers.Count; i++)
		{
			_activePlayerControllers[i].playerInput.SwitchCurrentActionMap("Player Controls");
		}
	}

	private void EnableInput()
	{
		for (int i = 0; i < _activePlayerControllers.Count; i++)
		{
			_activePlayerControllers[i].playerInput.DeactivateInput();
		}
	}

	private void DisableInput()
	{
		for (int i = 0; i < _activePlayerControllers.Count; i++)
		{
			_activePlayerControllers[i].playerInput.ActivateInput();
		}
	}
}

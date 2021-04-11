using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	private Transform[] _startTransforms;

	[SerializeField]
	private SplitscreenManager _splitscreenManager;

	[SerializeField]
	private CameraOverviewBehaviour _cameraOverviewBehaviour;

	private GameManager _gameManager;
	private List<PlayerController> _activePlayerControllers;

	private void Start()
	{
		_activePlayerControllers = new List<PlayerController>();
		_gameManager = FindObjectOfType<GameManager>();

		_cameraOverviewBehaviour.StartOverview();
		_cameraOverviewBehaviour.Event.AddListener(StartLevel);

		//instantiate balls
		PlaceActivePlayers(_gameManager.ActivePlayerControllers);

		//disable controls
		DisableInput();
	}

	private void PlaceActivePlayers(List<PlayerController> activePlayerControllers)
	{
		for (int i = 0; i < activePlayerControllers.Count; i++)
		{
			GameObject player = Instantiate(_gameManager.playerPrefab, _startTransforms[i].position, _startTransforms[i].rotation);

			PlayerController playerController = player.GetComponent<PlayerController>();
			playerController.SetupPlayer(i, _gameManager.Materials[i]);
			_activePlayerControllers.Add(playerController);
		}
	}

	private void StartLevel()
	{
		_cameraOverviewBehaviour.Event.RemoveListener(StartLevel);

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

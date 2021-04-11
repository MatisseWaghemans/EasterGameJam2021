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
    private Transform[] _cursorStartTransforms;

	[SerializeField]
	private Transform _spectateTransform;

	[SerializeField]
	private SplitscreenManager _splitscreenManager;

	[SerializeField]
	private CameraOverviewBehaviour _cameraOverviewBehaviour;

	[SerializeField]
	private string _nextSceneName;

	[SerializeField]
	private ParticleSystem _fireworksParticle;

	private GameManager _gameManager;
	private List<PlayerController> _activePlayerControllers;
	public List<PlayerController> PlayerControllers { get => _activePlayerControllers; }

    private List<GameObject> _playerObjList;

    [SerializeField] private GameObject _cursorPrefab;

	public List<PlayerController> FinishedPlayers = new List<PlayerController>();



	//public ILevelStates.LevelState LevelState { get; set; }

	private void Start()
	{
		_activePlayerControllers = new List<PlayerController>();
		_gameManager = FindObjectOfType<GameManager>();

		_playerObjList = new List<GameObject>();

        //LevelState = ILevelStates.LevelState.Overview;

        _cameraOverviewBehaviour.StartOverview();
        _cameraOverviewBehaviour.Event.AddListener(StartPlacement);

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
		_fireworksParticle.Play();

        for (int i = 0; i < _activePlayerControllers.Count; i++)
        {
			GolfBallBehaviour gbb = _activePlayerControllers[i].GetComponentInChildren<GolfBallBehaviour>();
			_gameManager.TotalPlayerScores[i] += gbb.CurrentLevelScore;
        }

		StartCoroutine(LoadScene());
		
	}

	private IEnumerator LoadScene()
	{
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(_nextSceneName);
	}

    private void StartPlacement()
    {
        _cameraOverviewBehaviour.Event.RemoveListener(StartPlacement);

        PlacePlayerCursors(_gameManager.ActivePlayerControllers);
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

    private void PlacePlayerCursors(List<PlayerController> activePlayerControllers)
    {
        for (int i = 0; i < activePlayerControllers.Count; i++)
        {
			//GameObject player = Instantiate(_cursorPrefab, _cursorStartTransforms[i].position, _cursorStartTransforms[i].rotation);
			//PlayerController playerController = player.GetComponent<PlayerController>();

			//  playerController.SetupPlayer(i, _gameManager.Materials[i]);

			var playerInput = PlayerInput.Instantiate(_cursorPrefab, controlScheme:"Gamepad", pairWithDevice:_gameManager.UsedGamepads[i]);
			GameObject playerObject = playerInput.gameObject;

			playerObject.transform.position = _cursorStartTransforms[i].position;
			playerObject.transform.rotation = _cursorStartTransforms[i].rotation;

			_playerObjList.Add(playerObject);
        }
    }
	public void DestroyCursor(GameObject parent)
    {
		_playerObjList.Remove(parent);
		Destroy(parent);

		if(_playerObjList.Count == 0)
        {
			PlayLevel();
        }
    }

	private void PlayLevel()
    {
        _splitscreenManager.InitCameras(_activePlayerControllers, _startTransforms, true);

        //enable controls
        EnableInput();
        for (int i = 0; i < _activePlayerControllers.Count; i++)
        {
            _activePlayerControllers[i].playerInput.SwitchCurrentActionMap("Player Controls");
        }
    }
}

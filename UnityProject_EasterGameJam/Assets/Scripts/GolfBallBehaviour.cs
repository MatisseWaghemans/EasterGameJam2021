using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfBallBehaviour : MonoBehaviour
{

	[SerializeField] private Transform _cameraPivot;

	private LineRenderer _lineRenderer;

	[SerializeField] private float _aimSpeedDirection;


	private CameraBehaviour _cameraBehaviour;

	private Rigidbody _rb;


	public PlayerStates CurrentPlayerState
	{
		get { return _currentPlayerState; }
		set
		{
			//_cameraBehaviour.StateChanged = true;
			_currentPlayerState = value;
		}
	}

	private PlayerStates _currentPlayerState;

	private PlayerStates _previousPlayerState;

	private float _controllerStickValue;

	private Vector3 _controllerStickVector;

	private Gamepad _gamepad = Gamepad.current;

	private Vector3 _aimDirection;

	private bool _spectateStateToggled = false;

	public int CurrentLevelScore = 0;

	private void Start()
	{
		_cameraBehaviour = _cameraPivot.GetComponent<CameraBehaviour>();
		CurrentPlayerState = PlayerStates.Shooting;
		_previousPlayerState = CurrentPlayerState;
		_rb = this.transform.GetComponent<Rigidbody>();
		_lineRenderer = _cameraPivot.GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		switch (CurrentPlayerState)
		{
			case PlayerStates.Paused:
				break;
			case PlayerStates.Shooting:
				UpdatePlayerShootingControls();
				break;
			case PlayerStates.Spectating:
				UpdateSpectatingControls();
				break;
			case PlayerStates.Placing:
				break;
			case PlayerStates.Finished:
				_rb.isKinematic = true;
				break;
			default:
				break;
		}

	}

	private void UpdateSpectatingControls()
	{
		#region Spectate Movement

		// ADD MOVEMENT TOP DOWN CAMERA

		#endregion

		#region To Spectate

		if (_gamepad.buttonNorth.wasPressedThisFrame)
		{
			_spectateStateToggled = !_spectateStateToggled;

			if (_spectateStateToggled)
			{
				CurrentPlayerState = PlayerStates.Shooting;
				_cameraPivot.GetComponent<CameraBehaviour>().RegisterPositions();
				_cameraBehaviour.SpectatingPos.gameObject.SetActive(false);
			}
		}

		#endregion
	}

	private void UpdatePlayerShootingControls()
	{
		//joystick direction 

		Vector3 forward = _cameraPivot.forward;
		forward.y = 0;
		forward.Normalize();

		Vector3 right = _cameraPivot.right;
		right.y = 0;
		right.Normalize();

		_aimDirection = (forward * _gamepad.leftStick.y.ReadValue() + right * _gamepad.leftStick.x.ReadValue());
		if (_aimDirection.magnitude > 1)
		{
			_aimDirection.Normalize();
		}

		if (_gamepad.leftStick.ReadValue() != Vector2.zero && _rb.velocity == Vector3.zero)
		{
			// Debug.DrawLine(this.transform.position, this.transform.position + _aimDirection * _aimSpeedDirection);
			_lineRenderer.SetPosition(0, this.transform.position);
			_lineRenderer.SetPosition(1, this.transform.position + _aimDirection * _aimSpeedDirection);
			//Debug.Log(_aimDirection);
		}


		_controllerStickValue = _gamepad.leftStick.ReadValue().magnitude;
		_controllerStickVector = _aimDirection;

		#region Shoot

		if (_gamepad.buttonSouth.wasPressedThisFrame && _rb.velocity == Vector3.zero && _gamepad.leftStick.ReadValue() != Vector2.zero && CurrentPlayerState != PlayerStates.Finished)
		{
			ShootGolfBall(_controllerStickVector * _aimSpeedDirection);
		}
		if (_gamepad.buttonSouth.wasReleasedThisFrame)
		{
			
		}

		#endregion

		#region To Spectate

		if (_gamepad.buttonNorth.wasPressedThisFrame)
		{
			_spectateStateToggled = !_spectateStateToggled;

			if(_spectateStateToggled)
			{
				CurrentPlayerState = PlayerStates.Spectating;
				_cameraBehaviour.SpectatingPos.gameObject.SetActive(true);
			}
		}

		#endregion

		CheckVelocity(_rb);
	}

	private void ShootGolfBall(Vector3 shootingForce)
	{

		Vector3 flatPlaneVector = new Vector3(shootingForce.x, .1f, shootingForce.z);
		_rb.AddForce(flatPlaneVector * 10f, ForceMode.Impulse);
		CurrentLevelScore++;
		if(CurrentLevelScore == 12)
        {
			CurrentPlayerState = PlayerStates.Finished;
        }
		_lineRenderer.enabled = false;
	}

	private void CheckVelocity(Rigidbody rb)
	{
		if (rb.velocity == Vector3.zero)
		{
			_lineRenderer.enabled = true;
		}
		else
		{
			_lineRenderer.enabled = false;
		}
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
			PlayerFinished();
        }
    }

	private void PlayerFinished()
    {
		//#######################################################
		//#####Store score in total game score (game manager).###
		//######################################################
		CurrentLevelScore = 0;
		CurrentPlayerState = PlayerStates.Finished;
	}
}


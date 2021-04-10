using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfBallBehaviour : MonoBehaviour
{

	[SerializeField] private Transform _cameraPivot;

	[SerializeField] private LineRenderer _lineRenderer;

	[SerializeField] private float _aimSpeedDirection;

	[SerializeField]
	private CameraBehaviour _cameraBehaviour;


	public PlayerStates CurrentPlayerState
	{
		get { return _currentPlayerState; }
		set
		{
			_cameraBehaviour.StateChanged = true;
			_currentPlayerState = value;
		}
	}

	private PlayerStates _currentPlayerState;

	private PlayerStates _previousPlayerState;

	private float _controllerStickValue;

	private Vector3 _controllerStickVector;

	private Gamepad _gamepad = Gamepad.current;

	private Vector3 _aimDirection;

	private bool _ableToShoot = false;

	private bool _southButtonPressed = false;

	private bool _spectateStateToggled = false;

	private void Start()
	{
		CurrentPlayerState = PlayerStates.Shooting;
		_previousPlayerState = CurrentPlayerState;
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

		if (_gamepad.leftStick.ReadValue() != Vector2.zero && this.transform.GetComponent<Rigidbody>().velocity == Vector3.zero)
		{
			// Debug.DrawLine(this.transform.position, this.transform.position + _aimDirection * _aimSpeedDirection);
			_lineRenderer.SetPosition(0, this.transform.position);
			_lineRenderer.SetPosition(1, this.transform.position + _aimDirection * _aimSpeedDirection);
			//Debug.Log(_aimDirection);
		}


		_controllerStickValue = _gamepad.leftStick.ReadValue().magnitude;
		_controllerStickVector = _aimDirection;

		#region Shoot

		if (_gamepad.buttonSouth.wasPressedThisFrame)
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
			}
		}

		#endregion

		CheckVelocity(this.GetComponent<Rigidbody>());
	}

	private void ShootGolfBall(Vector3 shootingForce)
	{

		Vector3 flatPlaneVector = new Vector3(shootingForce.x, .2f, shootingForce.z);
		this.GetComponent<Rigidbody>().AddForce(flatPlaneVector * 10f, ForceMode.Impulse);

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
}


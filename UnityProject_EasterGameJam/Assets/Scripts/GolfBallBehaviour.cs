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

	private float _controllerStickValue;
	private Vector3 _controllerStickVector;

	Gamepad _gamepad = Gamepad.current;

	Vector3 _aimDirection;

	private bool _ableToShoot = false;

	private bool _buttonPressed = false;

	private PlayerStates _currentPlayerState;

	private PlayerStates _previousPlayerState;

	private void Start()
	{
		_currentPlayerState = PlayerStates.Shooting;
		_previousPlayerState = _currentPlayerState;
	}

	// Update is called once per frame
	void Update()
	{
		switch (_currentPlayerState)
		{
			case PlayerStates.Paused:
				break;
			case PlayerStates.Shooting:
				UpdatePlayerShootingControls();
				break;
			case PlayerStates.Spectating:
				break;
			case PlayerStates.Placing:
				break;
			default:
				break;
		}

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

		if (_gamepad.buttonSouth.wasPressedThisFrame)
		{
			_buttonPressed = true;
		}
		if (_gamepad.buttonSouth.wasReleasedThisFrame)
		{
			_buttonPressed = false;
		}
	}

	private void FixedUpdate()
	{
		CheckVelocity(this.GetComponent<Rigidbody>());

		//if (_gamepad.leftStick.ReadValue().magnitude < .01f && _controllerStickValue > .1f && this.transform.GetComponent<Rigidbody>().velocity == Vector3.zero)
		//{

		//   ShootGolfBall(_controllerStickVector * _aimSpeedDirection);

		//}
		Debug.Log(_buttonPressed);
		if (_buttonPressed)
		{
			ShootGolfBall(_controllerStickVector * _aimSpeedDirection);
			_buttonPressed = false;
		}

		_controllerStickValue = _gamepad.leftStick.ReadValue().magnitude;
		_controllerStickVector = _aimDirection;

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


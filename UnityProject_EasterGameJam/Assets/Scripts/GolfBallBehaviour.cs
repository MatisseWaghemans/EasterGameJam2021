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

	private Vector3 _resetShotPosition = Vector3.zero;


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

	public Transform ArrowTransform;

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
			_lineRenderer.SetPosition(1, this.transform.position + _aimDirection * _aimSpeedDirection / 2);
			//Debug.Log(_aimDirection);

			//arrow scaling
			float scale = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(_aimDirection.x * _aimSpeedDirection, 2) * Mathf.Pow(_aimDirection.z * _aimSpeedDirection, 2))) * 5;
			scale = Mathf.Clamp(scale, 1, 5);
			ArrowTransform.localScale = new Vector3(scale, 1/scale, .5f/scale);

			//arrow rotation
			ArrowTransform.localPosition = Vector3.zero;
			ArrowTransform.localRotation = Quaternion.Euler(-90 + _aimDirection.x * 360, 0, _aimDirection.y * 360 + 90);
			ArrowTransform.localPosition = Vector3.forward * .25f;
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

			if (_spectateStateToggled)
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


		_resetShotPosition = this.transform.position;
		Vector3 flatPlaneVector = new Vector3(shootingForce.x, .05f, shootingForce.z);
		_rb.AddForce(flatPlaneVector * 10f, ForceMode.Impulse);
		CurrentLevelScore++;
		if (CurrentLevelScore == 12)
		{
			CurrentPlayerState = PlayerStates.Finished;
		}
		_lineRenderer.enabled = false;
		ArrowTransform.gameObject.SetActive(true);
	}

	private void CheckVelocity(Rigidbody rb)
	{
		if (rb.velocity == Vector3.zero)
		{
			_lineRenderer.enabled = true;
			ArrowTransform.gameObject.SetActive(true);
		}
		else
		{
			_lineRenderer.enabled = false;
			ArrowTransform.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Finish"))
		{
			PlayerFinished();
		}
		if (other.gameObject.CompareTag("OutOfBounds"))
		{
			this.transform.position = _resetShotPosition;
			_rb.velocity = Vector3.zero;
			_rb.constraints = RigidbodyConstraints.FreezeAll;
			StartCoroutine(ResetForces());
		}
	}
	private bool sldks;
	private void PlayerFinished()
	{
		//#######################################################
		//#####Store score in total game score (game manager).		ALS GE EEN NIEUW LEVEL LAADT -> rigidbody constraints afzetten###
		//######################################################
		CurrentLevelScore = 0;
		CurrentPlayerState = PlayerStates.Finished;
		
		_rb.velocity = Vector3.zero;

	}
	private bool AllFinished(bool test)
	{
		bool newbool = test;
		return newbool;
	}

	private IEnumerator ResetForces()
	{
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		yield return new WaitForEndOfFrame();
		_rb.constraints = RigidbodyConstraints.None;
	}
}


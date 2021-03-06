using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GolfBallBehaviour : MonoBehaviour
{

	[SerializeField] private Transform _cameraPivot;

	[SerializeField] private Renderer _arrowRenderer;

	[SerializeField] private PlayerController _playerController;

	[SerializeField] private float _velocityThreshhold = 0.05f;

	private LevelManager _levelManager;

	

	[SerializeField]
	private ParticleSystem _hitParticle;

	[SerializeField]
	private Transform _hitParticleParent;


	private LineRenderer _lineRenderer;

	[SerializeField] private float _aimSpeedDirection;


	private CameraBehaviour _cameraBehaviour;

	private Rigidbody _rb;

	private Vector3 _resetShotPosition = Vector3.zero;


	public PlayerStates CurrentPlayerState { get; set; }

	private float _controllerStickValue;

	private Vector3 _controllerStickVector;

	private Gamepad _gamepad = Gamepad.current;

	private Vector3 _aimDirection;

	private bool _spectateStateToggled = false;

	public int CurrentLevelScore = 0;

	public Transform ArrowParentTransform;

	public Transform ArrowTransform;
	public TextMeshProUGUI TextMeshProUGUI;


	private void Start()
	{
		_cameraBehaviour = _cameraPivot.GetComponent<CameraBehaviour>();
		CurrentPlayerState = PlayerStates.Shooting;
		_rb = this.transform.GetComponent<Rigidbody>();
		_lineRenderer = _cameraPivot.GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	//void Update()
	//{
	//	switch (CurrentPlayerState)
	//	{
	//		case PlayerStates.Paused:
	//			break;
	//		case PlayerStates.Shooting:
	//			UpdatePlayerShootingControls();
	//			break;
	//		case PlayerStates.Spectating:
	//			UpdateSpectatingControls();
	//			break;
	//		case PlayerStates.Placing:
	//			break;
	//		case PlayerStates.Finished:
	//			break;
	//		default:
	//			break;
	//	}

	//}

	private void Update()
	{
		_hitParticleParent.transform.position = transform.position;
		_hitParticleParent.transform.LookAt(this.transform.position + _aimDirection * _aimSpeedDirection / 2);
	}

	private void UpdateSpectatingControls()
	{
		_arrowRenderer.enabled = false;

		#region Spectate Movement

		// ADD MOVEMENT TOP DOWN CAMERA

		#endregion

		//#region To Spectate

		//if (_gamepad.buttonNorth.wasPressedThisFrame)
		//{
		//		CurrentPlayerState = PlayerStates.Shooting;
		//		_cameraPivot.GetComponent<CameraBehaviour>().RegisterPositions();
		//		_cameraBehaviour.SpectatingPos.gameObject.SetActive(false);
		//}

		//#endregion
	}

	public void OnShoot(InputAction.CallbackContext value)
	{
		if (value.started)
		{
			if (_rb.velocity.magnitude <= _velocityThreshhold && _controllerStickVector != Vector3.zero && CurrentPlayerState != PlayerStates.Finished)
			{
				ShootGolfBall(_controllerStickVector * _aimSpeedDirection);
			}
		}
	}

	public void OnSpectate(InputAction.CallbackContext value)
	{
		if (CurrentPlayerState == PlayerStates.Finished)
			return;

		if (value.started)
		{
			if (CurrentPlayerState == PlayerStates.Spectating)
			{
				//TODO check if player 

				CurrentPlayerState = PlayerStates.Shooting;
				_cameraPivot.GetComponent<CameraBehaviour>().RegisterPositions();
				_arrowRenderer.enabled = true;
			}
			else if (CurrentPlayerState == PlayerStates.Shooting)
			{
				CurrentPlayerState = PlayerStates.Spectating;

				_arrowRenderer.enabled = false;
			}

			_cameraBehaviour.ChangeFocus();
		}
	}

	public void OnAim(InputAction.CallbackContext value)
	{
		Vector2 inputMovement = value.ReadValue<Vector2>();
		UpdateGolfballRendererActive();

		Vector3 forward = _cameraPivot.forward;
		forward.y = 0;
		forward.Normalize();

		Vector3 right = _cameraPivot.right;
		right.y = 0;
		right.Normalize();

		_aimDirection = (forward * inputMovement.y + right * inputMovement.x);
		if (_aimDirection.magnitude > 1)
		{
			_aimDirection.Normalize();
		}

		if (inputMovement != Vector2.zero && _rb.velocity.magnitude <= _velocityThreshhold)
		{
			//arrow scaling
			float scale = ((this.transform.position + _aimDirection * _aimSpeedDirection / 2) - (this.transform.position)).magnitude * 5;

			scale = Mathf.Clamp(scale, 0, 5);

			ArrowTransform.localScale = new Vector3(scale, ArrowTransform.localScale.y, ArrowTransform.localScale.z);

			//arrow rotation
			ArrowParentTransform.LookAt(this.transform.position + _aimDirection * _aimSpeedDirection / 2);
		}
		else if(inputMovement == Vector2.zero)
		{
			ArrowTransform.localScale = new Vector3(0, ArrowTransform.localScale.y, ArrowTransform.localScale.z);
		}

		_controllerStickValue = inputMovement.magnitude;
		_controllerStickVector = _aimDirection;

		CheckVelocity(_rb);
	}

	private void UpdateGolfballRendererActive()
	{
		if (ArrowTransform.localScale.x <= 0.05f)
		{
			_arrowRenderer.enabled = false;
		}
		else
		{
			_arrowRenderer.enabled = true;
		}
	}

	private void ShootGolfBall(Vector3 shootingForce)
	{


		_resetShotPosition = this.transform.position;
		Vector3 flatPlaneVector = new Vector3(shootingForce.x, .05f, shootingForce.z);
		_rb.AddForce(flatPlaneVector * 10f, ForceMode.Impulse);

		_hitParticle.Play();

		if (CurrentLevelScore == 99)
		{
			CurrentPlayerState = PlayerStates.Finished;
		}
		_lineRenderer.enabled = false;
		ArrowTransform.gameObject.SetActive(true);

		UpdateScoreUI();

	}

    private void UpdateScoreUI()
    {
		CurrentLevelScore++;
		TextMeshProUGUI.text = CurrentLevelScore.ToString();

	}

    private void CheckVelocity(Rigidbody rb)
	{
		if (rb.velocity.magnitude <= _velocityThreshhold)
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
			_levelManager = GameObject.FindObjectOfType<LevelManager>();
			PlayerFinished();
			_levelManager.CheckPlayersFinished();
		}
	}
	public void KillPlayer()
	{
		this.transform.position = _resetShotPosition;
		_rb.velocity = Vector3.zero;
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		StartCoroutine(ResetForces());
	}

	private void PlayerFinished()
	{
		//#######################################################
		//#####Store score in total game score (game manager).		ALS GE EEN NIEUW LEVEL LAADT -> rigidbody constraints afzetten###
		//######################################################
		GameManager.Instance.TotalPlayerScores[_playerController.ControllerID] += CurrentLevelScore;

		CurrentLevelScore = 0;
		CurrentPlayerState = PlayerStates.Finished;

		_rb.velocity = Vector3.zero;

	}

	private IEnumerator ResetForces()
	{
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		yield return new WaitForEndOfFrame();
		_rb.constraints = RigidbodyConstraints.None;
	}
}


using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehaviour : MonoBehaviour
{
    

    private Vector3 _cameraOffset = Vector3.zero;

    [Tooltip("Higher number means slower lerp time") ]
    [SerializeField] private float _lerpSpeed = 1;

    [SerializeField] private float _rotationSpeed = 1.5f;

	[SerializeField]
	private Gamepad _gamepad;

	private GolfBallBehaviour _golfBall;

	public Transform TargettedGolfBall;

    private Vector3 _currentCameraVelocity = Vector3.zero;

    private Transform _cameraTargetPos;

	private Transform _shootingPos;
    public Transform SpectateTransform;


    public bool StateChanged;
    private Vector2 _lookValue;

    // Start is called before the first frame update
    void Start()
    {
        _golfBall = TargettedGolfBall.GetComponent<GolfBallBehaviour>();
        _golfBall.CurrentPlayerState = PlayerStates.Shooting;
        _cameraOffset = this.transform.position - TargettedGolfBall.transform.position;
        _shootingPos = this.transform;
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        Vector2 inputLook = value.ReadValue<Vector2>();
        _lookValue = inputLook;
    }

    // Update is called once per frame
    void Update()
    {
		//_gamepad = Gamepad.current;

        if (_lookValue.x > 0 && _golfBall.CurrentPlayerState == PlayerStates.Shooting)
        {
            this.transform.RotateAround(TargettedGolfBall.position, Vector3.up, _lookValue.x * _rotationSpeed);
        }
        else if (_lookValue.x < 0 && _golfBall.CurrentPlayerState == PlayerStates.Shooting)
        {
            this.transform.RotateAround(TargettedGolfBall.position, Vector3.up, _lookValue.x * _rotationSpeed);
        }

        if (_golfBall.CurrentPlayerState == PlayerStates.Shooting)
        {
             _shootingPos.position = TargettedGolfBall.position + _cameraOffset;
             _shootingPos.rotation = this.transform.rotation;
            this.transform.position = Vector3.SmoothDamp(this.transform.position, TargettedGolfBall.position + _cameraOffset, ref _currentCameraVelocity, .3f);
        }
    }

    public void ChangeFocus()
    {
        switch (_golfBall.CurrentPlayerState)
        {
            case PlayerStates.Paused:
                break;
            case PlayerStates.Shooting:
                _cameraTargetPos = _shootingPos;
                this.transform.rotation = _cameraTargetPos.rotation;
                break;
            case PlayerStates.Spectating:
                _cameraTargetPos = SpectateTransform;
                this.transform.position = _cameraTargetPos.position;
                this.transform.rotation = _cameraTargetPos.rotation;
                break;
            case PlayerStates.Placing:
                break;
            default:
                break;
        }
    }

    public void RegisterPositions()
    {

        //_shootingPos.position = TargettedGolfBall.position + _cameraOffset;
        //_shootingPos.rotation = this.transform.rotation;
    }
}

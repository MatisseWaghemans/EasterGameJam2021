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

	[SerializeField]
	private GolfBallBehaviour _golfBall;

	public Transform TargettedGolfBall;

    private Vector3 _currentCameraVelocity = Vector3.zero;

	public bool StateChanged;

	// Start is called before the first frame update
	void Start()
    {
        this.transform.position = TargettedGolfBall.transform.position;
        _cameraOffset = this.transform.position - TargettedGolfBall.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
		_gamepad = Gamepad.current;

		switch (_golfBall.CurrentPlayerState)
		{
			case PlayerStates.Paused:
				break;
			case PlayerStates.Shooting:
				break;
			case PlayerStates.Spectating:
				break;
			case PlayerStates.Placing:
				break;
			default:
				break;
		}

        if (_gamepad.rightStick.x.ReadValue() >= 0)
        {
            this.transform.Rotate(Vector3.up, _gamepad.rightStick.x.ReadValue() * _rotationSpeed);
        }
        if (_gamepad.rightStick.x.ReadValue() <= 0)
        {
            this.transform.Rotate(Vector3.up, _gamepad.rightStick.x.ReadValue() * _rotationSpeed);
        }
        
        //this.transform.position = Vector3.Lerp(this.transform.position, TargettedGolfBall.position + _cameraOffset, Vector3.Distance(TargettedGolfBall.position, _cameraOffset) / _lerpSpeed);
    
        this.transform.position = Vector3.SmoothDamp(this.transform.position, TargettedGolfBall.position + _cameraOffset, ref _currentCameraVelocity, .3f);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehaviour : MonoBehaviour
{
    

    private Vector3 _cameraOffset = Vector3.zero;

    [Tooltip("Higher number means slower lerp time") ]
    [SerializeField] private float _lerpSpeed = 1;


    public Transform TargettedGolfBall;


    // Start is called before the first frame update
    void Start()
    {
        _cameraOffset = this.transform.position - TargettedGolfBall.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;

        if (gamepad.rightStick.x.ReadValue() >= 0)
        {
            this.transform.Rotate(Vector3.up, gamepad.rightStick.x.ReadValue());
        }
        if (gamepad.rightStick.x.ReadValue() <= 0)
        {
            this.transform.Rotate(Vector3.up, gamepad.rightStick.x.ReadValue());
        }

        this.transform.position = Vector3.Lerp(this.transform.position, TargettedGolfBall.position + _cameraOffset, Vector3.Distance(TargettedGolfBall.position, _cameraOffset) / _lerpSpeed);
    }
}

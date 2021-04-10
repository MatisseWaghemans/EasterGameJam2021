using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfBallBehaviour : MonoBehaviour
{

    [SerializeField] private Transform _cameraPivot;

    [SerializeField] private float _aimSpeedDirection;

    private float _controllerStickValue;
    private Vector3 _controllerStickVector;

    Gamepad _gamepad = Gamepad.current;

    Vector3 _aimDirection;

    private bool _ableToShoot = false;

    private bool _buttonPressed = false;

    // Update is called once per frame
    void Update()
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
            _cameraPivot.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
            _cameraPivot.GetComponent<LineRenderer>().SetPosition(1, this.transform.position + _aimDirection * _aimSpeedDirection);
            //Debug.Log(_aimDirection);
        }

        if (_gamepad.buttonSouth.isPressed)
        {
            _buttonPressed = true;
        }

    }
    private void FixedUpdate()
    {
        CheckVelocity(this.GetComponent<Rigidbody>());

        //if (_gamepad.leftStick.ReadValue().magnitude < .01f && _controllerStickValue > .1f && this.transform.GetComponent<Rigidbody>().velocity == Vector3.zero)
        //{

        //   ShootGolfBall(_controllerStickVector * _aimSpeedDirection);

        //}

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

        _cameraPivot.GetComponent<LineRenderer>().enabled = false;
    }

    private void CheckVelocity(Rigidbody rb)
    {
        if(rb.velocity == Vector3.zero)
        {
            _cameraPivot.GetComponent<LineRenderer>().enabled = true;
        }
        else
        {
            _cameraPivot.GetComponent<LineRenderer>().enabled = false;
        }
    }
}

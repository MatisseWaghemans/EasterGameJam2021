using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GolfBallBehaviour : MonoBehaviour
{

    [SerializeField] private Transform _cameraPivot;

    [SerializeField] private float _aimSpeed;

    private float _controllerStickValue;
    private Vector3 _controllerStickVector;

    Gamepad _gamepad = Gamepad.current;

    Vector3 _aimDirection;


    // Update is called once per frame
    void Update()
    {
        //TODO: change this to right input controller.
        

      


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

        if (_gamepad.leftStick.ReadValue() != Vector2.zero)
        {
            Debug.DrawLine(this.transform.position, _aimDirection * _aimSpeed);
            Debug.Log(this.transform.position);
        }

 
       
    }
    private void FixedUpdate()
    {
        if (_gamepad.leftStick.ReadValue().magnitude < .01 && _controllerStickValue > .05)
        {
            Debug.Log("Shoot");
            Debug.Log(_controllerStickVector);

            ShootGolfBall(_controllerStickVector * _aimSpeed);
            
        }

        _controllerStickValue = _gamepad.leftStick.ReadValue().magnitude;
        _controllerStickVector = _aimDirection;
            



       // Debug.Log(_gamepad.leftStick.ReadValue().magnitude);
    }

    private void ShootGolfBall(Vector3 shootingForce)
    {
        Vector3 flatPlaneVector = new Vector3(shootingForce.x, 0, shootingForce.z);
        this.GetComponent<Rigidbody>().AddForce(flatPlaneVector * 10f, ForceMode.Impulse);
    }
}

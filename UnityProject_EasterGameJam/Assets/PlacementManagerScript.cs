using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.Events;

public class PlacementManagerScript : MonoBehaviour
{
    public static PlacementManagerScript Instance;

    public GameObject ControllerPointer;

    public bool _isHoldingItem;
    public bool _placeable;

    public GameObject CurrentObject;

    public float TranslateSpeed = 0.2f;
    public float RotationSpeed = 0.75f;

    public Material matGreen;
    public Material matRed;

    private Vector2 axisValues;
    private Vector2 rotationAxisValues;
    private Material _startMaterial;

    public LayerMask PlaceableFloor;

    public UnityEvent PlacedEvent;

    protected void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ControllerPointer = this.gameObject;
    }

    void Update()
    {
        ControllerPointer.transform.Translate(new Vector3(axisValues.x, 0, axisValues.y) * TranslateSpeed);

        if (_isHoldingItem)
        {
            if (rotationAxisValues != Vector2.zero && ControllerPointer.transform.GetChild(0) != null)
            {
                ControllerPointer.transform.GetChild(0).Rotate(Vector3.forward, rotationAxisValues.x * RotationSpeed, Space.Self);
            }
        }

        if (CurrentObject != null)
        {
            CheckPlacement();
        }
    }

    private void CheckPlacement()
    {
        Ray ray = new Ray(CurrentObject.transform.position, Vector3.down);

        if(Physics.Raycast(ray, out var hit, 100f))
        {
            if(hit.transform.gameObject.layer == 12)
            {

                 Debug.Log(hit.transform.name);
                CurrentObject.GetComponent<MeshRenderer>().material = matGreen;
                _placeable = true;
            }
            else
            {
                Debug.Log(hit.transform.gameObject.layer);
            CurrentObject.GetComponent<MeshRenderer>().material = matRed;
            _placeable = false;
            }
           
        }
        else
        {
        }
    }

    public void Movement(CallbackContext value)
    { 
        axisValues = value.ReadValue<Vector2>();
    }
    public void Rotate(CallbackContext value)
    {
        rotationAxisValues = value.ReadValue<Vector2>();
    }

    public void Interact(CallbackContext value)
    {
        if (value.started)
        {
            Ray ray = new Ray(ControllerPointer.transform.position, Vector3.down);

            if (!_isHoldingItem)
            {
                if (Physics.Raycast(ray, out var hit, 100f))
                {
                    if (hit.transform.gameObject.layer == 13)
                    {
                        var currentTransform = hit.transform;
                        PickUpObject(currentTransform);
                    }
                }
            }
            else if (_placeable)
            {
                if (Physics.Raycast(ray, out var hit, 100f))
                {
                    PlaceObject(hit);
                    _placeable = false;
                }
            }
        }
    }

    public void PickUpObject(Transform trans)
    {
        trans.parent = ControllerPointer.transform;
        CurrentObject = trans.gameObject;
        CurrentObject.layer = 2;
        _isHoldingItem = true;
        _startMaterial = CurrentObject.GetComponent<MeshRenderer>().material;
    }
    public void PlaceObject(RaycastHit hit)
    {
        CurrentObject.transform.parent = null;
        CurrentObject.transform.position = new Vector3(CurrentObject.transform.position.x, hit.point.y, CurrentObject.transform.position.z);
        CurrentObject.transform.gameObject.isStatic = true;
        CurrentObject.GetComponent<MeshRenderer>().material = _startMaterial;
        CurrentObject = null;
        _isHoldingItem = false;
        FindObjectOfType<LevelManager>().DestroyCursor(this.transform.parent.gameObject);
    }
}

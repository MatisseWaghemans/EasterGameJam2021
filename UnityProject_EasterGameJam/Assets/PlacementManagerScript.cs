using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManagerScript : MonoBehaviour
{
    public static PlacementManagerScript Instance;

    public GameObject ControllerPointer;

    public List<GolfBallBehaviour> GolfBallBehaviours = new List<GolfBallBehaviour>();

    public bool _isHoldingItem;
    public bool _placeable;

    public LayerMask PickUpObjectLayer;

    public LevelManager LevelManager;

    public GameObject CurrentObject;

    public float TranslateSpeed;

    public float RotationSpeed;


    protected void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        ControllerPointer = this.gameObject;


    }

    // Update is called once per frame
    void Update()
    {
        

        CheckPlacement();
        
    }

    private void CheckPlacement()
    {
       Ray ray = new Ray( ControllerPointer.transform.position, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100f))
        {
            if(hit.transform.gameObject.layer == 12)
            {
                //groene overlay
                _placeable = true;
            }
            else
            {
                //rode overlay en niet placeable
                _placeable = false;

            }
        }
    }

    public void Movement(Vector2 axisValues)
    {
        if(axisValues != Vector2.zero)
        {
            ControllerPointer.transform.Translate(new Vector3(axisValues.x, 0, axisValues.y) * TranslateSpeed) ;
        }
    }
    public void Interact()
    {
        Transform currentTransform;

        Ray ray = new Ray(ControllerPointer.transform.position, Vector3.down);


        if (!_isHoldingItem)
        {
            if (Physics.Raycast(ray, out var hit, 100f))
            {
                if(hit.transform.gameObject.layer == 13)
                {

                    currentTransform = hit.transform;
                    PickUpObject(currentTransform);
                }
            }
        }
        else if(_placeable)
        {
            if (Physics.Raycast(ray, out var hit, 100f))
            {
                
                PlaceObject(hit);
                _placeable = false;
            }
            
        }
    }

    public void PickUpObject(Transform trans)
    {
        trans.parent = ControllerPointer.transform;
        CurrentObject = trans.gameObject;
        trans.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        _isHoldingItem = true;
        
    }
    public void PlaceObject(RaycastHit hit)
    {
       CurrentObject.transform.parent = null;
        CurrentObject.transform.position = new Vector3(CurrentObject.transform.position.x, hit.point.y, CurrentObject.transform.position.z);
       CurrentObject.transform.gameObject.isStatic = true;
        _isHoldingItem = false;

        //GolfBallBehaviours[controllerID].enabled = false;

    }
    public void Rotate(Vector2 axisValues)
    {
        if (_isHoldingItem)
        {

             if (axisValues != Vector2.zero && ControllerPointer.transform.GetChild(0) != null)
             {
                 ControllerPointer.transform.GetChild(0).Rotate(Vector3.forward, axisValues.x * RotationSpeed, Space.Self);
             }
        }
    }

    public void CheckFinishedPlacement()
    {
        GolfBallBehaviour[] allPlayers = FindObjectsOfType<GolfBallBehaviour>();

        bool allFinished = true;
        foreach (var item in allPlayers)
        {
            if (item.enabled)
            {
                allFinished = false;
                return;
            }
        }

        if (allFinished)
        {
            foreach (var item in allPlayers)
            {

                item.enabled = true;
                item.CurrentPlayerState = PlayerStates.Shooting;
            }
        }
    }
}

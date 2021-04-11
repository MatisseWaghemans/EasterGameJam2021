using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class PlayerController : MonoBehaviour
{

    //Player ID
    public int ControllerID;

    [Header("Sub Behaviours")]
    public PlayerMovementBehaviour playerMovementBehaviour;
    public PlayerAnimationBehaviour playerAnimationBehaviour;
    public PlayerVisualsBehaviour playerVisualsBehaviour;

	public GolfBallBehaviour PlayerGolfBallBehaviour;


	[Header("Input Settings")]
    public PlayerInput playerInput;
    public float movementSmoothingSpeed = 1f;
    private Vector3 rawInputMovement;
    private Vector3 smoothInputMovement;
    
    //Action Maps
    private string actionMapPlayerControls = "Player Controls";
    private string actionMapMenuControls = "Menu Controls";
	private string actionMapSpectatingControls = "Spectating Controls";
	private string actionMapPlacementControls = "Placement Controls";

	//Current Control Scheme
	private string currentControlScheme;

    public bool IsDisconnected = false;

    [SerializeField]
    private Camera _camera;
	public Camera Camera { get => _camera; }

	[SerializeField]
    private MeshRenderer _meshRenderer;

	[SerializeField]
	private PlacementController _placementController;

	//This is called from the GameManager; when the game is being setup.
	public void SetupPlayer(int newControllerID, Material material)
    {
        Debug.Log($"tried connecting: {playerInput.currentControlScheme}");
        ControllerID = newControllerID;

        _meshRenderer.material = material;

        currentControlScheme = playerInput.currentControlScheme;

        playerVisualsBehaviour.SetupBehaviour(ControllerID, playerInput, material);
    }


	//INPUT SYSTEM ACTION METHODS --------------

	//This is called from PlayerInput; when a joystick or arrow keys has been pushed.
	//It stores the input Vector as a Vector3 to then be used by the smoothing function.

	public void OnSelectNextObject(InputAction.CallbackContext value)
	{
			PlacementController.Instance.HandleNewObjectHotkey();
	}



    //This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
    public void OnAttack(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            playerAnimationBehaviour.PlayAttackAnimation();
        }
    }

	//This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
	public void PlaceObstacle(InputAction.CallbackContext value)
	{
		if (value.started)
		{
			Debug.Log("Place");
			PlacementController.Instance.ReleaseObject();
		}
	}

	//This is called from Player Input, when a button has been pushed, that correspons with the 'TogglePause' action
	public void OnTogglePause(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            GameManager.Instance.TogglePauseState(this);
        }
    }

    public void OnSubmit(InputAction.CallbackContext value)
    {
        Debug.Log("Submit button pressed");
        if (value.started)
        {
            GameManager.Instance.Submit(this);
        }
    }

    public void OnExitState(InputAction.CallbackContext value)
	{
		if (value.started)
		{
			GameManager.Instance.ExitState(this);
		}
	}

	//INPUT SYSTEM AUTOMATIC CALLBACKS --------------

	//This is automatically called from PlayerInput, when the input device has changed
	//(IE: Keyboard -> Xbox Controller)
	public void OnControlsChanged()
    {

        if(playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;

            playerVisualsBehaviour.UpdatePlayerVisuals();
            RemoveAllBindingOverrides();
        }
    }

    //This is automatically called from PlayerInput, when the input device has been disconnected and can not be identified
    //IE: Device unplugged or has run out of batteries



    public void OnDeviceLost()
    {
        IsDisconnected = true;
        GameManager.Instance.TryActivateStart();
        playerVisualsBehaviour.SetDisconnectedDeviceVisuals();
    }


    public void OnDeviceRegained()
    {
        StartCoroutine(WaitForDeviceToBeRegained());
    }

    IEnumerator WaitForDeviceToBeRegained()
    {
        yield return new WaitForSeconds(0.1f);
        playerVisualsBehaviour.UpdatePlayerVisuals();
        IsDisconnected = false;
        GameManager.Instance.TryActivateStart();
    }

    //Update Loop - Used for calculating frame-based data
    void Update()
    {

	}

	//Input's Axes values are raw
    public void SetInputActiveState(bool gameIsPaused)
    {
        switch (gameIsPaused)
        {
            case true:
                playerInput.DeactivateInput();
                break;

            case false:
                playerInput.ActivateInput();
                break;
        }
    }

    void RemoveAllBindingOverrides()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(playerInput.currentActionMap);
    }



    //Switching Action Maps ----


    
    public void EnableGameplayControls()
    {
        playerInput.SwitchCurrentActionMap(actionMapPlayerControls);  
    }

    public void EnablePauseMenuControls()
    {
        playerInput.SwitchCurrentActionMap(actionMapMenuControls);
    }

	public void EnableSpectatingControls()
	{
		playerInput.SwitchCurrentActionMap(actionMapSpectatingControls);
	}

	public void EnablePlacementControls()
	{
		playerInput.SwitchCurrentActionMap(actionMapPlacementControls);
	}

	//Get Data ----
	public int GetPlayerID()
    {
        return ControllerID;
    }

    public InputActionAsset GetActionAsset()
    {
        return playerInput.actions;
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }


}

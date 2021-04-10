using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    //Player ID
    private int playerID;

    [Header("Sub Behaviours")]
    public PlayerMovementBehaviour playerMovementBehaviour;
    public PlayerAnimationBehaviour playerAnimationBehaviour;
    public PlayerVisualsBehaviour playerVisualsBehaviour;


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


    //This is called from the GameManager; when the game is being setup.
    public bool TrySetupPlayer(int newPlayerID)
    {
        playerID = newPlayerID;

        currentControlScheme = playerInput.currentControlScheme;
		if (currentControlScheme != "Gamepad")
		{
            return false;
		}

        playerMovementBehaviour.SetupBehaviour();
        playerAnimationBehaviour.SetupBehaviour();
        playerVisualsBehaviour.SetupBehaviour(playerID, playerInput);

        return true;
    }


    //INPUT SYSTEM ACTION METHODS --------------

    //This is called from PlayerInput; when a joystick or arrow keys has been pushed.
    //It stores the input Vector as a Vector3 to then be used by the smoothing function.


    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    //This is called from PlayerInput, when a button has been pushed, that corresponds with the 'Attack' action
    public void OnAttack(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            playerAnimationBehaviour.PlayAttackAnimation();
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

<<<<<<< Updated upstream
	public void OnExitState(InputAction.CallbackContext value)
	{
		if (value.started)
		{
			GameManager.Instance.ExitState(this);
		}
	}
=======
    public void OnSubmit(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            GameManager.Instance.Submit(this);
        }
    }

    public void OnCancel(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            GameManager.Instance.Cancel(this);
        }
    }

    //INPUT SYSTEM AUTOMATIC CALLBACKS --------------
>>>>>>> Stashed changes

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
        playerVisualsBehaviour.SetDisconnectedDeviceVisuals();

        //display UI untill reconnected
    }


    public void OnDeviceRegained()
    {
        StartCoroutine(WaitForDeviceToBeRegained());
    }

    IEnumerator WaitForDeviceToBeRegained()
    {
        yield return new WaitForSeconds(0.1f);
        playerVisualsBehaviour.UpdatePlayerVisuals();
        //remove disconnected UI
    }

    //Update Loop - Used for calculating frame-based data
    void Update()
    {
        CalculateMovementInputSmoothing();
        UpdatePlayerMovement();
        UpdatePlayerAnimationMovement();
    }

    //Input's Axes values are raw


    void CalculateMovementInputSmoothing()
    {
        smoothInputMovement = Vector3.Lerp(smoothInputMovement, rawInputMovement, Time.deltaTime * movementSmoothingSpeed);
    }

    void UpdatePlayerMovement()
    {
        playerMovementBehaviour.UpdateMovementData(smoothInputMovement);
    }

    void UpdatePlayerAnimationMovement()
    {
        playerAnimationBehaviour.UpdateMovementAnimation(smoothInputMovement.magnitude);
    }


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

<<<<<<< Updated upstream
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
=======
    //Get Data ----
    public int GetPlayerID()
>>>>>>> Stashed changes
    {
        return playerID;
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

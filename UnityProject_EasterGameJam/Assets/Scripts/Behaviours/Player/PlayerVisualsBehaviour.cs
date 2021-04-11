using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisualsBehaviour : MonoBehaviour
{

    //Player ID
    private int controllerID;
    private PlayerInput playerInput;

    [Header("Device Display Settings")]
    public DeviceDisplayConfigurator deviceDisplaySettings;

    [Header("Sub Behaviours")]
    public PlayerUIDisplayBehaviour playerUIDisplayBehaviour;

    [Header("Player Material")]
    public SkinnedMeshRenderer playerSkinnedMeshRenderer;

    private Material _material;

    public void SetupBehaviour(int newControllerID, PlayerInput newPlayerInput, Material material)
    {
        controllerID = newControllerID;
        playerInput = newPlayerInput;

        _material = material;

		UpdatePlayerVisuals();
	}

    public void UpdatePlayerVisuals()
    {
        UpdateUIDisplay();
    }

    void UpdateUIDisplay()
    {
        playerUIDisplayBehaviour.UpdatePlayerIDDisplayText(controllerID);

        string deviceName = "Player " + controllerID;
        playerUIDisplayBehaviour.UpdatePlayerDeviceNameDisplayText(deviceName);

        Color deviceColor = _material.color;
        playerUIDisplayBehaviour.UpdatePlayerIconDisplayColor(deviceColor);
    }

    public void SetDisconnectedDeviceVisuals()
    {
        string disconnectedName = deviceDisplaySettings.GetDisconnectedName();
        playerUIDisplayBehaviour.UpdatePlayerDeviceNameDisplayText(disconnectedName);

        Color disconnectedColor = deviceDisplaySettings.GetDisconnectedColor();
        playerUIDisplayBehaviour.UpdatePlayerIconDisplayColor(disconnectedColor);       
    }
}

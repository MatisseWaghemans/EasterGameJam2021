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

    private int clothingTintShaderID;
    private Material _material;

    public void SetupBehaviour(int newControllerID, PlayerInput newPlayerInput, Material material)
    {
        controllerID = newControllerID;
        playerInput = newPlayerInput;

        _material = material;

        SetupShaderIDs();

		UpdatePlayerVisuals();
	}

    void SetupShaderIDs()
    {
        //clothingTintShaderID = Shader.PropertyToID("_Clothing_Tint");
    }

    public void UpdatePlayerVisuals()
    {
        UpdateUIDisplay();
        UpdateCharacterShader();
    }

    void UpdateUIDisplay()
    {
        playerUIDisplayBehaviour.UpdatePlayerIDDisplayText(controllerID);

        string deviceName = "Player " + controllerID;
        playerUIDisplayBehaviour.UpdatePlayerDeviceNameDisplayText(deviceName);

        Color deviceColor = deviceDisplaySettings.GetDeviceColor(playerInput);
        playerUIDisplayBehaviour.UpdatePlayerIconDisplayColor(deviceColor);
    }

    void UpdateCharacterShader()
    {
        Color deviceColor = deviceDisplaySettings.GetDeviceColor(playerInput);
        //playerSkinnedMeshRenderer.material.SetColor(clothingTintShaderID, deviceColor);
    }

    public void SetDisconnectedDeviceVisuals()
    {
        string disconnectedName = deviceDisplaySettings.GetDisconnectedName();
        playerUIDisplayBehaviour.UpdatePlayerDeviceNameDisplayText(disconnectedName);

        Color disconnectedColor = deviceDisplaySettings.GetDisconnectedColor();
        playerUIDisplayBehaviour.UpdatePlayerIconDisplayColor(disconnectedColor);
        //playerSkinnedMeshRenderer.material.SetColor(clothingTintShaderID, disconnectedColor);
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SplitscreenManager : MonoBehaviour
{
    [SerializeField] private Camera _cameraPrefab;
    private Camera[] _cameras;

    public void InitCameras(List<PlayerController> players, Transform[] startTransforms, bool autoEnable = false)
    {
        _cameras = new Camera[players.Count];

        for (int i = 0; i < _cameras.Length; i++)
        {
            //TODO fix offset & camera prefab from CameraBehaviour
            _cameras[i] = Instantiate(_cameraPrefab, startTransforms[i].position, startTransforms[i].rotation);
            _cameras[i].enabled = autoEnable;
        }

        SetCameraPositions();
    }

    public void EnableCameras(bool enabled)
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].enabled = enabled;
        }
    }

    void SetCameraPositions()
    {
        switch (_cameras.Length)
        {
            case 2:
                _cameras[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
                _cameras[1].rect = new Rect(0f, 0f, 1f, 0.5f);
                break;
            case 3:
                _cameras[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                _cameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                _cameras[2].rect = new Rect(0, 0f, 1f, 0.5f);
                break;
            case 4:
                _cameras[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                _cameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                _cameras[2].rect = new Rect(0f, 0f, 0.5f, 0.5f);
                _cameras[3].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                break;
        }
    }
}

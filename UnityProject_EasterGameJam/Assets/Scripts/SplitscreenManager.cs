using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitscreenManager : MonoBehaviour
{
    [SerializeField] private int _amountOfPlayers = 4;
    [SerializeField] private Camera _cameraPrefab;
    private Camera[] _cameras;
    private GameObject[] _players;

    enum CameraState
    {
        Overview,
        TopDown,
        SplitScreen
    };

    void Start()
    {
        SetPlayerAmount();
        InitCameras();
        SetCameraPositions();
    }

    void InitCameras()
    {
        _cameras = new Camera[_amountOfPlayers];
        _players = new GameObject[_amountOfPlayers];

        for (int i = 0; i < _cameras.Length; i++)
        {
            //TODO fix transform.position for each player
            _cameras[i] = Instantiate(_cameraPrefab, transform.position, Quaternion.identity);
        }
    }

    void SetPlayerAmount()
    {
        switch (_amountOfPlayers)
        {
            case 2:
                _cameras = new Camera[2];
                break;
            case 3:
                _cameras = new Camera[3];
                break;
            case 4:
                _cameras = new Camera[4];
                break;
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

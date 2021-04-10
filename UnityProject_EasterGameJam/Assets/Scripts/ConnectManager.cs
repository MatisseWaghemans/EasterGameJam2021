using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectManager : MonoBehaviour
{
	private GameManager _gameManager;
	private void Start()
	{
		_gameManager = FindObjectOfType<GameManager>();
	}

	private void Update()
	{
		if (_gameManager.CanConnect())
		{
			_gameManager.ConnectGamepads();
		}

        if (_gameManager.CanDisconnect())
        {
            _gameManager.DisconnectGamepads();
        }
	}
}

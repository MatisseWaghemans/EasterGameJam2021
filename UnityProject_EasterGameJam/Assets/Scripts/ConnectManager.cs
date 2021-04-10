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
		if (_gameManager.CanConnect() == true)
		{
			_gameManager.ConnectGamepads();
		}
	}
}

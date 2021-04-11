using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EndScreenBehaviour : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _playerPanels = new List<GameObject>();

	[SerializeField]
	private List<Text> _playerScores = new List<Text>();

	[SerializeField]
	private Transform _panelsParent;

	[SerializeField]
	private GameObject _playerPanel;

	private Dictionary<int, int> _playerNumberDictionary;

	private int[] _scores;

	private GameManager _gamemanager;


	// Start is called before the first frame update
	void Start()
	{
		_playerNumberDictionary = new Dictionary<int, int>();
		_gamemanager = FindObjectOfType<GameManager>();

		_scores = _gamemanager.TotalPlayerScores;
		Array.Sort(_scores);


		for (int i = 0; i < _scores.Length; i++)
		{
			var currentPlayer = Instantiate(_playerPanel, _panelsParent);

			//int playerNumber = 0;

			//if (_playerNumberDictionary.ContainsKey(_scores[i]))
			//{
			//	playerNumber = _playerNumberDictionary[playerNumber] + 1;
			//	_playerNumberDictionary[playerNumber] = playerNumber;
			//}
			//else
			//{
			//	_playerNumberDictionary.Add(_scores[i], 1);
			//}

			////break;
			////for (int j = 0; j < _gamemanager.TotalPlayerScores.Length; j++)
			////{
			////	if (_gamemanager.TotalPlayerScores[j] == _scores[i])
			////	{
			////	}
			////}

			currentPlayer.GetComponentsInChildren<Text>()[0].text = "Player " + (/*playerNumber*/i + 1) + " : ";
			currentPlayer.GetComponentsInChildren<Text>()[1].text = "" + _scores[i];


		}

	}
}

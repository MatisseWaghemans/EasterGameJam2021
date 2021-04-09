using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
	[SerializeField]
	private int _playerID; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		CheckControllerInput();
		ShootBall();
    }

	private void CheckControllerInput()
	{
	}

	private void ShootBall()
	{
		throw new NotImplementedException();
	}
}

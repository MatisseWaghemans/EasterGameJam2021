using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class KillFloor : MonoBehaviour
{
	private GolfBallBehaviour _golfBall;

	private void OnTriggerEnter(Collider collision)
	{
		collision.gameObject.TryGetComponent<GolfBallBehaviour>(out _golfBall);

		if(_golfBall != null)
		{
			_golfBall.KillPlayer();
		}

		_golfBall = null;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningBall : MonoBehaviour
{
	[SerializeField]
	private Rigidbody _rigidbody;

	[SerializeField]
	private Transform _startPoint;

	[SerializeField]
	private Transform _endPoint;

	[SerializeField]
	private float _forceMultiplier;

	private Rigidbody collidedRigidbody;

	private Vector3 _force;

	public GameObject soundPlayer;

	private void Start()
	{
		_rigidbody.isKinematic = true;
		DontDestroyOnLoad(soundPlayer);
	}

	public void ShootBall()
	{
		StartCoroutine(AddForce());
	}

	public IEnumerator AddForce()
	{
		yield return new WaitForSeconds(1f);
		_rigidbody.isKinematic = false;


		_force = _endPoint.position - _startPoint.position;

		_rigidbody.AddForce(_force * _forceMultiplier);
	}

	private void OnCollisionEnter(Collision collision)
	{

		collision.gameObject.TryGetComponent<Rigidbody>(out collidedRigidbody);
		if (collidedRigidbody != null)
		{
			collidedRigidbody.isKinematic = false;
		}

		collidedRigidbody = null;
	}
}

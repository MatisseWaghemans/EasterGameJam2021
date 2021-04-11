using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraOverviewBehaviour : MonoBehaviour
{
	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private float _startDelay = 5f;

	[SerializeField]
	private Transform[] _waypoints;

	[SerializeField]
	private Transform _startPoint;

	[SerializeField]
	private float _speed = 5f;

	private int _waypointIndex = 0;

	public UnityEvent Event;

    private LevelManager _levelManager;

	private void Awake()
	{
		_camera.transform.position = _waypoints[0].transform.position;
		_camera.transform.rotation = _waypoints[0].transform.rotation;

        _levelManager = FindObjectOfType<LevelManager>();
    }
	public void StartOverview()
	{
		_camera.enabled = true;
		_waypointIndex = 0;
		StartCoroutine(LerpToWaypoint());
	}

	private IEnumerator LerpToWaypoint()
	{
		if (_waypointIndex == 0)
			yield return new WaitForSeconds(_startDelay);

		_waypointIndex++;
		float time = 0;

		Vector3 startPosition = _camera.transform.position;
		Vector3 targetPosition = _waypoints[_waypointIndex].position;

		Quaternion startRotation = _camera.transform.rotation;
		Quaternion targetRotation = _waypoints[_waypointIndex].rotation;

		while (time < _speed)
		{
			_camera.transform.position = Vector3.Lerp(startPosition, targetPosition, time / _speed);
			_camera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / _speed);
			time += Time.deltaTime;
			yield return null;
		}

		_camera.transform.position = targetPosition;
		_camera.transform.rotation = targetRotation;

		if (_waypointIndex < _waypoints.Length - 1)
			StartCoroutine(LerpToWaypoint());
		else
			StartCoroutine(LerpToStartPoint());
	}

	private IEnumerator LerpToStartPoint()
	{
		float time = 0;

		Vector3 startPosition = _camera.transform.position;
		Vector3 targetPosition = _startPoint.position;

		Quaternion startRotation = _camera.transform.rotation;
		Quaternion targetRotation = _startPoint.rotation;

		while (time < _speed)
		{
			_camera.transform.position = Vector3.Lerp(startPosition, targetPosition, time / _speed);
			_camera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / _speed);
			time += Time.deltaTime;
			yield return null;
		}

		_camera.transform.position = targetPosition;
		_camera.transform.rotation = targetRotation;

		Event.Invoke();

		_camera.enabled = false;
    }
}

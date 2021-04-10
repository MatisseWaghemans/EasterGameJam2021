using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
	[SerializeField] private LayerMask _placeableMask;
	public GameObject[] placeableObjectPrefab;

	private GameObject currentPlaceableObject;
	private float mouseWheelRotation;
	private int currentPrefabIndex = 1;

	private bool _isRaycastValid = false;

	[SerializeField]
	private Transform _controllerPointer;

	public static PlacementController Instance;

	protected void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		if (currentPlaceableObject != null)
		{
			MoveCurrentObjectToPointer();
			RotateFromMouseWheel();
		}

		if (_isRaycastValid == false)
		{
			currentPlaceableObject = null;
		}
	}

	public void HandleNewObjectHotkey()
	{
		for (int i = 0; i < placeableObjectPrefab.Length; i++)
		{
			//if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
			//{
				if (PressedKeyOfCurrentPrefab(i))
				{
					Destroy(currentPlaceableObject);
					currentPrefabIndex = -1;
				}
				else
				{
					if (currentPlaceableObject != null)
					{
						Destroy(currentPlaceableObject);
					}
					currentPlaceableObject = Instantiate(placeableObjectPrefab[i]);
					currentPrefabIndex = i;
				}
				break;
			//}
		}
	}

	private bool PressedKeyOfCurrentPrefab(int i)
	{
		return currentPlaceableObject != null && currentPrefabIndex == i;
	}

	private void MoveCurrentObjectToPointer()
	{
		if (Physics.Raycast(_controllerPointer.position, -Vector3.up, out var hitInfo, _placeableMask))
		{
			currentPlaceableObject.transform.position = hitInfo.point;
			currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
			_isRaycastValid = true;
		}
		else
		{
			_isRaycastValid = false;
		}
	}

	private void RotateFromMouseWheel()
	{
		//Debug.Log(Input.mouseScrollDelta);
		mouseWheelRotation += Input.mouseScrollDelta.y;
		currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
	}

	public void ReleaseObject()
	{

		currentPlaceableObject = null;

	}
}

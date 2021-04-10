using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonHighlight : MonoBehaviour, ISelectHandler
{
	[SerializeField]
	private Material _selectedMat;

	[SerializeField]
	private Material _unselectedMat;

	[SerializeField]
	private Renderer _renderer;

	public bool Selected = false;

	[SerializeField]
	private ButtonHighlight _buttonHighlight;

	public void OnSelect(BaseEventData eventData)
	{
		Selected = !Selected;
		_buttonHighlight.Selected = !Selected;
	}

	private void Update()
	{
		if (Selected)
		{
			_renderer.material = _selectedMat;
		}
		else
		{
			_renderer.material = _unselectedMat;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
		_animator.speed = Random.Range(0.1f, 1f);
	}

}

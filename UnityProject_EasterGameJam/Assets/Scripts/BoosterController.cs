using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterController : MonoBehaviour
{
    [Header("Proporties")]
    [SerializeField] private float _boosterSpeed = 5;

    [Header("Components")]
    private Transform _transform;

    private void Start()
    {
        _transform = this.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            other.GetComponent<Rigidbody>().AddForce(_transform.forward * _boosterSpeed);
        }
    }
}

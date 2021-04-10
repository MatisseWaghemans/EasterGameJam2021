using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMillRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 2f;
    void Update()
    {
         transform.RotateAround(transform.position, transform.right, Time.deltaTime * 90f);
    }
}

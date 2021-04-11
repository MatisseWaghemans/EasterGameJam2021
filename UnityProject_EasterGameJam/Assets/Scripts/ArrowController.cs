using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Proporties")]
    [Tooltip("1 to 5")] [Range(1,5)] public float Scale = 1;

    [Header("Components")]
    private Transform _transform;

    private void Start()
    {
        _transform = this.transform;
    }

    private void Update()
    {
        _transform.localScale = new Vector3(Scale, 1 / Scale, 1 / Scale);
    }
}

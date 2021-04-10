using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayScriptV2 : MonoBehaviour
    {
    private Vector3 SwayScale = Vector3.one;
    [Header("Position")]
    [SerializeField] private Vector3 _positionAmplitude;
    [SerializeField] private Vector3 _positionFrequency;
    [Header("Rotation")]
    [SerializeField] private Vector3 _rotAmplitude;
    [SerializeField] private Vector3 _rotFrequency;
    [Header("Scale")]
    [SerializeField] private Vector3 _scalingAmplitude;
    [SerializeField] private Vector3 _scalingFrequency;
    [Header("Offset")]
    [SerializeField] private bool _randomOffset = true;
    [SerializeField] private float _offset;

    public float Scale { get; set; }
    public float RotScale { get; set; }

    private Vector3 _startPosition;
    private Vector3 _startRotation;
    private Vector3 _startScale;
    
    void Start()
    {
        Scale = 1;
        RotScale = 1;
        _startPosition = transform.localPosition;
        _startRotation = transform.localEulerAngles;
        _startScale = transform.localScale;

        if (_randomOffset)
            _offset = Random.Range(0, 5) * 10;
    }
    
    void Update()
    {
        float offsetFramecount = Time.time + _offset;

        //position
        transform.localPosition = _startPosition + new Vector3(
            _positionAmplitude.x * SwayScale.x * Mathf.Sin(offsetFramecount * _positionFrequency.x * SwayScale.x),
            _positionAmplitude.y * SwayScale.y * Mathf.Sin(offsetFramecount * _positionFrequency.y * SwayScale.y),
            _positionAmplitude.z * SwayScale.z * Mathf.Sin(offsetFramecount * _positionFrequency.z * SwayScale.z)
            ) * Scale;
        //roattion
        transform.localEulerAngles = _startRotation + new Vector3(
            _rotAmplitude.x * SwayScale.x * Mathf.Sin(offsetFramecount * _rotFrequency.x * SwayScale.x),
            _rotAmplitude.y * SwayScale.y * Mathf.Sin(offsetFramecount * _rotFrequency.y * SwayScale.y),
            _rotAmplitude.z * SwayScale.z * Mathf.Sin(offsetFramecount * _rotFrequency.z * SwayScale.z))
            * RotScale;
        //scale
        transform.localScale = _startScale + new Vector3(
            _scalingAmplitude.x * SwayScale.x * Mathf.Sin(offsetFramecount * _scalingFrequency.x * SwayScale.x),
            _scalingAmplitude.y * SwayScale.y * Mathf.Sin(offsetFramecount * _scalingFrequency.y * SwayScale.y),
            _scalingAmplitude.z * SwayScale.z * Mathf.Sin(offsetFramecount * _scalingFrequency.z * SwayScale.z));
    }

    public void SetSway(bool doSway)
    {
        if (doSway)
            this.enabled = true;
        else
            this.enabled = false;
    }

    public void SetSway(bool doSway, Vector3 swayScale)
    {
        if (doSway)
            this.enabled = true;
        else
            this.enabled = false;

        SwayScale = swayScale;
    }

    public void SetSway(
        bool doSway,
        Vector3 swayScale,
        Vector3 positionAmplitude, Vector3 positionFrequency,
        Vector3 rotAmplitude, Vector3 rotFrequency,
        Vector3 scalingAmplitude,  Vector3 scalingFrequency
        )
    {
        if (doSway)
            this.enabled = true;
        else
            this.enabled = false;
        
        SwayScale = swayScale;
        _positionAmplitude = positionAmplitude;
        _positionFrequency = positionFrequency;
        _rotAmplitude = rotAmplitude;
        _rotFrequency = rotFrequency;
        _scalingAmplitude = scalingAmplitude;
        _scalingFrequency = scalingFrequency;
    }
}

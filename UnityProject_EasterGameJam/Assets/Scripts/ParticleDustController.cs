using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDustController : MonoBehaviour
{
    [Header("Proporties")]
    [SerializeField] bool _activate = false;
    [SerializeField] float _burst = 50;

    [Header("Components")]
   private ParticleSystem _ps;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_activate)
        {
            _ps.emission.SetBursts(new[]
                                {
                                    new ParticleSystem.Burst(0f, _burst), //float_time, short_count
                                });

            _ps.Play();
            _activate = false;
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSmoke : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float grey_smoke_threshold;
    [SerializeField] float black_smoke_threshold;

    [Header("References")]
    [SerializeField] ParticleSystem grey_smoke;
    [SerializeField] ParticleSystem black_smoke;

    private float last_percentage = 1;


    public void UpdateSmoke(float _new_health_percentage)
    {
        if (last_percentage > grey_smoke_threshold && _new_health_percentage <= grey_smoke_threshold &&
            !grey_smoke.isPlaying)
        {
            grey_smoke.Play();
            black_smoke.Stop();
        }

        if (last_percentage <= grey_smoke_threshold && _new_health_percentage > grey_smoke_threshold &&
            grey_smoke.isPlaying)
        {
            grey_smoke.Stop();
        }

        if (last_percentage > black_smoke_threshold && _new_health_percentage <= black_smoke_threshold &&
            !black_smoke.isPlaying)
        {
            grey_smoke.Stop();
            black_smoke.Play();
        }

        if (last_percentage <= black_smoke_threshold && _new_health_percentage > black_smoke_threshold &&
            black_smoke.isPlaying)
        {
            grey_smoke.Play();
            black_smoke.Stop();
        }

        if (_new_health_percentage <= 0)
        {
            grey_smoke.Stop();
            black_smoke.Stop();
        }

        last_percentage = _new_health_percentage;
    }

}

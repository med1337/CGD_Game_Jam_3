using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControl : Controllable
{
    [Header("Parameters")]
    [SerializeField] float reticle_move_speed;
    [SerializeField] float steer_speed;

    [Space]
    [SerializeField] float shot_delay;
    [SerializeField] float shot_spread;
    [SerializeField] GameObject shot_prefab;

    [Header("References")]
    [SerializeField] GameObject turret;
    [SerializeField] GameObject reticle;
    [SerializeField] Transform shoot_point;
    [SerializeField] Transform ejection_point;

    private float last_shot_timestamp;


    public override void Move(Vector3 _dir)
    {
        Vector3 move = _dir * reticle_move_speed * Time.deltaTime;
        reticle.transform.position += move;
    }


    public override void OnControlStart(PlayerControl _player)
    {
        reticle.SetActive(true);
    }


    public override void OnControlEnd()
    {
        reticle.SetActive(false);
    }


    public override void Activate()
    {
        Debug.Log("Turret Activate");
    }


    void Start()
    {

    }


    void Update()
    {
        //float angle = Vector3.Angle(transform.up, reticle.transform.position);

        Vector3 new_forward = (reticle.transform.position - turret.transform.position).normalized;
        turret.transform.forward = Vector3.Slerp(turret.transform.forward, new_forward, steer_speed * Time.deltaTime);
    }

}

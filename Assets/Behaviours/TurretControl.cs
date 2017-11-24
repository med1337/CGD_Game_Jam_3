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

    [Space]
    [SerializeField] GameObject particle_prefab;

    [Header("References")]
    [SerializeField] GameObject turret;
    [SerializeField] Transform shoot_point;
    [SerializeField] Transform ejection_point;

    private Vector3 reticule_pos;
    private Vector3 reticule_offset;
    private GameObject reticule_vis;
    private float last_shot_timestamp;


    public override void Move(Vector3 _dir)
    {
        Vector3 move = _dir * reticle_move_speed * Time.deltaTime;
        reticule_offset += move;
    }


    public override void OnControlStart(PlayerControl _player)
    {
        base.OnControlStart(_player);

        reticule_vis = GameObject.CreatePrimitive(PrimitiveType.Cube);
        reticule_vis.transform.position = reticule_pos;
        reticule_vis.transform.localScale = Vector3.one * 0.25f;
        reticule_vis.GetComponent<MeshRenderer>().material.color = Color.red;
        Destroy(reticule_vis.GetComponent<BoxCollider>());
    }


    public override void OnControlEnd()
    {
        base.OnControlEnd();

        Destroy(reticule_vis);
        ResetReticulePos();
    }


    public override void Activate()
    {
        if (Time.time >= last_shot_timestamp + shot_delay)
        {
            last_shot_timestamp = Time.time;
            Shoot();
        }
    }


    void Shoot()
    {
        Vector3 shot_forward = shoot_point.forward;
        Vector3 variance = new Vector3(
            Random.Range(-shot_spread, shot_spread),
            Random.Range(-shot_spread, shot_spread),
            Random.Range(-shot_spread, shot_spread));
        shot_forward += variance;

        GameObject shot_clone = Instantiate(shot_prefab, shoot_point.position,
            Quaternion.LookRotation(shot_forward));
        TurretShot shot = shot_clone.GetComponent<TurretShot>();

        GameObject particle_clone = Instantiate(particle_prefab, shoot_point.position,
            Quaternion.LookRotation(shot_forward));
    }


    void Start()
    {
        ResetReticulePos();
    }

    
    void ResetReticulePos()
    {
        reticule_offset = turret.transform.forward;
    }


    void Update()
    {
        if (!being_controlled)
            return;

        // TODO: constrain aiming angle..
        //float angle = Vector3.Angle(transform.up, reticle.transform.position);

        Vector3 new_forward = (reticule_pos - turret.transform.position).normalized;
        turret.transform.forward = Vector3.Slerp(turret.transform.forward, new_forward, steer_speed * Time.deltaTime);

        reticule_pos = turret.transform.position + reticule_offset;

        if (reticule_vis != null)
            reticule_vis.transform.position = reticule_pos;
    }

}

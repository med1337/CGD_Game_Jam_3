using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeRingControl : Controllable
{
    [Header("Parameters")]
    [SerializeField] float steer_speed;
    [SerializeField] float max_steer_angle;

    [Space]
    [SerializeField] float reticule_move_speed;
    [SerializeField] float reticule_min_distance;
    [SerializeField] float reticule_max_distance;

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
    private Vector3 target_forward;


    public override void Move(Vector3 _dir)
    {
        move_dir = _dir;
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
    }


    public override void Activate()
    {
        
    }


    public override void Stop()
    {

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

    }


    void Update()
    {
        if (being_controlled)
            ControlUpdate();
    }


    void ControlUpdate()
    {
        reticule_offset = move_dir * reticule_max_distance;
        reticule_pos = turret.transform.position + reticule_offset;
        reticule_pos += turret.transform.forward * reticule_min_distance;

        if (reticule_vis != null)
            reticule_vis.transform.position = reticule_pos;

        Vector3 new_forward = (reticule_pos - turret.transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, new_forward);

        if (angle <= max_steer_angle)
        {
            target_forward = new_forward;
        }

        Vector3 slerp = Vector3.Slerp(turret.transform.forward, target_forward, steer_speed * Time.deltaTime);
        turret.transform.forward = slerp;
    }

}

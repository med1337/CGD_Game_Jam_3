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
    [SerializeField] float shoot_strength;
    [SerializeField] CurvedLineRenderer rope_main;
    [SerializeField] Rigidbody rope_mid;
    [SerializeField] Transform rope_end;

    [Space]
    [SerializeField] GameObject life_ring_prefab;
    [SerializeField] float max_ring_distance;

    [Header("References")]
    [SerializeField] GameObject turret;
    [SerializeField] Transform shoot_point;
    [SerializeField] Transform ejection_point;
    [SerializeField] Transform trajectory_end;

    private Vector3 reticule_pos;
    private Vector3 reticule_offset;
    private GameObject reticule_vis;
    private Vector3 target_forward;

    Vector3 left_max;
    Vector3 right_max;

    private bool life_ring_out { get { return life_ring != null; } }
    private bool can_activate = true;
    private LifeRing life_ring;


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


    // Called as long as the controlling player holds the Attack button.
    public override void Activate()
    {
        if (!life_ring_out && can_activate)
        {
            can_activate = false;

            ThrowRing();
        }
        else if (life_ring_out && can_activate)
        {
            can_activate = false;

            RetractRing();
        }
    }


    // Called when the controlling player releases the Attack button.
    public override void Stop()
    {
        can_activate = true;
    }


    void ThrowRing()
    {
        Vector3 shot_forward = shoot_point.forward;

        GameObject clone = Instantiate(life_ring_prefab, shoot_point.position + shot_forward, Quaternion.identity);
        life_ring = clone.GetComponent<LifeRing>();
        life_ring.rigid_body.AddForce(shot_forward * (shoot_strength * move_dir.magnitude), ForceMode.Impulse);
        life_ring.trajectory_end_ref = trajectory_end;

        rope_mid.gameObject.SetActive(true);
        rope_end.gameObject.SetActive(true);

        rope_mid.transform.position = shoot_point.position;
        rope_mid.AddForce(shot_forward * (shoot_strength / 2), ForceMode.Impulse);

        AudioManager.PlayOneShot("throwing");
    }


    void RetractRing()
    {
        Destroy(life_ring.gameObject);

        rope_mid.gameObject.SetActive(false);
        rope_end.gameObject.SetActive(false);

        rope_main.UpdatePoints();
    }


    void Start()
    {

    }


    void Update()
    {
        left_max = Quaternion.Euler(0, -max_steer_angle, 0) * transform.forward;
        right_max = Quaternion.Euler(0, max_steer_angle, 0) * transform.forward;

        if (life_ring != null)
        {
            rope_end.position = life_ring.transform.position;
            rope_main.UpdatePoints();
        }

        if (being_controlled)
            ControlUpdate();
    }


    void FixedUpdate()
    {
        if (life_ring == null)
            return;

        // Keep the life ring in range.
        if (Vector3.Distance(life_ring.transform.position, transform.position) > max_ring_distance)
        {
            Vector3 diff = (transform.position - life_ring.transform.position);
            life_ring.rigid_body.MovePosition(life_ring.transform.position + diff * Time.fixedDeltaTime);
        }

        // Keep the mid rope point in range.
        if (Vector3.Distance(rope_mid.transform.position, transform.position) > max_ring_distance / 2)
        {
            Vector3 diff = (transform.position - rope_mid.transform.position);
            rope_mid.MovePosition(rope_mid.transform.position + diff * Time.fixedDeltaTime);
        }
    }


    void ControlUpdate()
    {
        reticule_offset = move_dir * reticule_max_distance;
        reticule_pos = transform.position + reticule_offset;
        reticule_pos += transform.forward * reticule_min_distance;

        if (reticule_vis != null)
            reticule_vis.transform.position = reticule_pos;

        Vector3 new_forward = (reticule_pos - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, new_forward);

        if (angle <= max_steer_angle)
        {
            target_forward = new_forward;
        }
        else
        {
            float l_angle = Vector3.Angle(new_forward, left_max);
            float r_angle = Vector3.Angle(new_forward, right_max);

            if (l_angle > r_angle)
            {
                target_forward = right_max;
            }
            else
            {
                target_forward = left_max;
            }
        }

        Vector3 slerp = Vector3.Slerp(turret.transform.forward, target_forward, steer_speed * Time.deltaTime);
        turret.transform.forward = slerp;
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, left_max * 10);
        Gizmos.DrawRay(transform.position, right_max * 10);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, target_forward * 10);
    }

}

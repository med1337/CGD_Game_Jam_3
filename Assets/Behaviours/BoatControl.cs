using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControl : Controllable
{
    [Header("Parameters")]
    [SerializeField] float max_speed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float steer_speed;
    [SerializeField] float max_steer_speed;
    [SerializeField] List<Station> stations = new List<Station>();

    private float acc;


    public override void Move(Vector3 _dir)
    {
        move_dir = _dir;
    }


    public override void OnControlStart(PlayerControl _player)
    {
        
    }


    public override void OnControlEnd()
    {

    }


    void Update()
    {
        Vector3 target = transform.position + move_dir;
        Vector3 direction = target - transform.position;

        if (move_dir.sqrMagnitude > 0)
        {

            steer_speed += 0.005f;
            if (steer_speed > max_steer_speed)
            {
                steer_speed = max_steer_speed;
            }

            acc += acceleration * Time.deltaTime;

            Quaternion new_rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, new_rot, steer_speed  * Time.deltaTime);
        }
        else
        {
            steer_speed -= 0.2f;
            if (steer_speed < 0)
            {
                steer_speed = 0.0f;
            }

            acc -= deceleration * Time.deltaTime;
            acc = Mathf.Clamp(acc, 0, Mathf.Infinity);
        }

        Vector3 move = transform.forward * acc * Time.deltaTime;
        move = Vector3.ClampMagnitude(move, max_speed);
        transform.position += move;

        move_dir = Vector3.zero;
    }

}

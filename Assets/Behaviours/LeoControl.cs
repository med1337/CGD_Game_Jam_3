using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeoControl : Controllable
{
    [Header("Parameters")]
    [SerializeField]float motorPower;
    float acceleration;
    float deceleration;
    [SerializeField] float steer_speed;
    [SerializeField] float max_steer_speed;
    [SerializeField] List<Station> stations = new List<Station>();

    public GameObject hull;
    public List<WheelCollider> colliders;


    [Space] [Header("Engine Audio")]
    [SerializeField] private AudioSource engine_audio_source;
    [SerializeField] private float pitch_to_speed_reduction = 8;
    [SerializeField] private float minimum_pitch = 0.9f;
    [SerializeField] private float maximum_pitch = 1.9f;


    public override void Move(Vector3 _dir)
    {
        move_dir = _dir;
    }


    public override void Accelerate(Vector2 _acc)
    {
        acceleration = _acc.x;
        deceleration = _acc.y*0.25f;
    }


    public override void OnControlStart(PlayerControl _player)
    {
        base.OnControlStart(_player);
    }


    public override void OnControlEnd()
    {
        base.OnControlEnd();

        move_dir = Vector3.zero;

        acceleration = 0;
        deceleration = 0;
    }


    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        engine_audio_source.pitch = Remap(Mathf.Abs(colliders[3].motorTorque), 0, motorPower, minimum_pitch,
            maximum_pitch);
    }

    private void FixedUpdate()
    {
        //if(!colliders[0].isGrounded && !colliders[2].isGrounded || !colliders[1].isGrounded && !colliders[3].isGrounded)
        //{
        //    colliders[0].transform.parent.gameObject.SetActive(false);
        //    hull.SetActive(true);
        //}

        for (int i = 0; i < colliders.Count; ++i)
        {
            if (i < 2)
            {

                colliders[i].steerAngle = move_dir.x * steer_speed/2;
            }
            else
            {
                colliders[i].steerAngle = -move_dir.x * steer_speed;
                colliders[i].motorTorque = acceleration * motorPower;
                colliders[i].motorTorque += deceleration * -motorPower;
            }
        }


        if (transform.position.sqrMagnitude > GameManager.map_bound_radius * GameManager.map_bound_radius)
        {
            transform.position = transform.position.normalized * GameManager.map_bound_radius;
        }
    }


    float Remap(float currentValue, float minimumOne, float maximumOne, float minimumTwo, float maximumTwo)
    {
        return ((currentValue - minimumOne) / (maximumOne - minimumOne) * (maximumTwo - minimumTwo)) + minimumTwo;
    }
}

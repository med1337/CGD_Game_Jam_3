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
    [SerializeField] float decceleration_multiplier = 1;
    [SerializeField] Vector3 centre_of_mass = Vector3.up;

    public bool GoSelf = false;
    public bool SpawnCargo = false;
    public GameObject cargo;
    public float angle;
    public GameObject hull;
    public List<WheelCollider> colliders;


    [Space] [Header("Engine Audio")]
    [SerializeField] private AudioSource engine_audio_source;
    [SerializeField] private float pitch_to_speed_reduction = 8;
    [SerializeField] private float minimum_pitch = 0.9f;
    [SerializeField] private float maximum_pitch = 1.9f;


    public void CleanUpDeadBoat()
    {
        Destroy(this.gameObject, 10);
    }


    public override void Move(Vector3 _dir)
    {
        move_dir = _dir;
    }


    public override void Accelerate(Vector2 _acc)
    {
        acceleration = _acc.x;
        deceleration = _acc.y* decceleration_multiplier;
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
        GetComponent<Rigidbody>().centerOfMass = centre_of_mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnCargo)
        {
            var SpawnPos = new Vector3(0,8,0);
            SpawnPos += transform.position;
            GameObject.Instantiate(cargo, SpawnPos, Quaternion.identity);
            SpawnCargo = false;
        }
        if (engine_audio_source == null)
            return;

        engine_audio_source.pitch = Remap(Mathf.Abs(colliders[3].motorTorque), 0, motorPower, minimum_pitch,
            maximum_pitch);
    }

    private void FixedUpdate()
    {
        if(GoSelf)
        {
            angle = Vector3.Angle(transform.right, Vector3.up) - 90;
            move_dir.x = angle;
            acceleration = 1;
        }
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


    private void LateUpdate()
    {
        if (GoSelf)
            transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y,
                transform.rotation.eulerAngles.z);
    }

    float Remap(float currentValue, float minimumOne, float maximumOne, float minimumTwo, float maximumTwo)
    {
        return ((currentValue - minimumOne) / (maximumOne - minimumOne) * (maximumTwo - minimumTwo)) + minimumTwo;
    }
}

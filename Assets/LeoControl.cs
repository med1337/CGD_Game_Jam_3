﻿using System.Collections;
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



    public override void Move(Vector3 _dir, Vector2 accVector2)
    {
        move_dir = _dir;
        acceleration = accVector2.x;
        deceleration = accVector2.y;
    }


    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {

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

                colliders[i].steerAngle = move_dir.x * steer_speed;
            }
            else
            {
                colliders[i].steerAngle = -move_dir.x * steer_speed;
                colliders[i].motorTorque = acceleration * motorPower;
                colliders[i].motorTorque += deceleration * -motorPower;
            }
        }
    }
}

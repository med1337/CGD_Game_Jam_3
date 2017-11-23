﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControl : Controllable
{
    public Player input;

    [Header("Parameters")]
    [SerializeField] float move_speed;
    [SerializeField] Vector3 mount_scan_offset;
    [SerializeField] float mount_scan_radius;
    [SerializeField] float mount_scan_delay;

    [SerializeField] LayerMask station_layer;
    [SerializeField] LayerMask pickup_layer;

    [Header("References")]
    [SerializeField] Rigidbody rigid_body;

    private Station current_station;
    private Station nearest_station;
    private float last_scan_timestamp;

    private Pickup current_pickup;
    private Pickup nearest_pickup;


    public bool IsUsingStation()
    {
        return current_station != null;
    }


    public bool IsLifting()
    {
        return current_pickup != null;
    }


    public override void Move(Vector3 _dir)
    {
        if (IsUsingStation())
        {
            current_station.controllable.Move(_dir);
        }
        else
        {
            move_dir = _dir;
        }
    }


    void Start()
    {

    }


    void Update()
    {
        float horizontal = input.GetAxis("Horizontal");
        float vertical = input.GetAxis("Vertical");

        if (input.GetButtonDown("Interact"))
            Interact();

        if (!IsUsingStation())
        {
            Move(new Vector3(horizontal, 0, vertical));

            if (input.GetButtonDown("Attack"))
                Attack();

            if (Time.time >= last_scan_timestamp + mount_scan_delay)
            {
                last_scan_timestamp = Time.time;

                if (!IsLifting())
                {
                    MountScan();
                    PickUpScan();
                }
            }
        }
        else // Stuff to do with controlling a station ..
        {
            current_station.controllable.Move(new Vector3(horizontal, 0, vertical));

            if (input.GetButton("Attack"))
                current_station.controllable.Activate();
        }
    }


    void FixedUpdate()
    {
        if (!IsUsingStation())
        {
            Vector3 move = move_dir * move_speed * Time.deltaTime;
            rigid_body.MovePosition(transform.position + move);

            move_dir = Vector3.zero;
        }
    }


    void MountScan()
    {
        var colliders = Physics.OverlapSphere(transform.position + mount_scan_offset, mount_scan_radius, station_layer);
        foreach (var collider in colliders)
        {
            var mount = collider.GetComponent<Station>();
            if (mount == null || mount.occupied)
                continue;

            nearest_station = mount;
            return;
        }

        nearest_station = null;
    }


    void PickUpScan()
    {
        var colliders = Physics.OverlapSphere(transform.position + mount_scan_offset, mount_scan_radius, pickup_layer);
        foreach (var collider in colliders)
        {
            var pickup = collider.GetComponent<Pickup>();

            if (pickup == null || pickup.GetInUse())
                continue;

            nearest_pickup = pickup;
            return;
        }

        nearest_pickup = null;
    }

        
    void Interact()
    {
        Debug.Log("Interact");

        if (!IsUsingStation() && nearest_station != null)
        {
            // Occupy station.
            current_station = nearest_station;
            current_station.controllable.OnControlStart(this);

            transform.position = current_station.transform.position;
            rigid_body.isKinematic = true;

            current_station.occupied = true;
            nearest_station = null;
        }
        else if (IsUsingStation())
        {
            // Leave station.
            current_station.controllable.OnControlEnd();
            current_station.occupied = false;
            current_station = null;
            rigid_body.isKinematic = false;
        }
        else if(!IsLifting() && nearest_pickup != null)
        {
            // Carry thing.
        }
        else if (IsLifting())
        {
            // Throw/drop.
        }
    }


    void Attack()
    {
        // Player's personal attack.
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + mount_scan_offset, mount_scan_radius);
    }


    void OnTriggerStay(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            transform.SetParent(_other.transform);
        }
    }


    void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            transform.SetParent(null);
        }
    }

}

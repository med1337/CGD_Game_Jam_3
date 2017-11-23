using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Controllable
{
    [Header("Parameters")]
    [SerializeField] float move_speed;
    [SerializeField] Vector3 mount_scan_offset;
    [SerializeField] float mount_scan_radius;
    [SerializeField] float mount_scan_delay;

    [Header("References")]
    [SerializeField] Rigidbody rigid_body;

    private Station current_station;
    private Station nearest_station;
    private float last_scan_timestamp;


    public bool IsUsingStation()
    {
        return current_station != null;
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

    
    public void UseStation()
    {
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
            current_station.occupied = false;
            current_station = null;
            rigid_body.isKinematic = false;
        }
    }


    void Start()
    {

    }


    void Update()
    {
        if (!IsUsingStation() && Time.time >= last_scan_timestamp + mount_scan_delay)
        {
            last_scan_timestamp = Time.time;
            MountScan();
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
        var colliders = Physics.OverlapSphere(transform.position + mount_scan_offset, mount_scan_radius);
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

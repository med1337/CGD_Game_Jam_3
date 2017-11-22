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

    private Mount mounted_mount;
    private Mount nearest_mount;
    private float last_scan_timestamp;


    public bool IsMounted()
    {
        return mounted_mount != null;
    }


    public override void Move(Vector3 _dir)
    {
        if (IsMounted())
        {
            mounted_mount.controllable.Move(_dir);
        }
        else
        {
            move_dir = _dir;
        }
    }

    
    public override void OnInteract()
    {
        if (!IsMounted() && nearest_mount != null)
        {
            mounted_mount = nearest_mount;
            transform.position = mounted_mount.transform.position;

            mounted_mount.occupied = true;
            nearest_mount = null;
        }
        else if (IsMounted())
        {
            mounted_mount.occupied = false;
            mounted_mount = null;
        }
    }


    void Start()
    {

    }


    void Update()
    {
        if (!IsMounted() && Time.time >= last_scan_timestamp + mount_scan_delay)
        {
            last_scan_timestamp = Time.time;
            MountScan();
        }
    }


    void FixedUpdate()
    {
        if (!IsMounted())
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
            var mount = collider.GetComponent<Mount>();
            if (mount == null || mount.occupied)
                continue;

            nearest_mount = mount;
            return;
        }

        nearest_mount = null;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + mount_scan_offset, mount_scan_radius);
    }


    void OnCollisionEnter(Collision _other)
    {
        if (_other.collider.CompareTag("Deck"))
        {
            transform.SetParent(_other.collider.transform);
        }
    }


    void OnCollisionExit(Collision _other)
    {
        if (_other.collider.CompareTag("Deck"))
        {
            transform.SetParent(null);
        }
    }

}

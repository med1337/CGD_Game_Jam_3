using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControl : Controllable
{
    public Player input;

    [Header("Parameters")]
    [SerializeField]
    float move_speed;
    [SerializeField]
    float water_move_modifier;
    [SerializeField]
    Vector3 mount_scan_offset;
    [SerializeField]
    float mount_scan_radius;
    [SerializeField]
    float mount_scan_delay;

    [SerializeField]
    LayerMask station_layer;
    [SerializeField]
    LayerMask pickup_layer;

    [SerializeField]
    float throw_power;

    [Header("References")]
    public Rigidbody rigid_body;
    [SerializeField]
    GameObject smoke_puff_prefab;
    [SerializeField]
    GameObject punch_particle_prefab;
    [SerializeField]
    BuoyantObject buoyant_obj;
    private GameObject current_deck = null;

    private Station current_station;
    private Station nearest_station;
    private float last_scan_timestamp;

    private Pickup current_pickup;
    private Pickup nearest_pickup;

    public Transform carry_position;
    public Transform drop_position;


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
            if (_dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(_dir);

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

        if (buoyant_obj.in_water)
        {
            horizontal *= water_move_modifier;
            vertical *= water_move_modifier;
        }

        float acceleration = input.GetAxis("Forward");
        float decceleration = input.GetAxis("Backward");

        if (input.GetButtonDown("Interact"))
            Interact();

        if (!IsUsingStation())
        {
            Move(new Vector3(horizontal, 0, vertical));

            if (input.GetButtonDown("Attack"))
                Attack();

            if (input.GetButtonDown("Jump"))
                Jump();

            if (Time.time >= last_scan_timestamp + mount_scan_delay)
            {
                last_scan_timestamp = Time.time;

                if (!IsLifting() && !IsUsingStation())
                {
                    MountScan();
                    PickUpScan();
                }
            }
        }
        else // Stuff to do with controlling a station ..
        {
            transform.position = current_station.transform.position;
            transform.rotation = current_station.transform.rotation;

            current_station.controllable.Move(new Vector3(horizontal, 0, vertical));
            current_station.controllable.Accelerate(new Vector2(acceleration, decceleration));

            if (input.GetButton("Attack"))
                current_station.controllable.Activate();

            if (input.GetButtonUp("Attack"))
                current_station.controllable.Stop();
        }
    }


    void FixedUpdate()
    {
        if (!IsUsingStation())
        {
            Vector3 move = move_dir * move_speed * Time.deltaTime;
            rigid_body.MovePosition(transform.position + move);

            //move_dir = Vector3.zero;
        }
    }


    void MountScan()
    {
        var colliders = Physics.OverlapSphere(drop_position.position, mount_scan_radius, station_layer);
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
        var colliders = Physics.OverlapSphere(drop_position.position, mount_scan_radius, pickup_layer);
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
        //If the player is NOT on a station or Carrying AND has a nearest station and pickup
        if (!IsUsingStation() && nearest_station != null && !IsLifting() && nearest_pickup != null)
        {
            // If station is closer mount it
            if (Vector3.Distance(drop_position.position, nearest_station.transform.position) <
                Vector3.Distance(drop_position.position, nearest_pickup.transform.position))
            {
                OccupyStation();
            }

            else
                CarryItem();
        }

        else if (!IsUsingStation() && nearest_station != null && !IsLifting())
        {
            OccupyStation();
        }
        else if (IsUsingStation())
        {
            LeaveStation();
        }
        else if (!IsLifting() && nearest_pickup != null && !IsUsingStation())
        {
            CarryItem();
        }
        else if (IsLifting())
        {
            ThrowItem();
        }
    }


    void OccupyStation()
    {
        if (IsUsingStation() || IsLifting() ||
            nearest_station == null)
        {
            return;
        }

        current_station = nearest_station;
        current_station.controllable.OnControlStart(this);

        transform.position = current_station.transform.position;

        current_station.occupied = true;
        nearest_station = null;
    }


    void LeaveStation()
    {
        if (!IsUsingStation() || IsLifting())
        {
            return;
        }

        // Leave station.
        current_station.controllable.OnControlEnd();
        current_station.occupied = false;
        current_station = null;
    }


    void CarryItem()
    {
        if (IsUsingStation() || IsLifting() ||
            nearest_pickup == null)
        {
            return;
        }

        // Carry thing.
        nearest_pickup.transform.position = carry_position.position;
        nearest_pickup.transform.rotation = carry_position.rotation;

        // Disable Components.
        nearest_pickup.GetComponent<Rigidbody>().isKinematic = true;
        nearest_pickup.GetComponent<Collider>().enabled = false;

        // Set player as parent of Pickup.
        nearest_pickup.gameObject.GetComponent<Transform>().parent = (this.transform);
        current_pickup = nearest_pickup;
        nearest_pickup = null;
    }


    void ThrowItem()
    {
        if (IsUsingStation() || !IsLifting())
        {
            return;
        }

        // Throw/drop.
        current_pickup.transform.position = drop_position.position;
        current_pickup.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        // Enable disabled components.
        current_pickup.GetComponent<Rigidbody>().isKinematic = false;
        current_pickup.GetComponent<Collider>().enabled = true;

        // If player isnt moving.... place the object.
        if (move_dir == Vector3.zero)
        {
            current_pickup.GetComponent<Rigidbody>().AddForce(transform.forward + transform.up);
            Debug.Log("Placing Object");
        }
        // if the player is moving throw the object.
        else
        {
            current_pickup.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * throw_power);
            Debug.Log("Throw Object");
        }



        // If the player is currently parented to a boat ect
        if (transform.parent != null)
        {
            // Set pickup's parent to that of the player...
            current_pickup.transform.parent = transform.parent;
        }
        else
        {
            // else remove the parentof the pickup
            current_pickup.transform.parent = null;
        }

        current_pickup = null;
    }


    void Attack()
    {
        //if not lifting
        if (!IsLifting())
        {
            // Player's personal attack.
            float hit_force = 5.0f;

            //set positions for raycast and punch particle effect
            Vector3 ray_spawn_position = transform.position;
            ray_spawn_position.y += 1;

            Vector3 punch_spawn_pos = transform.position;
            punch_spawn_pos.y += 0.8f;
            punch_spawn_pos += transform.forward * 0.35f;

            Instantiate(punch_particle_prefab, punch_spawn_pos, transform.rotation);

            //check in front of player
            RaycastHit hit;
            if (Physics.SphereCast(ray_spawn_position, 0.5f, transform.forward, out hit, 0.5f))
            {
                if (hit.rigidbody)
                {
                    //if on a deck, don't punch the deck's rigidbody
                    if (current_deck)
                    {
                        if (current_deck.GetComponent<Rigidbody>() != hit.rigidbody)
                        {
                            hit.rigidbody.AddForce(transform.forward * hit_force, ForceMode.VelocityChange);
                        }
                    }

                    //otherwise smack it 
                    else
                    {
                        hit.rigidbody.AddForce(transform.forward * hit_force, ForceMode.VelocityChange);
                    }
                }
            }
        }
    }


    void Jump()
    {
        rigid_body.AddForce(Vector3.up * 7, ForceMode.Impulse);
        Instantiate(smoke_puff_prefab, transform.position + new Vector3(0, 0.33f), Quaternion.identity);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(drop_position.position, mount_scan_radius);
    }


    void OnTriggerStay(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            if (current_deck != _other.transform.parent.gameObject)
            {
                current_deck = _other.transform.parent.gameObject;
            }

            transform.SetParent(_other.transform);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Roof")
        {
            other.GetComponent<HideableRoof>().Hide();
        }
    }

    void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            current_deck = null;
            transform.SetParent(null);
        }
        if (_other.tag == "Roof")
        {
            _other.GetComponent<HideableRoof>().Show();
        }
    }


    void OnDestroy()
    {
        LeaveStation();
        ThrowItem();
    }

}

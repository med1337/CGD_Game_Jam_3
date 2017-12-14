using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControl : Controllable
{
    public Player input;
    public Transform relative_point;

    [Header("Parameters")]
    [SerializeField] float move_speed;
    [SerializeField] float water_move_modifier;
    [SerializeField] Vector3 mount_scan_offset;
    [SerializeField] float mount_scan_radius;
    [SerializeField] float mount_scan_delay;
    [SerializeField] float jump_cooldown;
    [SerializeField] float punch_cooldown;

    [SerializeField] LayerMask station_layer;
    [SerializeField] LayerMask pickup_layer;

    [SerializeField] float throw_power;

    [Header("References")]
    public Rigidbody rigid_body;
    [SerializeField] GameObject smoke_puff_prefab;
    [SerializeField] GameObject punch_particle_prefab;
    [SerializeField] BuoyantObject buoyant_obj;
    [SerializeField] PlayerWorldCanvas player_canvas;

    private GameObject current_deck = null;

    private Station current_station;
    private Station nearest_station;
    private float last_scan_timestamp;
    private float last_jump_timestamp;
    private float last_punch_timestamp;

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
        move_dir = (relative_point.forward * _dir.z) + (relative_point.right * _dir.x);

        if (IsUsingStation())
        {
            current_station.controllable.Move(move_dir);
        }
        else
        {
            if (move_dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(move_dir);
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

        bool interact_prompt_enabled = !IsUsingStation() && !IsLifting() &&
                                       (nearest_station != null || nearest_pickup != null);
        player_canvas.SetInteractPromptActive(interact_prompt_enabled);

        if (interact_prompt_enabled)
        {
            if (nearest_station != null && nearest_pickup != null)
            {
                // If station is closer mount it
                if (Vector3.Distance(drop_position.position, nearest_station.transform.position) <
                    Vector3.Distance(drop_position.position, nearest_pickup.transform.position))
                {
                    nearest_station.outline_enabled = true;
                    nearest_pickup.outline_enabled = false;
                }
                else
                {
                    nearest_station.outline_enabled = false;
                    nearest_pickup.outline_enabled = true;
                }
            }
            else
            {
                if (nearest_station != null)
                    nearest_station.outline_enabled = true;

                if (nearest_pickup != null)
                    nearest_pickup.outline_enabled = true;
            }
        }

        if (input.GetButtonDown("Interact"))
            Interact();

        Move(new Vector3(horizontal, 0, vertical));

        if (!IsUsingStation())
        {
            if (input.GetButtonDown("Attack"))
                Attack();

            if (input.GetButtonDown("Jump"))
                Jump();

            if (Time.time >= last_scan_timestamp + mount_scan_delay)
            {
                last_scan_timestamp = Time.time;

                if (!IsLifting() && !IsUsingStation())
                {
                    StationScan();
                    PickUpScan();
                }
            }
        }
        else // Stuff to do with controlling a station ..
        {
            //transform.position = current_station.transform.position;
            //transform.rotation = current_station.transform.rotation;

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
        }
    }


    void ResetNearbyOutlines()
    {
        if (nearest_station != null)
            nearest_station.outline_enabled = false;

        if (nearest_pickup != null)
            nearest_pickup.outline_enabled = false;
    }


    void StationScan()
    {
        var colliders = Physics.OverlapSphere(drop_position.position, mount_scan_radius, station_layer);
        foreach (var collider in colliders)
        {
            var station = collider.GetComponent<Station>();
            if (station == null || station.occupied)
                continue;

            if (nearest_station != null && station != nearest_station)
                nearest_station.outline_enabled = false;

            nearest_station = station;
            return;
        }

        if (nearest_station != null)
            nearest_station.outline_enabled = false;

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

            if (nearest_pickup != null && pickup != nearest_pickup)
                nearest_pickup.outline_enabled = false;

            nearest_pickup = pickup;
            return;
        }

        if (nearest_pickup != null)
            nearest_pickup.outline_enabled = false;

        nearest_pickup = null;
    }


    void Interact()
    {
        ResetNearbyOutlines();

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
            {
                CarryItem();
            }
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

        rigid_body.isKinematic = true;

        current_station = nearest_station;
        current_station.controllable.OnControlStart(this);

        if (current_station.name == "TurretStation")
        {
            AudioManager.PlayOneShot("occupying_gun_station");
        }

        else if (current_station.name == "CaptainStation")
        {
            AudioManager.PlayOneShot("occupying_captain_station");
        }

        transform.position = current_station.transform.position;
        transform.rotation = current_station.transform.rotation;

        current_station.occupied = true;
        nearest_station = null;
    }


    public void LeaveStation()
    {
        if (!IsUsingStation() || IsLifting())
        {
            return;
        }

        rigid_body.isKinematic = false;

        if (current_station.name == "TurretStation")
        {
            AudioManager.PlayOneShot("leaving_gun_station");
        }

        else if (current_station.name == "CaptainStation")
        {
            AudioManager.PlayOneShot("leaving_captain_station");
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

        nearest_pickup.SetInUse(true);

        // Disable Components.
        nearest_pickup.GetComponent<Rigidbody>().isKinematic = true;
        nearest_pickup.GetComponent<Collider>().enabled = false;

        // Set player as parent of Pickup.
        nearest_pickup.transform.parent = (this.transform);
        current_pickup = nearest_pickup;
        nearest_pickup = null;
    }


    public void ThrowItem()
    {
        if (IsUsingStation() || !IsLifting())
        {
            return;
        }

        // Throw/drop.
        current_pickup.transform.position = drop_position.position;
        current_pickup.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        current_pickup.SetInUse(false);

        // Enable disabled components.
        current_pickup.GetComponent<Rigidbody>().isKinematic = false;
        current_pickup.GetComponent<Collider>().enabled = true;

        // If player isnt moving.... place the object.
        if (move_dir == Vector3.zero)
        {
            AudioManager.PlayOneShot("drop_box");
            current_pickup.GetComponent<Rigidbody>().AddForce(transform.forward + transform.up);
        }
        // if the player is moving throw the object.
        else
        {
            AudioManager.PlayOneShot("throwing");
            current_pickup.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * throw_power);
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
            float hit_force = 2.5f;

            if (Time.time < last_punch_timestamp + punch_cooldown)
                return;

            last_punch_timestamp = Time.time;

            Instantiate(punch_particle_prefab, carry_position.position, transform.rotation);

            //check in front of player
            Collider[] hitColliders = Physics.OverlapSphere(carry_position.position, 1.0f);

            List<Rigidbody> hit_rigidbodies = new List<Rigidbody>();            

            int i = 0;
            while (i < hitColliders.Length)
            {
                Rigidbody hit_rigidbody = hitColliders[i].GetComponent<Rigidbody>();

                if (!hit_rigidbody)
                {
                    hit_rigidbody = hitColliders[i].GetComponentInParent<Rigidbody>();
                }

                if (hit_rigidbody == rigid_body)
                {
                    i++;
                    continue;
                }
                    

                if ((hit_rigidbody) && (!hit_rigidbodies.Contains(hit_rigidbody)))
                {
                    Debug.Log(hitColliders[i].gameObject.name);

                    if (hit_rigidbody.gameObject.name == "Player(Clone)")
                    {
                        Debug.Log("hit player");

                        PlayerControl control_script = hitColliders[i].GetComponentInParent<PlayerControl>();
                        control_script.ThrowItem();
                        control_script.LeaveStation();
                    }

                    //if on a deck, don't punch the deck's rigidbody
                    if (current_deck)
                    {
                        if (current_deck.GetComponent<Rigidbody>() == hit_rigidbody)
                        {
                            i++;
                            continue;
                        }
                    }

                    //otherwise smack it 
                    AudioManager.PlayOneShot("slap");
                    hit_rigidbody.AddForce(transform.forward * hit_force, ForceMode.VelocityChange);
                    hit_rigidbody.AddForce(transform.up * hit_force * 2, ForceMode.VelocityChange);

                    hit_rigidbodies.Add(hit_rigidbody);

                }

                i++;
            }
        }
    }


    void Jump()
    {
        if (Time.time < last_jump_timestamp + jump_cooldown)
            return;

        last_jump_timestamp = Time.time;

        AudioManager.PlayOneShot("jump");
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
        ResetNearbyOutlines();
        LeaveStation();
        ThrowItem();
    }

}

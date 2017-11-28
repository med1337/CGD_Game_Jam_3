using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] float cleanup_after = 60;

    public bool outline_enabled
    {
        get
        {
            return outlines.TrueForAll(elem => elem.enabled);
        }

        set
        {
            outlines.ForEach(elem => elem.enabled = value);
        }
    }

    [SerializeField] List<Outline> outlines;

    private bool in_use;
    private bool hooked;
    private Vector3 trajectory_start;
    private Transform trajectory_end;
    private float hook_progress;

    private float cleanup_timer;


    public void SetInUse(bool _in_use)
    {
        in_use = _in_use;
        cleanup_timer = 0;
    }


    public bool GetInUse()
    {
        return in_use || hooked;
    }


    void Update()
    {
        if (GetInUse())
        {
            cleanup_timer = 0;
        }
        else
        {
            float prev_timer = cleanup_timer;
            cleanup_timer += Time.deltaTime;

            if (prev_timer < cleanup_after && cleanup_timer >= cleanup_after)
            {
                Destroy(GetComponent<Collider>());
                Destroy(this.gameObject, 5);
            }
        }

        if (hooked)
            HookedUpdate();
    }


    void HookedUpdate()
    {
        float trajectory_height = 5;
        hook_progress += Time.deltaTime;

        Vector3 lerp = Vector3.Lerp(trajectory_start, trajectory_end.position, hook_progress);
        lerp.y += trajectory_height * Mathf.Sin(Mathf.Clamp01(hook_progress) * Mathf.PI);

        transform.position = lerp;

        if (Vector3.Distance(transform.position, trajectory_end.position) < 1)
        {
            hooked = false;
            hook_progress = 0;
        }
    }


    void OnTriggerStay(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            cleanup_timer = 0;
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


    void OnCollisionEnter(Collision _other)
    {
        if (_other.rigidbody != null)
        {
            LifeRing life_ring = _other.rigidbody.GetComponent<LifeRing>();
            if (life_ring != null)
            {
                trajectory_start = transform.position;
                trajectory_end = life_ring.trajectory_end_ref;

                hooked = true;
                hook_progress = 0;
            }
        }
    }

}

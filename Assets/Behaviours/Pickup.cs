using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool in_use;
    private bool hooked;
    private Vector3 trajectory_start;
    private Transform trajectory_end;
    private float hook_progress;


    public void SetInUse(bool _in_use)
    {
        in_use = _in_use;
    }


    public bool GetInUse()
    {
        return in_use || hooked;
    }


    void Update()
    {
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

            Debug.Log("hook end");
        }
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

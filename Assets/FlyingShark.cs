using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShark : MonoBehaviour {

    [SerializeField] bool hooked;
    [SerializeField] bool caught = false;
    private Vector3 trajectory_start;
    private Transform trajectory_end;
    private float hook_progress;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (hooked)
        {
            HookedUpdate();
        }
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
            caught = true;
            hooked = false;
            hook_progress = 0;
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        if (!caught)
        {
            if (!hooked)
            {
                if (_other.tag == "Deck")
                {
                    trajectory_start = transform.position;
                    trajectory_end = _other.transform;

                    hooked = true;
                    hook_progress = 0;

                    GetComponent<AICaptain>().Kill();
                }
            }
        }
    }
}

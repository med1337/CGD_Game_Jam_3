using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropPoint : MonoBehaviour
{
    [SerializeField] string tag;
    [SerializeField] float despawn_timer;

    // Detect Loot Dropped
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(tag))
        {
            // increase player score ect.

            // despawn loot container.
            Destroy(collider.gameObject, despawn_timer);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropPoint : MonoBehaviour
{
    [SerializeField] string tag;
    [SerializeField] float despawn_timer;
    [SerializeField] ParticleSystem coin_particle;
    private VillageStats stats;

    void Start()
    {
        stats = GetComponent<VillageStats>();
    }

    // Detect Loot Dropped
    void OnTriggerEnter(Collider collider)
    { 
        if (collider.CompareTag(tag))
        {
            float current_money = stats.GetMoney();

            float cargo_value = collider.GetComponent<CargoStats>().GetValue();

            // increase player score ect.
            stats.SetMoney(current_money + cargo_value);

            Vector3 coin_spawn = collider.transform.position;

            coin_spawn.y =+ 1;

            // despawn loot container.
            Destroy(collider.gameObject, despawn_timer);

            Instantiate(coin_particle, coin_spawn, coin_particle.transform.rotation);
        }
    }
}
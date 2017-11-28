using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropPoint : MonoBehaviour
{
    [SerializeField]
    float despawn_timer;
    [SerializeField]
    ParticleSystem coin_particle;
    [SerializeField]
    float destroy_timer = 1.0f;
    private VillageStats stats;
    private List<GameObject> collected_items = new List<GameObject>();
    private List<float> collection_timers = new List<float>();


    void Start()
    {
        stats = GetComponent<VillageStats>();
    }

    void Update()
    {
        collected_items.RemoveAll(item => item == null);

        if (collection_timers.Count > 0)
        {
            for (int i = collection_timers.Count - 1; i >= 0; i--)
            {
                collection_timers[i] += Time.deltaTime;

                if (collection_timers[i] > destroy_timer)
                {
                    collection_timers.RemoveAt(i);
                    if (collected_items[i])
                    {
                        Instantiate(coin_particle, collected_items[i].transform.position, coin_particle.transform.rotation);
                        Destroy(collected_items[i]);
                        AudioManager.PlayOneShot("coin_spill");
                    }
                }
            }
        }
    }

    // Detect Loot Dropped
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Loot"))
        {
            Destroy(collider.gameObject.GetComponent<Pickup>());

            collected_items.Add(collider.gameObject);

            float timer = 0.0f;
            collection_timers.Add(timer);

            float current_money = stats.GetMoney();

            int cargo_value = collider.GetComponent<CargoStats>().GetValue();

            // increase player score ect.

            GameManager.scene.player_score.IncreaseScore(cargo_value);            
        }
    }
}
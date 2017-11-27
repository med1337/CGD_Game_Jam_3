using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipLootSpawnManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] LayerMask check_layers; // Raycast Layers to check spawn pos is in sea
    [SerializeField] float spawn_distance; // Distance to spawn object, away from avg player pos
    [SerializeField] float clear_distance;

    [Space]
    [Header("Ship Type Populations")]
    // Max pop for each Ship class
    [SerializeField] int max_loot_pop;
    [SerializeField] int max_civilian_pop;
    [SerializeField] int max_cargo_pop;
    [SerializeField] int max_navy_pop;

    [Space]
    [Header("Time Between Spawning")]
    // Time Between Spawning a new object of this class
    [SerializeField] int loot_spawn_timer;
    [SerializeField] int civ_spawn_timer;
    [SerializeField] int cargo_spawn_timer;
    [SerializeField] int navy_spawn_timer;

    [Space]
    [Header("How Often Ships Are Cleared")]
    [SerializeField] int update_timer;

    [Space]
    [Header("Ship Type Prefabs")]
    [SerializeField] List<GameObject> loot_prefabs;
    [SerializeField] List<GameObject> civilian_ships_prefabs;
    [SerializeField] List<GameObject> cargo_ships_prefabs;
    [SerializeField] List<GameObject> navy_ships_prefabs;

    // Handles to ships in scene
    private List<GameObject> floating_loot;
    private List<AICaptain> civilian_ships;
    private List<AICaptain> cargo_ships;
    private List<AICaptain> navy_ships;

    private float loot_counter;
    private float civ_counter;
    private float cargo_counter;
    private float navy_counter;
    private float update_counter;

    //How many times to check for a spawn pos
    private int search_attempts = 20;

    private void Start()
    {
        floating_loot  = new List<GameObject>();
        civilian_ships = new List<AICaptain>();
        cargo_ships    = new List<AICaptain>();
        navy_ships     = new List<AICaptain>();

        loot_counter     = 0;
        civ_counter      = 0;
        cargo_counter    = 0;
        navy_counter     = 0;
        update_counter   = 0;
    }

    private void Update()
    {
        SpawnNewItems(); // Spawn Ships

        StatusUpdate(); // Update Ships & Loot

        // Update Spawn Timers
        loot_counter   += Time.deltaTime;
        civ_counter    += Time.deltaTime;
        cargo_counter  += Time.deltaTime;
        navy_counter   += Time.deltaTime;
        update_counter += Time.deltaTime;
    }


    void StatusUpdate()
    {
        Vector3 pos = new Vector3(GameManager.scene.camera_manager.target_pos.x, 0.0f,
        GameManager.scene.camera_manager.target_pos.z);

        if (update_counter >= update_timer)
        {
            ClearShips(civilian_ships, pos);

            ClearShips(cargo_ships, pos);

            ClearShips(navy_ships, pos);

            ClearLoot(floating_loot, pos);

            // Reset Timer
            update_counter = 0;
        }
    }


    // Clear Inactive or Distant ships
    void ClearShips(List<AICaptain> _ships, Vector3 _pos)
    {
        // Remove Disabled Ships
        _ships.RemoveAll(item => item == null);

        Vector3 pos = new Vector3(GameManager.scene.camera_manager.target_pos.x, 0.0f,
            GameManager.scene.camera_manager.target_pos.z);

        for (int i = _ships.Count - 1; i > -1; i--)
        {
            if(Vector3.Distance(_pos, _ships[i].transform.position) > clear_distance)
            {
                Destroy(_ships[i].gameObject, 0.5f);
                _ships.RemoveAt(i);
                Debug.Log("Captain too far away... Removing");
            }
        }
    }

    
    void ClearLoot(List<GameObject> _items, Vector3 _pos)
    {
        for (int i = floating_loot.Count - 1; i > -1; i--)
        {
            if (Vector3.Distance(_pos, floating_loot[i].transform.position) > clear_distance)
            {
                Destroy(floating_loot[i], 0.5f);
                floating_loot.RemoveAt(i);
                Debug.Log("Loot too far away... Removing");
            }

            else if(floating_loot[i].transform.parent != null)
            {
                floating_loot.RemoveAt(i);
                Debug.Log("Players have claimed Loot");
            }
        }
    }


    void SpawnNewItems()
    {
        //Spawn new CIV boat
        if (civ_counter >= civ_spawn_timer && civilian_ships.Count < max_civilian_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnItem(pos, civilian_ships_prefabs);
            }

            civ_counter = 0;
        }

        // Spawn new CARGO boat
        if (cargo_counter >= cargo_spawn_timer && cargo_ships.Count < max_cargo_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnItem(pos, cargo_ships_prefabs);
            }

            cargo_counter = 0;
        }

        // Spawn new NAVY boat
        if (navy_counter >= navy_spawn_timer && navy_ships.Count < max_navy_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnItem(pos, navy_ships_prefabs);
            }

            navy_counter = 0;
        }

        // Spawn Cargo
        if (loot_counter >= loot_spawn_timer && floating_loot.Count < max_loot_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnItem(pos, loot_prefabs);
            }

            loot_counter = 0;
        }
    }


    bool ValidatePos(Vector3 _pos)
    {
        _pos = GeneratePos();

        if(_pos != Vector3.zero)
        {
            return true;
        }

        Debug.Log("No position found...");
        return false;
    }


    Vector3 GeneratePos()
    {
        // Loop until a position is found
        for (int i = 0; i < search_attempts; i++)
        {
            float angle = Random.Range(0, 360);

            // Take Cameras X & Z position
            Vector3 position = new Vector3 (GameManager.scene.camera_manager.target_pos.x, 0.0f,
                GameManager.scene.camera_manager.target_pos.z);

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 dist_to_target = rotation * transform.forward * spawn_distance;

            Vector3 potential_pos = position + dist_to_target;

            Vector3 potential_pos_valid = ValidDestination(potential_pos);

            if (Vector3.Distance(potential_pos_valid, Vector3.zero) < GameManager.map_bound_radius)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(potential_pos_valid.x, 100.0f, potential_pos_valid.z),
                    Vector3.down, out hit, 100.0f, check_layers))
                {
                    Debug.DrawRay(position, dist_to_target, Color.red, 3);

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                        return potential_pos_valid;
                }
            }
        }

        return Vector3.zero;
    }


    private Vector3 ValidDestination(Vector3 _desired_waypoint)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(_desired_waypoint, out hit, 100, NavMesh.AllAreas))
            return _desired_waypoint;

        NavMesh.FindClosestEdge(_desired_waypoint, out hit, NavMesh.AllAreas);
        return hit.position;
    }


    // Spawn a new Ship OR Loot
    void SpawnItem(Vector3 _spawn_pos, List<GameObject> object_type)
    {
        int rand_item = Random.Range(0, object_type.Count);

        var game_obj = Instantiate(object_type[rand_item], _spawn_pos,
            object_type[rand_item].transform.rotation);


        if (object_type == civilian_ships_prefabs)
        {
            civilian_ships.Add(game_obj.GetComponent<AICaptain>());
        }

        if (object_type == cargo_ships_prefabs)
        {
            cargo_ships.Add(game_obj.GetComponent<AICaptain>());
        }

        if (object_type == navy_ships_prefabs)
        {
            navy_ships.Add(game_obj.GetComponent<AICaptain>());
        }

        if (object_type == loot_prefabs)
        {
            floating_loot.Add(game_obj.gameObject);
        }
    }


    void OnDrawGizmos()
    {
        Vector3 pos = new Vector3(GameManager.scene.camera_manager.target_pos.x, 0.0f,
            GameManager.scene.camera_manager.target_pos.z);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pos, clear_distance);
    }
}

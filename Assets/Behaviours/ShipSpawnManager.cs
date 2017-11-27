using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipSpawnManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] LayerMask check_layers; // Raycast Layers to check spawn pos is in sea
    [SerializeField] float spawn_distance; // Distance to spawn ship, away from avg player pos
    [SerializeField] float clear_ship_distance;

    [Space]
    [Header("Ship Type Populations")]
    // Max pop for each Ship class
    [SerializeField] int max_civilian_pop;
    [SerializeField] int max_cargo_pop;
    [SerializeField] int max_navy_pop;

    [Space]
    [Header("Time Between Spawning")]
    // Time Between Spawning a new ship of this clas
    [SerializeField] int civ_spawn_timer;
    [SerializeField] int cargo_spawn_timer;
    [SerializeField] int navy_spawn_timer;

    [Space]
    [Header("How Often Ships Are Cleared")]
    [SerializeField] int update_ship_timer;

    [Space]
    [Header("Ship Type Prefabs")]
    [SerializeField] List<GameObject> civilian_ships_prefabs;
    [SerializeField] List<GameObject> cargo_ships_prefabs;
    [SerializeField] List<GameObject> navy_ships_prefabs;

    // Handles to ships in scene
    private List<AICaptain> civilian_ships;
    private List<AICaptain> cargo_ships;
    private List<AICaptain> navy_ships;

    private float civ_counter;
    private float cargo_counter;
    private float navy_counter;
    private float update_counter;

    //How many times to check for a spawn pos
    private int search_attempts = 20;

    private void Start()
    {
        civilian_ships = new List<AICaptain>();
        cargo_ships    = new List<AICaptain>();
        navy_ships     = new List<AICaptain>();

        civ_counter      = 0;
        cargo_counter    = 0;
        navy_counter     = 0;
        update_counter   = 0;
    }

    private void Update()
    {
        SpawnNewShips(); // Spawn Ships
        StatusUpdate(); // Update Ships

        // Update Spawn Timers
        civ_counter    += Time.deltaTime;
        cargo_counter  += Time.deltaTime;
        navy_counter   += Time.deltaTime;
        update_counter += Time.deltaTime;
    }


    void StatusUpdate()
    {
        if(update_counter >= update_ship_timer)
        {
            ClearShips(civilian_ships);

            ClearShips(cargo_ships);

            ClearShips(navy_ships);

            // Reset Timer
            update_counter = 0;
        }
    }


    // Clear Inactive or Distant ships
    void ClearShips(List<AICaptain> _ships)
    {
        // Remove Disabled Ships
        _ships.RemoveAll(item => item == null);

        Vector3 pos = new Vector3(GameManager.scene.camera_manager.target_pos.x, 0.0f,
            GameManager.scene.camera_manager.target_pos.z);

        for (int i = _ships.Count - 1; i > -1; i--)
        {
            if(Vector3.Distance(pos, _ships[i].transform.position) > clear_ship_distance)
            {
                Destroy(_ships[i].gameObject, 0.5f);
                _ships.RemoveAt(i);
                Debug.Log("Captain too far away... Removing");
            }
        }
    }


    void SpawnNewShips()
    {
        //Spawn new CIV boat
        if (civ_counter >= civ_spawn_timer && civilian_ships.Count < max_civilian_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnShip(pos, civilian_ships_prefabs);
            }

            civ_counter = 0;
        }

        // Spawn new CARGO boat
        if (cargo_counter >= cargo_spawn_timer && cargo_ships.Count < max_cargo_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnShip(pos, cargo_ships_prefabs);
            }

            cargo_counter = 0;
        }

        // Spawn new NAVY boat
        if (navy_counter >= navy_spawn_timer && navy_ships.Count < max_navy_pop)
        {
            Vector3 pos = GeneratePos();

            if (ValidatePos(pos))
            {
                SpawnShip(pos, navy_ships_prefabs);
            }

            navy_counter = 0;
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


    void SpawnShip(Vector3 _spawn_pos, List<GameObject> _ship_class)
    {
        int rand_ship = Random.Range(0, _ship_class.Count);

        var ship = Instantiate(_ship_class[rand_ship], _spawn_pos,
            _ship_class[rand_ship].transform.rotation);


        if (_ship_class == civilian_ships_prefabs)
        {
            civilian_ships.Add(ship.GetComponent<AICaptain>());
        }

        if (_ship_class == cargo_ships_prefabs)
        {
            cargo_ships.Add(ship.GetComponent<AICaptain>());
        }

        if (_ship_class == navy_ships_prefabs)
        {
            navy_ships.Add(ship.GetComponent<AICaptain>());
        }
    }


    void OnDrawGizmos()
    {
        Vector3 pos = new Vector3(GameManager.scene.camera_manager.target_pos.x, 0.0f,
            GameManager.scene.camera_manager.target_pos.z);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pos, clear_ship_distance);
    }
}

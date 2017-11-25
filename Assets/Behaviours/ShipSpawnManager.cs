using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipSpawnManager : MonoBehaviour
{
    [SerializeField] LayerMask check_layers;
    [SerializeField] float spawn_distance;

    [SerializeField] int max_civilian_pop;
    [SerializeField] int max_cargo_pop;
    [SerializeField] int max_navy_pop;

    [SerializeField] List<GameObject> civilian_ships_prefabs;
    [SerializeField] List<GameObject> cargo_ship_prefabs;
    [SerializeField] List<GameObject> navy_ship_prefabs;

    private List<GameObject> civilian_ships;
    private List<GameObject> cargo_ships;
    private List<GameObject> navy_ships;

    private int civ_spawn_timer = 2;
    private int cargo_spawn_timer = 5;
    private int navy_spawn_timer = 10;

    private float civ_timer;
    private float cargo_timer;
    private float navy_timer;

    private void Start()
    {
        civilian_ships = new List<GameObject>();
        cargo_ships = new List<GameObject>();
        navy_ships = new List<GameObject>();

        civ_timer = 0;
        cargo_timer = 0;
        navy_timer = 0;
    }

    private void Update()
    {
        //Spawn new CIV boat
        if (civ_timer >= civ_spawn_timer && civilian_ships.Count < max_civilian_pop)
        {
            SpawnShip(GeneratePos(), civilian_ships_prefabs);
            civ_timer = 0;
        }

        // Spawn new CARGO boat
        if (cargo_timer >= cargo_spawn_timer && cargo_ships.Count < max_cargo_pop)
        {
            SpawnShip(GeneratePos(), cargo_ship_prefabs);
            cargo_timer = 0;
        }

        // Spawn new NAVY boat
        if (navy_timer >= navy_spawn_timer && navy_ships.Count < max_navy_pop)
        {
            SpawnShip(GeneratePos(), navy_ship_prefabs);
            navy_timer = 0;
        }


        // Update Spawn Timers
        civ_timer += Time.deltaTime;
        cargo_timer += Time.deltaTime;
        navy_timer += Time.deltaTime;
    }


    Vector3 GeneratePos()
    {
        float angle = Random.Range(0, 360);

        // Loop until a position is found
        while (true)
        {
            //Vector3 position = GameManager.scene.camera_manager.target_pos;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 dist_to_target = rotation * transform.forward * spawn_distance;

            Vector3 potential_pos = transform.position + dist_to_target;

            Vector3 safe_spawn = ValidDestination(potential_pos);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(safe_spawn.x, 100.0f, safe_spawn.z), Vector3.down, out hit, 100.0f, check_layers))
            {
                Debug.DrawRay(transform.position, dist_to_target, Color.red, 3);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                    return safe_spawn;
            }

            // Increase angle and try next position
            angle += 10.0f;
        }
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

        var ship = Instantiate(_ship_class[rand_ship], _spawn_pos, _ship_class[rand_ship].transform.rotation);


        if (_ship_class == civilian_ships_prefabs)
        {
            civilian_ships.Add(ship);
        }

        if (_ship_class == cargo_ship_prefabs)
        {
            cargo_ships.Add(ship);
        }

        if (_ship_class == navy_ship_prefabs)
        {
            navy_ships.Add(ship);
        }
    }


    public void SetNavySpawnTimer(int _time)
    {
        navy_spawn_timer = _time;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawnManager : MonoBehaviour
{
    [SerializeField] LayerMask ship_mask;
    [SerializeField] int ship_scan_range;

    [SerializeField] int max_civilian_pop;
    [SerializeField] int max_cargo_pop;
    [SerializeField] int max_navy_pop;

    [SerializeField] List<GameObject> civilian_ships;
    [SerializeField] List<GameObject> cargo_ships;
    [SerializeField] List<GameObject> navy_ships;


    private int civ_spawn_timer;
    private int cargo_spawn_timer;
    private int navy_spawn_timer;

    private float civ_timer;
    private float cargo_timer;
    private float navy_timer;

    private void Start()
    {
        civ_timer = 0;
        cargo_timer = 0;
        navy_timer = 0;
    }

    private void Update()
    {
        //Spawn new CIV boat
        if (civ_timer >= civ_spawn_timer && civilian_ships.Count < max_civilian_pop)
        {
            SpawnShip(GenerateIniPos(), civilian_ships);
            civ_timer = 0;
        }

        // Spawn new CARGO boat
        if (cargo_timer >= cargo_spawn_timer && cargo_ships.Count < max_cargo_pop)
        {
            SpawnShip(GenerateIniPos(), cargo_ships);
            cargo_timer = 0;
        }

        // Spawn new NAVY boat
        if (navy_timer >= navy_spawn_timer && navy_ships.Count < max_navy_pop)
        {
            SpawnShip(GenerateIniPos(), navy_ships);
            navy_timer = 0;
        }

        civ_timer += Time.deltaTime;
        cargo_timer += Time.deltaTime;
        navy_timer += Time.deltaTime;
    }


    Vector3 GenerateIniPos()
    {
        float degrees = Random.Range(0, 360);
        float distance = 100.0f;

        //GameManager.scene.CameraManager

        while(true)
        {
            Vector3 position = Vector3.zero/* Toms Function */;
            Quaternion rotation = Quaternion.AngleAxis(degrees, Vector3.up);

            Vector3 dist_to_direction = rotation * transform.forward * distance;
            Debug.DrawRay(transform.position, dist_to_direction, Color.red, 10.0f);

            // Call Toms Static function to Find position of
            Collider[] colliders = Physics.OverlapSphere(position, ship_scan_range, ship_mask);

            if(colliders.Length == 0)
            {
                return position;
            }
        }



        // Generate a position thats safe and return position
        return Vector3.zero;
    }


    void SpawnShip(Vector3 _spawn_pos, List<GameObject> _ship_class)
    {

    }


    public void SetNavySpawnTimer(int _time)
    {
        navy_spawn_timer = _time;
    }
}

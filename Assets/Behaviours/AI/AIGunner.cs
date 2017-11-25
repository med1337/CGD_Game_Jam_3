using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(TurretControl))]
public class AIGunner : MonoBehaviour
{
    [SerializeField] float detect_radius = 20;
    [SerializeField] private float scatter_radius = 3;

    private PlayerControl closest_enemy = null;
    private TurretControl turret = null;


    void Start()
    {
        turret = GetComponent<TurretControl>();
        turret.ai_controlling = true;
    }


    void Update()
    {
        FindClosestPlayer();
        UpdateGunBehaviour();      
    }


    public void Kill()
    {
        turret.ai_controlling = false;
        Destroy(this);
    }


    private void UpdateGunBehaviour()
    {
        if (closest_enemy == null)
            return;

        if (closest_enemy.transform.parent == null)
            return;

        if (closest_enemy.transform.root.GetComponent<AICaptain>() != null)//if the boat they are on has an ai captain stop firing
            return;

        if ((transform.position - closest_enemy.transform.position).sqrMagnitude <=
            detect_radius * detect_radius)
        {
            Vector2 random = Random.insideUnitCircle * scatter_radius;//get random point in circle
            Vector3 random_scatter = new Vector3(random.x, 0, random.y);//convert to vec3
            Vector3 target = closest_enemy.transform.parent.position + random_scatter;

            Vector3 aim_dir = (target - transform.position).normalized ;
            turret.Move(aim_dir);
            turret.Activate();
        }
    }


    private void FindClosestPlayer()
    {
        List<PlayerControl> current_players = GameManager.scene.respawn_manager.alive_players;

        PlayerControl current_closest = null;
        float closest_distance = Mathf.Infinity;

        foreach (PlayerControl player in current_players)
        {
            if (player == null)
                continue;

            float distance = (transform.position - player.transform.position).sqrMagnitude;

            if (distance < closest_distance)
            {
                current_closest = player;//store new closest
                closest_distance = distance;
            }
        }

        closest_enemy = current_closest;
    }
}

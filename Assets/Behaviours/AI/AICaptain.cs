using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AICaptain : MonoBehaviour
{
    private enum CaptainState
    {
        PATROL,
        CHASE,
        DEAD
    }

    [SerializeField] private Faction captain_faction;
    [SerializeField] private BoatControl boat_control;
    [SerializeField] private NavMeshAgent nav_mesh_agent;
    
    [SerializeField] private float waypoint_radius;
    [SerializeField] private float distance_between_waypoints;
    [SerializeField] private float chase_radius;
    [SerializeField] private float patrol_speed = 10;
    [SerializeField] private float chase_speed = 15;
    [SerializeField] private float broad_side_distance = 1;

    private CaptainState current_state = CaptainState.PATROL;
    private PlayerControl closest_enemy;
    private Vector3 waypoint;


    void Start()
    {
        nav_mesh_agent.speed = patrol_speed;
        SetNewPatrolPoint();
    }


	void Update ()
    {
        UpdateState();
	}


    public void Kill()
    {
        current_state = CaptainState.DEAD;
    }


    public void Revive()
    {
        current_state = CaptainState.PATROL;
    }


    void UpdateState()
    {
        switch (current_state)
        {
            case CaptainState.PATROL: Patrol();
                break;
            case CaptainState.CHASE: Chase();
                break;
            case CaptainState.DEAD:
                break;
        }
    }


    void Patrol()
    {
        if (nav_mesh_agent.remainingDistance < 5)
            SetNewPatrolPoint();

        FindClosestPlayer();
        CheckTransitionToChase();
    }


    void FindClosestPlayer()
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
                current_closest = player;
                closest_distance = distance;
            }
        }

        closest_enemy = current_closest;
    }


    void CheckTransitionToChase()
    {
        if (closest_enemy == null)
            return;

        if ((transform.position - closest_enemy.transform.position).sqrMagnitude <=
            chase_radius * chase_radius) //chase when a player is near
        {
            nav_mesh_agent.speed = chase_speed;
            current_state = CaptainState.CHASE;
        }
    }


    void SetNewPatrolPoint()
    {
        Vector2 random = Random.insideUnitCircle * waypoint_radius;//get random point in circle
        Vector3 random_waypoint = new Vector3(random.x, boat_control.transform.position.y, random.y);//convert to vec3

        waypoint = transform.position+ boat_control.transform.forward * distance_between_waypoints +  random_waypoint;//put rand point in front of boat
        nav_mesh_agent.SetDestination(waypoint);//set nav direction
    }


    void Chase()
    {
        CheckTransitionToPatrol();

        if (captain_faction == Faction.NAVY)
        {
            SetChaseTarget();
        }
        else if (captain_faction == Faction.CIVILIAN)
        {
            SetFleeTarget();
        }  
    }


    void CheckTransitionToPatrol()
    {
        if (closest_enemy == null)
        {
            current_state = CaptainState.PATROL;
            return;
        }

        if ((transform.position - closest_enemy.transform.position).sqrMagnitude >
            chase_radius * chase_radius) //chase when a player is near
        {
            nav_mesh_agent.speed = chase_speed;
            current_state = CaptainState.PATROL;
        }
    }


    void SetChaseTarget()
    {
        Vector3 chase_target = closest_enemy.transform.position + (closest_enemy.transform.right * broad_side_distance);
        nav_mesh_agent.SetDestination(chase_target);
    }


    void SetFleeTarget()
    {
        Vector3 flee_target = (transform.position - closest_enemy.transform.position).normalized * distance_between_waypoints;
        nav_mesh_agent.SetDestination(flee_target);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 next_waypoint_pos = transform.position + boat_control.transform.forward * distance_between_waypoints;
        Gizmos.DrawWireSphere(next_waypoint_pos, waypoint_radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chase_radius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(waypoint, 5);
    }
}

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

    [SerializeField] private Faction captain_faction = Faction.CIVILIAN;
    [SerializeField] private BoatControl boat_control;
    [SerializeField] private NavMeshAgent nav_mesh_agent;
    [SerializeField] private Transform deck_volume;
    
    [SerializeField] private float waypoint_radius = 60;
    [SerializeField] private float distance_between_waypoints = 45;
    [SerializeField] private float chase_radius = 40;
    [SerializeField] private float patrol_speed = 10;
    [SerializeField] private float chase_speed = 11;
    [SerializeField] private float broad_side_distance = 5;

    private CaptainState current_state = CaptainState.PATROL;
    private PlayerControl closest_enemy;
    private Vector3 waypoint;


    private void Start()
    {
        nav_mesh_agent.speed = patrol_speed;
        SetNewPatrolPoint();
    }


    private void Update ()
    {
        UpdateState();
	}


    public void Kill()
    {
        current_state = CaptainState.DEAD;
        Destroy(nav_mesh_agent);
        Destroy(this);
    }


    private void UpdateState()
    {
        CheckTransitionToDead();

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


    private void Patrol()
    {
        if (nav_mesh_agent.remainingDistance < 5)
            SetNewPatrolPoint();

        FindClosestPlayer();
        CheckTransitionToChase();
    }


    private void CheckTransitionToDead()
    {
      //if boat is destroyed transition to death      
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
                current_closest = player;
                closest_distance = distance;
            }
        }

        closest_enemy = current_closest;
    }


    private void CheckTransitionToChase()
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


    private void SetNewPatrolPoint()
    {
        Vector2 random = Random.insideUnitCircle * waypoint_radius;//get random point in circle
        Vector3 random_waypoint = new Vector3(random.x, boat_control.transform.position.y, random.y);//convert to vec3

        waypoint = transform.position+ boat_control.transform.forward * distance_between_waypoints +  random_waypoint;//put rand point in front of boat
        nav_mesh_agent.SetDestination(waypoint);//set nav direction
    }


    private void Chase()
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


    private void CheckTransitionToPatrol()
    {
        if (closest_enemy == null)
        {
            current_state = CaptainState.PATROL;
            return;
        }

        if ((transform.position - closest_enemy.transform.position).sqrMagnitude >
            chase_radius * chase_radius || closest_enemy.transform.parent == deck_volume) //chase when a player is near
        {
            nav_mesh_agent.speed = chase_speed;
            current_state = CaptainState.PATROL;
        }
    }


    private void SetChaseTarget()
    {
        if (closest_enemy == null)
            return;

        if (closest_enemy.transform.parent == null)
            return;

        if (closest_enemy.transform.parent == deck_volume)//disable movement if player is on board
        {
            nav_mesh_agent.isStopped = true;
            return;
        }

        nav_mesh_agent.isStopped = false;

        Vector3 chase_target = closest_enemy.transform.parent.position + (closest_enemy.transform.right * broad_side_distance);//use players parent whih should be boat
        nav_mesh_agent.SetDestination(chase_target);
    }


    private void SetFleeTarget()
    {
        if (closest_enemy == null)
            return;

        Vector3 flee_target = (transform.position - closest_enemy.transform.position).normalized * distance_between_waypoints;
        nav_mesh_agent.SetDestination(flee_target);
    }


    private void OnDrawSelectedGizmos()
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

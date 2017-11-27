using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.AI;


public class AICaptain : MonoBehaviour
{
    private enum AICaptainState
    {
        PATROL,
        CHASE
    }

    [SerializeField] bool is_starting_boat = false;
    [Space]

    [Header("References")] 
    [SerializeField] NavMeshAgent nav_mesh_agent;
    [SerializeField] NavMeshObstacle nav_mesh_obstacle;
    [SerializeField] Transform deck_volume;
    [Space]

    [Header("Behaviour Parameters")]
    [SerializeField] Faction captain_faction = Faction.CIVILIAN;
    [SerializeField] float waypoint_radius = 60;
    [SerializeField] float distance_between_waypoints = 45;
    [SerializeField] float chase_radius = 40;
    [SerializeField] float patrol_speed = 10;
    [SerializeField] float chase_speed = 11;
    [SerializeField] float broad_side_distance = 5;

    private AICaptainState current_state = AICaptainState.PATROL;
    private PlayerControl closest_enemy = null;
    private Vector3 waypoint;


    private void Start()
    {
        if (is_starting_boat)
        {
            Kill();
            return;
        }

        nav_mesh_agent.speed = patrol_speed;
        SetNewPatrolPoint();//set start patrol waypoint
    }


    private void Update ()
    {
        UpdateState();
	}


    public void Kill()
    {
        Destroy(nav_mesh_agent);//destroy AI related components
        nav_mesh_obstacle.enabled = true;
        Destroy(this);//will allow player to captain ship       
    }


    private void UpdateState()
    {
        switch (current_state)
        {
            case AICaptainState.PATROL: Patrol();
                break;
            case AICaptainState.CHASE: Chase();
                break;
        }
    }


    private void Patrol()
    {
        const float TOLERANCE = 5;
        if (nav_mesh_agent.remainingDistance <= TOLERANCE)
            SetNewPatrolPoint();

        FindClosestPlayer();
        CheckTransitionToChase();
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


    private void CheckTransitionToChase()
    {
        if (closest_enemy == null)
            return;

        if ((transform.position - closest_enemy.transform.position).sqrMagnitude <=
            chase_radius * chase_radius) //chase when a player is near
        {
            nav_mesh_agent.speed = chase_speed;
            current_state = AICaptainState.CHASE;
        }
    }


    private void SetNewPatrolPoint()
    {
        Vector2 random = Random.insideUnitCircle * waypoint_radius;//get random point in circle
        Vector3 random_waypoint = new Vector3(random.x, transform.position.y, random.y);//convert to vec3

        waypoint = transform.position+ transform.forward * distance_between_waypoints +  random_waypoint;//put rand point in front of boat
        nav_mesh_agent.SetDestination(ValidDestination(waypoint));//set nav direction
    }


    private void Chase()
    {
        CheckTransitionToPatrol();

        if (captain_faction == Faction.NAVY)//chase if navy
        {
            SetChaseTarget();
        }
        else if (captain_faction == Faction.CIVILIAN)//flee if civilian
        {
            SetFleeTarget();
        }  
    }


    private void CheckTransitionToPatrol()
    {
        if (closest_enemy == null)
        {
            current_state = AICaptainState.PATROL;//revert to patrol if no enemies
            return;
        }

        if ((transform.position - closest_enemy.transform.position).sqrMagnitude >
            chase_radius * chase_radius || closest_enemy.transform.parent == deck_volume)//chase when a player is near
        {
            nav_mesh_agent.speed = chase_speed;
            current_state = AICaptainState.PATROL;
        }
    }


    private void SetChaseTarget()
    {
        if (closest_enemy == null)
            return;

        if (closest_enemy.transform.parent == null)//boat must exist in order to chase
            return;

        if (closest_enemy.transform.parent == deck_volume)//disable movement if the player is on board
        {
            nav_mesh_agent.isStopped = true;
            return;
        }

        nav_mesh_agent.isStopped = false;

        Vector3 side_direction = DetermineApproachSide();
        Vector3 chase_target = closest_enemy.transform.parent.position + (side_direction * broad_side_distance);//use players parent whih should be boat
        nav_mesh_agent.SetDestination(ValidDestination(chase_target));
    }


    private Vector3 DetermineApproachSide()
    {
        bool right = CheckRelativeSide(closest_enemy.transform.parent.forward,
            transform.position - closest_enemy.transform.parent.position , closest_enemy.transform.parent.up);//check if to the left or right

        Vector3 side = closest_enemy.transform.right;
        if (!right)//if not right side set as enemys left
            side = -closest_enemy.transform.right;

        return side;
    }


    private Vector3 ValidDestination(Vector3 _desired_waypoint)
    {
        if (_desired_waypoint.sqrMagnitude > GameManager.map_bound_radius * GameManager.map_bound_radius)
        {
            _desired_waypoint = _desired_waypoint.normalized * GameManager.map_bound_radius;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(_desired_waypoint, out hit, 100, NavMesh.AllAreas))
            return _desired_waypoint;

        NavMesh.FindClosestEdge(_desired_waypoint, out hit, NavMesh.AllAreas);
        return hit.position;
    }


    public bool CheckRelativeSide(Vector3 _forward, Vector3 _target_dir, Vector3 _up)
    {
        Vector3 perp = Vector3.Cross(_forward, _target_dir);
        float dir = Vector3.Dot(perp, _up);

        if (dir > 0.0f)//on right
            return true;

        if (dir < 0.0f)//on left
            return false;

        return true;//default right
    }


    private void SetFleeTarget()
    {
        if (closest_enemy == null)
            return;

        if (closest_enemy.transform.parent == deck_volume)//disable movement if the player is on board
        {
            current_state = AICaptainState.PATROL;
            return;
        }

        Vector3 flee_target = (transform.position - closest_enemy.transform.position).normalized *
            distance_between_waypoints;//set waypoint in oposite direction to enemy
        nav_mesh_agent.SetDestination(ValidDestination(flee_target));
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 next_waypoint_pos = transform.position + transform.forward * distance_between_waypoints;
        Gizmos.DrawWireSphere(next_waypoint_pos, waypoint_radius);//draw patrol waypoint selection area

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chase_radius);//draw chase trigger radius

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(waypoint, .5f);//draw current waypoint
    }

}

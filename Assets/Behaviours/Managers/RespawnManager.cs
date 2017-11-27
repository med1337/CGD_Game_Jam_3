using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public List<PlayerControl> alive_players = new List<PlayerControl>();

    [SerializeField] GameObject player_prefab;
    [SerializeField] Transform respawn_point;
    [SerializeField] Vector3 respawn_offset;


    void Start()
    {

    }


    void Update()
    {
        alive_players.RemoveAll(elem => elem == null);

        RespawnPlayers();
    }


    void RespawnPlayers()
    {
        foreach (ConnectedPlayer player in PlayerManager.players)
        {
            if (PlayerAwaitingRespawn(player))
            {
                RespawnPlayer(player);
            }
        }
    }


    bool PlayerAwaitingRespawn(ConnectedPlayer _player)
    {
        return _player.state == PlayerState.PLAYING && _player.character == null;
    }


    void RespawnPlayer(ConnectedPlayer _player)
    {
        _player.character = CreatePlayer("Player" + _player.id.ToString(), _player.color);
        _player.character.input = _player.input;
    }


    PlayerControl CreatePlayer(string _name, Color _color)
    {
        // Create the player.
        PlayerControl player = Instantiate(player_prefab).GetComponent<PlayerControl>();

        // Position the player.
        GameObject[] spawn_positions = GameObject.FindGameObjectsWithTag("Spawn");
        Transform nearest = null;
        float distance = 10000;
        foreach (var spawn in spawn_positions)
        {
            float dist2 = Vector3.Distance(spawn.transform.position, player.transform.position);
            if (dist2 < distance)
            {
                nearest = spawn.transform;
                distance = dist2;
            }
        }

        Vector3 respawn_pos = nearest.position;
        //if (respawn_point != null)
        //    respawn_pos = respawn_point.position;

        respawn_pos += respawn_offset;

        player.transform.position = respawn_pos;

        // Add player to alive list.
        alive_players.Add(player);

        return player;
    }



}

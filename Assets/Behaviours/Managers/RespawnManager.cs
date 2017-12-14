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
        player.transform.position = respawn_point.position;

        player.relative_point = GameObject.Find("CameraMount").transform;

        // Add player to alive list.
        alive_players.Add(player);

        return player;
    }



}

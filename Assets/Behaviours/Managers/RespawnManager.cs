using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public List<PlayerControl> alive_players = new List<PlayerControl>();

    [SerializeField] GameObject player_prefab;


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
    }


    PlayerControl CreatePlayer(string _name, Color _color)
    {
        // Create the player.
        PlayerControl player = Instantiate(player_prefab).GetComponent<PlayerControl>();

        // Position the player.
        player.transform.position = new Vector3(0, 0, 0);

        // Configure the player.
        // TODO: ...

        alive_players.Add(player);

        return player;
    }



}

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

        Vector3 ray_pos = GameManager.scene.camera_manager.target_pos;
        ray_pos.y = 1000;

        RaycastHit hit;
        Physics.Raycast(ray_pos, -Vector3.up, out hit, Mathf.Infinity);

        Vector3 respawn_pos = hit.point + (Vector3.up * 5);
        player.transform.position = respawn_pos;

        // Add player to alive list.
        alive_players.Add(player);

        return player;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<int, ConnectedPlayer>.ValueCollection players { get { return player_dictionary.Values; } }
    public static int active_player_count;

    private static PlayerManager instance;
    private static Dictionary<int, ConnectedPlayer> player_dictionary = new Dictionary<int, ConnectedPlayer>();


    public static void IdleAllPlayers()
    {
        foreach (ConnectedPlayer player in player_dictionary.Values)
            player.state = PlayerState.WAITING;

        active_player_count = 0;
    }


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;

        // Subscribe to Rewired controller connection events.
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        // Add a new player for all connected joysticks at startup.
        AssignAllConnectedJoysticks();
    }


    void OnControllerConnected(ControllerStatusChangedEventArgs _args)
    {
        // Only process Joysticks.
        if (_args.controllerType != ControllerType.Joystick)
            return;

        AddPlayer(_args.controllerId);
    }


    void OnControllerDisconnected(ControllerStatusChangedEventArgs _args)
    {
        int controller_id = _args.controllerId;

        // Remove controller from the player it is attached to.
        ConnectedPlayer connected_player = player_dictionary[controller_id];
        connected_player.input.controllers.ClearAllControllers();

        // Clean up the player's attached USBCharacter if it exists.
        if (connected_player.character)
            PlayerLeave(controller_id);

        // Remove the player entry from the list.
        player_dictionary.Remove(controller_id);
    }


    void AssignAllConnectedJoysticks()
    {
        foreach (Joystick joystick in ReInput.controllers.Joysticks)
        {
            AddPlayer(joystick.id);
        }
    }


    void AddPlayer(int _controller_id)
    {
        ConnectedPlayer connected_player = new ConnectedPlayer();

        connected_player.input = ReInput.players.GetPlayer(_controller_id);
        connected_player.input.controllers.AddController(ReInput.controllers.Joysticks[_controller_id], true);

        player_dictionary.Add(_controller_id, connected_player);
    }


    void Update()
    {
        HandlePlayers();
    }


    // Tick players and handle drop-in / drop-out.
    void HandlePlayers()
    {
        foreach (ConnectedPlayer player in player_dictionary.Values)
        {
            player.Update();

            if (player.state == PlayerState.JOINING)
            {
                PlayerJoin(player.id);
            }
            else if (player.state == PlayerState.LEAVING)
            {
                PlayerLeave(player.id);
            }
        }
    }


    // Create a character for the player to control and set to playing state.
    void PlayerJoin(int _player_id)
    {
        ConnectedPlayer connected_player = player_dictionary[_player_id];
        connected_player.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        connected_player.state = PlayerState.PLAYING;
        ++active_player_count;
    }


    // Remove the player's attached character and set to waiting state.
    void PlayerLeave(int _player_id)
    {
        ConnectedPlayer connected_player = player_dictionary[_player_id];

        if (connected_player.character != null)
            Destroy(connected_player.character.gameObject);

        connected_player.state = PlayerState.WAITING;
        --active_player_count;
    }

}

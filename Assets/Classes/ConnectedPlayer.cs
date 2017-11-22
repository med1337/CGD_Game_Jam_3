using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public enum PlayerState
{
    WAITING,
    JOINING,
    PLAYING,
    LEAVING
}

public class ConnectedPlayer
{
    public Player input;
    public int id { get { return input.id; } }
    public PlayerState state = PlayerState.WAITING;
    public PlayerControl character;
    public Color color;

    private float horizontal;
    private float vertical;


    public void Update()
    {
        HandleDropIn();

        if (character != null)
            ControlCharacter();
    }


    void HandleDropIn()
    {
		if (input.GetButtonDown("DropIn"))
        {
            if (state == PlayerState.WAITING)
            {
                state = PlayerState.JOINING;
            }
            else if (state == PlayerState.PLAYING)
            {
                state = PlayerState.LEAVING;
            }
        }
    }


    void ControlCharacter()
    {
        horizontal = input.GetAxis("Horizontal");
        vertical = input.GetAxis("Vertical");

        character.Move(new Vector3(horizontal, 0, vertical));

        CharacterControl();
    }


    void CharacterControl()
    {
        if (input.GetButtonDown("Interact"))
            character.OnInteract();
    }

}

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


    public void Update()
    {
        HandleDropIn();
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

}

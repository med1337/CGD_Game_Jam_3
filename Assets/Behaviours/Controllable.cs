using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controllable : MonoBehaviour
{
    public bool ai_controlling = false;
    public bool being_controlled { get { return controlling_player != null || ai_controlling; } }
    public PlayerControl controlling_player { get; private set; }

    protected Vector3 move_dir;

    public abstract void Move(Vector3 _dir);
    public virtual void Accelerate(Vector2 _acc) {}

    public virtual void OnControlStart(PlayerControl _player) { controlling_player = _player; }
    public virtual void OnControlEnd() { controlling_player = null; }

    public virtual void Activate() {}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controllable : MonoBehaviour
{
    protected Vector3 move_dir;

    public abstract void Move(Vector3 _dir, Vector2 accVector2);

    public virtual void OnControlStart(PlayerControl _player) {}
    public virtual void OnControlEnd() {}

    public virtual void Activate() {}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controllable : MonoBehaviour
{
    protected Vector3 move_dir;

    public abstract void Move(Vector3 _dir);
    public abstract void OnInteract();
}

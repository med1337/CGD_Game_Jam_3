using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mount : MonoBehaviour
{
    public bool occupied = false;
    public Controllable controllable { get { return controllable_; } }

    [SerializeField] Controllable controllable_;
}

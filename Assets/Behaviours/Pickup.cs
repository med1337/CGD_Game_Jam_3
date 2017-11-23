using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool in_use;

    public void SetInUse(bool _in_use)
    {
        in_use = _in_use;
    }


    public bool GetInUse()
    {
        return in_use;
    }
}

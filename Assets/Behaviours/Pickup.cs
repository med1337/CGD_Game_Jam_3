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


    void OnTriggerStay(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            transform.SetParent(_other.transform);
        }
    }


    void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Deck"))
        {
            transform.SetParent(null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class Station : MonoBehaviour
{
    public bool outline_enabled
    {
        get
        {
            return outlines.TrueForAll(elem => elem.enabled);
        }

        set
        {
            outlines.ForEach(elem => elem.enabled = value);
        }
    }

    public bool occupied = false;
    public Controllable controllable { get { return controllable_; } }

    [SerializeField] Controllable controllable_;
    [SerializeField] List<Outline> outlines;

}

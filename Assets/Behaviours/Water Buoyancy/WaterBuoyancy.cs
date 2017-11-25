using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBuoyancy : MonoBehaviour
{
    [SerializeField] private float buoyancy_force = 30;
    [SerializeField] private float water_viscosity = 2;

    private void OnTriggerStay(Collider _other)
    {
        BuoyantObject buoyant_object = _other.GetComponent<BuoyantObject>() ?? _other.GetComponentInParent<BuoyantObject>();

        if (buoyant_object == null)
            return;

        if (buoyant_object.target_rigidbody == null)
            return;

        Vector3 buoyent_force = Vector3.up * buoyancy_force;//go up...
        buoyant_object.target_rigidbody.AddForce(buoyent_force);

        Vector3 drag_force = buoyant_object.target_rigidbody.velocity * -1 * water_viscosity;//invert velocity and scale by viscosity
        buoyant_object.target_rigidbody.AddForce(drag_force);

        buoyant_object.in_water = true;
    }


    void OnTriggerExit(Collider _other)
    {
        BuoyantObject buoyant_object = _other.GetComponent<BuoyantObject>() ??
            _other.GetComponentInParent<BuoyantObject>();

        if (buoyant_object == null)
            return;

        buoyant_object.in_water = false;
    }

}

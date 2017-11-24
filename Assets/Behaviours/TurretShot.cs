using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShot : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float travel_speed;
    [SerializeField] GameObject ricochet_prefab;

    
    void Start()
    {

    }


    void Update()
    {
        Vector3 prev_pos = transform.position;

        transform.position += transform.forward * travel_speed * Time.deltaTime;

        Vector3 current_pos = transform.position;

        HitCheck(prev_pos, current_pos);
    }


    void HitCheck(Vector3 _prev_pos, Vector3 _current_pos)
    {
        Vector3 diff = (_current_pos - _prev_pos);
        RaycastHit hit;
        Physics.Raycast(_prev_pos, diff.normalized, out hit, diff.magnitude);

        if (hit.collider == null)
            return;

        LifeForce life = hit.collider.GetComponent<LifeForce>();
        if (life == null)
        {
            if (hit.collider.transform.parent != null)
            {
                life = hit.collider.GetComponentInParent<LifeForce>();

                if (life == null)
                {
                    if (hit.collider.transform.parent.parent != null)
                        life = hit.collider.transform.parent.GetComponentInParent<LifeForce>();
                }
            }
        }

        if (life == null)
            return;

        life.Damage(0);

        GameObject particle_clone = Instantiate(ricochet_prefab, hit.point,
            Quaternion.LookRotation(hit.normal));

        Destroy(this.gameObject);
    }

}

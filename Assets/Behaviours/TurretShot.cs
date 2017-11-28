using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShot : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float travel_speed;
    [SerializeField] GameObject ricochet_prefab;
    [SerializeField] int damage = 5;
    [SerializeField] LayerMask hit_layers;

    
    void Start()
    {

    }


    void FixedUpdate()
    {
        Vector3 prev_pos = transform.position;

        transform.position += transform.forward * travel_speed * Time.fixedDeltaTime;

        Vector3 current_pos = transform.position;

        HitCheck(prev_pos, current_pos);
    }


    void HitCheck(Vector3 _prev_pos, Vector3 _current_pos)
    {
        Vector3 diff = (_current_pos - _prev_pos);
        RaycastHit hit;
        Physics.Raycast(_prev_pos, diff.normalized, out hit, diff.magnitude, hit_layers);

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

        life.Damage(damage);

        GameObject particle_clone = Instantiate(ricochet_prefab, hit.point,
            Quaternion.LookRotation(hit.normal));

        AudioManager.PlayOneShot("ricochet");

        Destroy(this.gameObject);
    }

}

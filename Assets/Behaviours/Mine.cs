using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] GameObject explosion;

    void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Mine") && other.name != "Water With Buoyency" && other.name != "Terrain")
        {
            Debug.Log(other.gameObject);
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
            Vector3 pos = GetComponentInParent<Transform>().position;
            Instantiate(explosion, pos, explosion.transform.rotation);
            Destroy(this.transform.parent.gameObject, 0.2f);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] GameObject explosion;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Deck"))
        {
            Vector3 pos = GetComponentInParent<Transform>().position;

            GameObject clone = Instantiate(explosion, pos, Quaternion.identity);
            clone.GetComponent<CannonShot>().ManualDetonation();

            Destroy(this.transform.parent.gameObject);
        }
    }
}


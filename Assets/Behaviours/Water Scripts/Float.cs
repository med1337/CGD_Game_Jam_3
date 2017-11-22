using UnityEngine;
using System.Collections;

public class Float : MonoBehaviour
{
    private float waterLevel = 1.0f;
    private float floatHeight = 2.0f;
    private float bounceDamp = 0.1f;

    private float forceFactor;
    private Vector3 actionPoint;
    private Vector3 uplift;
    Vector3 buoyancyCentreOffset;

    void Update()
    {
        actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
        forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);

        if (forceFactor > 0f)
        {
            uplift = -Physics.gravity * (forceFactor - GetComponent<Rigidbody>().velocity.y * bounceDamp);
            GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dingy : MonoBehaviour
{
    [SerializeField] Transform deck_transform;

    private const float CLEANUP_AFTER = 10;
    private float cleanup_timer;

    void Update()
    {
        if (deck_transform.childCount > 0)
        {
            cleanup_timer = 0;
        }
        else
        {
            HandleCleanUp();
        }
    }


    void HandleCleanUp()
    {
        CameraManager cam = GameManager.scene.camera_manager;
        Vector3 cam_target_pos = cam.target_pos;
        float sqr_mag = Vector3.SqrMagnitude(transform.position - cam_target_pos);

        float cam_y_sqr = cam.transform.position.y * cam.transform.position.y;
        if (sqr_mag <= cam_y_sqr)
        {
            cleanup_timer = 0;
            return;
        }

        float prev_timer = cleanup_timer;
        cleanup_timer += Time.deltaTime;

        if (prev_timer < CLEANUP_AFTER && cleanup_timer >= CLEANUP_AFTER)
        {
            Destroy(GetComponent<Collider>());
            Destroy(this.gameObject, 5);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraUpdateMode
{
    DELTA,
    FIXED_DELTA
}

public struct CameraSettings
{
    public Transform target;
    public Vector3 target_pos;
    public float target_zoom;
    public CameraUpdateMode update_mode;
}

public class CameraManager : MonoBehaviour
{
    [HideInInspector] public float original_zoom;

    [Header("Parameters")]
    public Vector3 offset;
    public CameraUpdateMode update_mode;
    [SerializeField] float lerp_speed;
    [SerializeField] float zoom_speed;

    [Header("References")]
    [SerializeField] Camera cam;

    public Vector3 target_pos { get; private set; }
    private float target_zoom;
    private Transform spawn_point;


    public void SetTarget(Vector3 _target, float _zoom)
    {
        target_pos = _target;
        target_zoom = _zoom;
    }


    public void SetTarget(Vector3 _target)
    {
        SetTarget(_target, target_zoom);
    }


    void Start()
    {
        target_zoom = cam.orthographicSize;
        original_zoom = target_zoom;
        spawn_point = GameObject.FindGameObjectWithTag("Spawn").transform;
    }


    void Update()
    {
        target_zoom = Mathf.Clamp(target_zoom, 0, 100);
        UpdateZoom();

        if (update_mode == CameraUpdateMode.DELTA)
        {
            SetTarget(CalculateAveragePos());
            transform.LookAt(target_pos);

            UpdatePosition();
        }
    }

    
    void FixedUpdate()
    {
        if (update_mode == CameraUpdateMode.FIXED_DELTA)
        {
            SetTarget(CalculateAveragePos());
            transform.LookAt(target_pos);

            UpdatePosition();
        }
    }


    Vector3 CalculateAveragePos()
    {
        Vector3 avg_pos = new Vector3();

        var players = GameManager.scene.respawn_manager.alive_players;
        if (players.Count > 0)
        {
            foreach (var player in players)
            {
                if (player == null)
                    continue;

                avg_pos = player.transform.position;
                break;
            }

            for (int i = 1; i < players.Count; ++i)
            {
                avg_pos += players[i].transform.position;
            }

            avg_pos /= players.Count;
        }
        else
        {
            return spawn_point.position;
        }

        

        return avg_pos;
    }


    void UpdatePosition()
    {
        target_pos += offset;

        transform.position = Vector3.Lerp(transform.position, target_pos,
            lerp_speed * GetCurrentDelta());
    }


    void UpdateZoom()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,
            target_zoom, zoom_speed * GetCurrentDelta());
    }


    float GetCurrentDelta()
    {
        return update_mode == CameraUpdateMode.DELTA ?
            Time.unscaledDeltaTime : Time.fixedUnscaledDeltaTime;
    }

}

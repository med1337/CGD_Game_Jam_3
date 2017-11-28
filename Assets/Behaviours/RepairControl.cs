using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class RepairControl : Controllable
{
    [Header("Parameters")]
    [SerializeField] int repair_per_activate;

    [Space]
    [SerializeField] GameObject particle_prefab;

    [Space]
    [SerializeField] AudioClip repair_sound;
    [SerializeField] Transform repair_point;

    [Header("References")]
    [SerializeField] LifeForce attached_lifeforce;
    [SerializeField] GameObject canvas_obj;
    [SerializeField] Image health_bar;

    private bool can_activate = true;


    public override void Move(Vector3 _dir)
    {
        move_dir = _dir;
    }


    public override void OnControlStart(PlayerControl _player)
    {
        base.OnControlStart(_player);

        if (canvas_obj != null)
        {
            UpdateHealthBar();
            canvas_obj.SetActive(true);
        }
    }


    public override void OnControlEnd()
    {
        base.OnControlEnd();

        canvas_obj.SetActive(false);
    }


    public override void Activate()
    {
        if (!can_activate)
            return;

        can_activate = false;

        if (attached_lifeforce != null)
            Repair();
    }


    public override void Stop()
    {
        can_activate = true;
    }


    void Start()
    {

    }


    void Update()
    {
        if (being_controlled)
            ControlUpdate();
    }


    void ControlUpdate()
    {

    }


    void Repair()
    {
        attached_lifeforce.Heal(repair_per_activate);
        UpdateHealthBar();

        if (particle_prefab != null)
            Instantiate(particle_prefab, repair_point.position + Vector3.up, Quaternion.identity);

        AudioManager.PlayOneShot(repair_sound);
    }


    void UpdateHealthBar()
    {
        health_bar.fillAmount = attached_lifeforce.GetHealthPercentage();
    }

}

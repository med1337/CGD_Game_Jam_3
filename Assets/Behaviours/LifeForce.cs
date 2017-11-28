using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class LifeForce : MonoBehaviour
{
    [System.Serializable]
    private struct Threshold
    {
        public float threshold_percentage;
        public UnityEvent threshold_trigger_event;
    }


    [SerializeField] private int max_health = 100;
    [SerializeField] private List<Threshold> thresholds = new List<Threshold>();

    public CustomEvents.GameObjectEvent on_death_event;
    public CustomEvents.IntEvent on_damage_event;
    public CustomEvents.IntEvent on_health_changed_event;

    public int current_health { get; private set;}


    public float GetHealthPercentage()
    {
        return (float)current_health / (float)max_health;
    }


    void Start()
    {
        ResetHealth();
    }


    public void Damage(int _damage)
    {
        if (current_health <= 0)
            return;

        int prev_health = current_health;

        current_health -= _damage;//damage health
        current_health = Mathf.Clamp(current_health, 0, int.MaxValue);
        on_health_changed_event.Invoke(current_health);

        thresholds.ForEach(t => CheckThresholdTriggered(t, prev_health));

        if (current_health > 0)
        {
            on_damage_event.Invoke(_damage);//trigger damage event if survived
        }
        else
        {
            on_death_event.Invoke(gameObject);//trigger death event
        }
    }


    void CheckThresholdTriggered(Threshold _threshold, int _prev_health)
    {
        float health_threshold = _threshold.threshold_percentage * max_health;
        if (_prev_health >=  health_threshold && current_health < health_threshold)
            _threshold.threshold_trigger_event.Invoke();
    }


    public void Heal(int _heal_amount)
    {
        current_health += _heal_amount;
        current_health = Mathf.Clamp(current_health, 0, max_health);//clamp to max value
    }


    public void ResetHealth()
    {
        current_health = max_health;
    }


    public void SetMaxHealth(int _max_health, bool _update_current_health = true)
    {
        max_health = _max_health;

        if (!_update_current_health)
            return;

        ResetHealth();//update current health if specified
    }

}

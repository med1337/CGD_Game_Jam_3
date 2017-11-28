using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] int starting_cash = 1000;

    [Space]
    [SerializeField] float collection_delay = 60;
    [SerializeField] int collection_amount = 500;

    [Space]
    [SerializeField] UnityEvent score_gained_events;
    [SerializeField] UnityEvent bankrupt_events;

    [Header("References")]
    [SerializeField] Text cash_display;
    [SerializeField] Text time_display;
    [SerializeField] Text collection_amount_display;
    [SerializeField] Text next_collection_display;

    private int current_cash;
    public int accumulated_cash { get; private set; }

    private float play_time;
    private float collection_countdown;


    public void InsertDoubloon()
    {
        current_cash = starting_cash;
        collection_countdown = collection_delay;

        cash_display.text = current_cash.ToString();
        collection_amount_display.text = "$" + collection_amount.ToString();
    }


    public string GetTimePlayedString()
    {
        return time_display.text;
    }


    public void IncreaseScore(int _amount)
    {
        current_cash += _amount;
        accumulated_cash += _amount;

        cash_display.text = current_cash.ToString();
        
        score_gained_events.Invoke();
    }


    void Start()
    {
        InsertDoubloon();
    }


    void Update()
    {
        play_time += Time.deltaTime;
        collection_countdown -= Time.deltaTime;

        UpdateTimerDisplay(play_time, time_display);
        UpdateTimerDisplay(collection_countdown, next_collection_display);

        if (collection_countdown <= 0)
        {
            collection_countdown = collection_delay;
            collection_amount_display.text = "$" + collection_amount.ToString(); // in case we change the collection amount.

            CollectPayment();
        }
    }


    void UpdateTimerDisplay(float _f, Text _display)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(_f);
        _display.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }


    void CollectPayment()
    {
        current_cash -= collection_amount;
        cash_display.text = current_cash.ToString();

        if (current_cash <= 0)
        {
            bankrupt_events.Invoke();
        }
    }

}

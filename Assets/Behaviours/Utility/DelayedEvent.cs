using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField] float invoke_delay = 1;
    [SerializeField] UnityEvent on_delay_end;


	void Start ()
    {
		Invoke("TriggerEvent", invoke_delay);
	}


    void TriggerEvent()
    {
        on_delay_end.Invoke();
    }

}

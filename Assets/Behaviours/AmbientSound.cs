using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour {

    private float seagull_timer = 0;
    private float seagull_play_timer = 10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        seagull_timer += Time.deltaTime;

        if (seagull_timer >= seagull_play_timer)
        {
            seagull_timer = 0.0f;

            seagull_play_timer = Random.Range(20.0f, 30.0f);

            int sound_choice = Random.Range(1, 3);

            switch(sound_choice)
            {
                case 1:
                    AudioManager.PlayOneShot("Seagull");
                    break;
                case 2:
                    AudioManager.PlayOneShot("Seagull2");
                    break;
                case 3:
                    AudioManager.PlayOneShot("Seagull3");
                    break;
                default:
                    AudioManager.PlayOneShot("Seagull");
                    break;
            }
        }
		
	}
}

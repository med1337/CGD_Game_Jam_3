using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("joystick button 7"))
        {
            SceneManager.LoadScene("Main");
        }
        if (Input.GetKeyDown("joystick button 8") && (Input.GetKeyDown("joystick button 9")))
        {
            SceneManager.LoadScene("Elliott's Sharknado");
        }
	}
}

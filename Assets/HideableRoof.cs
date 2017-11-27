using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableRoof : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hide()
    {
        Color c = GetComponent<MeshRenderer>().material.color;
        c.a = 0.15f;
        GetComponent<MeshRenderer>().material.color = c;
    }

    public void Show()
    {

        Color c = GetComponent<MeshRenderer>().material.color;
        c.a = 1.0f;
        GetComponent<MeshRenderer>().material.color = c;
    }

  
}

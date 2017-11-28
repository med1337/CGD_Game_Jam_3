using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoStats : MonoBehaviour {

    [SerializeField] int cargo_value = 10;

	// Use this for initialization
	void Start () {
        cargo_value = Random.Range(5, 25);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetValue()
    {
        return cargo_value;
    }
}

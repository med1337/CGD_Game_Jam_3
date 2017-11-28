using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoStats : MonoBehaviour {

    private int cargo_value = 10;
    [SerializeField] int min_value = 5;
    [SerializeField] int max_value = 25;

    // Use this for initialization
    void Start () {
        cargo_value = Random.Range(min_value, max_value);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetValue()
    {
        return cargo_value;
    }
}

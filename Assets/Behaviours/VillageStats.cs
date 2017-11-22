using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageStats : MonoBehaviour {

    private float village_money = 150;  //currency
    private float village_population = 500; //villagers
    private float village_happiness = 30;   //approval rating 

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetMoney()
    {
        return village_money;
    }

    public void SetMoney(float money)
    {
        village_money += money;
    }

    public float GetPopulation()
    {
        return village_population;
    }

    public void SetPopulation(float population)
    {
        village_population += population;
    }

    public float GetHappiness()
    {
        return village_happiness;
    }

    public void SetHappiness(float happiness)
    {
        village_happiness += happiness;
    }
}

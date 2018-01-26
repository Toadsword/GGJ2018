using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call : MonoBehaviour
{
    float randomCountDown = Random.Range(0, 60);

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    public float Timers()
    {
        randomCountDown -= Time.deltaTime;

        return randomCountDown;
    }
}

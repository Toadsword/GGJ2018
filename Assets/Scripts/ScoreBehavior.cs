using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBehavior : MonoBehaviour {
    
    float timer_destruction=0.8f;
    Vector3 direction;
	// Use this for initialization
	void Start () {
		direction = new Vector3(Random.Range(-300,300)/100.0f, Random.Range(-300,300)/100.0f);
        direction = direction.normalized*10.0f;
	}
	
	// Update is called once per frame
	void Update () {
		//déplacement
        transform.position += direction;

        direction = direction*0.9f;

        Destroy(gameObject, timer_destruction);
	}
}

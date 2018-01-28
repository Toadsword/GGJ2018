using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloBehavior : MonoBehaviour {

    private Vector2 startScale;
    private Vector2 endScale = new Vector2(2.0f, 2.0f);
    private float timeToFade = 0.5f;
    private float timePassedInTotal = 0.0f;

    private Vector2 deltaScalePerSec;
    private float deltaOpacityPerSec;

    private SpriteRenderer render;

	// Use this for initialization
	void Start ()
    {
        startScale = new Vector2(this.transform.localScale.x, this.transform.localScale.y);
        deltaScalePerSec = endScale - startScale * timeToFade;
        deltaOpacityPerSec = 1.0f / timeToFade;

        render = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        timePassedInTotal += Time.deltaTime;
        transform.localScale = startScale + deltaScalePerSec * timePassedInTotal;
            if(render.color.r == 1 && render.color.g==0 && render.color.b == 0)
                render.color -= new Color(0.0f,0.0f,0.0f,0.2f*deltaOpacityPerSec * Time.deltaTime);
            else
                render.color -= new Color(0.0f, 0.0f, 0.0f, deltaOpacityPerSec * Time.deltaTime);

        if(timeToFade <= timePassedInTotal)
        {
            Destroy(gameObject);
        }
    }
}

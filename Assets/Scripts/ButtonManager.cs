using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : Button {

    public Image lien;

    public bool buttonPressed = false;
    
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
		if(buttonPressed) {
        }
        else {
        }
	}

 
    public override void OnPointerDown(PointerEventData eventData){
        buttonPressed = true;
        /*Color coul = new Color(221/255.0f, 85/255.0f, 85/255.0f);
        transform.GetChild(1).GetComponent<Image>().color = coul;
        transform.GetChild(2).GetComponent<Image>().color = coul;
        GetComponent<Image>().color = coul;*/
        transform.GetChild(0).position -= new Vector3(0, 10, 0);
        transform.GetChild(1).position -= new Vector3(0, 10, 0);
        transform.GetChild(2).position -= new Vector3(0, 10, 0);

        FindObjectOfType<SoundManager>().PlaySound(SoundManager.SoundList.PLUG);

        base.OnPointerDown(eventData);
    }
 
    public override void OnPointerUp(PointerEventData eventData){
        buttonPressed = false;
        transform.GetChild(0).position += new Vector3(0, 10, 0);
        transform.GetChild(1).position += new Vector3(0, 10, 0);
        transform.GetChild(2).position += new Vector3(0, 10, 0);
        base.OnPointerUp(eventData);
    }
}

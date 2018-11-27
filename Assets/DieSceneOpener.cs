using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieSceneOpener : MonoBehaviour
{

    [SerializeField] private InputField nameInputField;

	// Use this for initialization
	void Start ()
	{
	    ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        Debug.Log(scoreManager);
	    if (scoreManager != null)
	        nameInputField.text = scoreManager.GetPlayerName();
	}
}

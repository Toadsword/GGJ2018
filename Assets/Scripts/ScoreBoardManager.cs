using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour {

    private GameObject playerRanksObjects;

    // Use this for initialization
    void Start ()
    {
        playerRanksObjects = FindObjectOfType<Canvas>().transform.Find("PlayerRanks").gameObject;

        for(int i = 1; i <= 10; i++)
        {
            string text = PlayerPrefs.GetString("Name" + i.ToString()) + " - " + PlayerPrefs.GetInt("Score" + i.ToString()).ToString();
            playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                .GetComponent<Text>()
                .text = text;
        }
    }
}

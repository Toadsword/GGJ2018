using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour
{

    [SerializeField] private Text levelText;
    private GameObject playerRanksObjects;

    // Use this for initialization
    void Start ()
    {
        playerRanksObjects = FindObjectOfType<Canvas>().transform.Find("PlayerRanks").gameObject;
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

        if (scoreManager != null)
        {
            int level = scoreManager.GetLevel();
            levelText.text = "level " + level;
            for (int i = 1; i <= 10; i++)
            {
                string text = PlayerPrefs.GetString(level+"Name" + i.ToString()) + " - " + PlayerPrefs.GetInt(level + "Score" + i.ToString()).ToString();
                playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = text;
            }
        }
    }
}

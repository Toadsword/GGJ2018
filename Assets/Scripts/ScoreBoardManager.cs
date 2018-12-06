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
            levelText.text = "L e v e l    " + level;
            for (int i = 1; i <= 10; i++)
            {
                string nom_joueur = PlayerPrefs.GetString(level + "Name" + i.ToString());
                string score_joueur = PlayerPrefs.GetInt(level + "Score" + i.ToString()).ToString();
                playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = nom_joueur;
                playerRanksObjects.transform.Find("PlayerScore" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = score_joueur;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

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

            WWWForm form = new WWWForm();
		    form.AddField("level", level);
		    WWW requete = new WWW("https://brandygonz12.alwaysdata.net/login.php", form);
		    StartCoroutine(getScoreBoard(requete));

            
            for(int i=1;i<=10;i++) {
                playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = "";
                playerRanksObjects.transform.Find("PlayerScore" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = "";
            }
        }
    }

    IEnumerator getScoreBoard (WWW requete){
		yield return requete;
		if (requete.error == null){


            playerRanksObjects = FindObjectOfType<Canvas>().transform.Find("PlayerRanks").gameObject;
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null)
            {
                int level = scoreManager.GetLevel();
                levelText.text = "L e v e l    " + level;

                Debug.Log( requete.text);

                string[] sepLigne = new string[] {"&;;&"};
                string[] sepJoueurScore = new string[] {"&==&"};

                string[] lignes = requete.text.Split(sepLigne, StringSplitOptions.None);

                for(int i=1;i<=10 && i<lignes.Length;i++) {
                    string[] joueurScore = lignes[i-1].Split(sepJoueurScore, StringSplitOptions.None);

                    string nom_joueur = joueurScore[0];
                    string score_joueur = joueurScore[1];
                    playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                        .GetComponent<Text>()
                        .text = nom_joueur;
                    playerRanksObjects.transform.Find("PlayerScore" + i.ToString()).gameObject
                        .GetComponent<Text>()
                        .text = score_joueur;
                }



                /* PLUS UTILES
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
                */

            }

            
		}
		else {
			Debug.Log("ERROR: " +requete.error);   
		}
	}
}

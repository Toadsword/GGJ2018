using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class ScoreBoardManager : MonoBehaviour
{

    [SerializeField] private Text levelText;
    [SerializeField] GameObject playerRanksObjects;

    public GameObject AffichageOnLine;
    public GameObject AffichageOffLine;

    public Text scoreGO;
    public Text recordGO;

    string playerName;
    double scoreMade;

    // Use this for initialization
    void Start ()
    {

        AffichageOffLine.gameObject.SetActive(false);
        AffichageOnLine.gameObject.SetActive(true);

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

        playerName = scoreManager.GetPlayerName();
        scoreMade = scoreManager.GetScore();

        if (scoreManager != null)
        {
            int level = scoreManager.GetLevel();

            WWWForm form = new WWWForm();
		    form.AddField("level", level);
		    WWW requete = new WWW("https://brandygonz12.alwaysdata.net/limited_connection/login.php", form);
		    StartCoroutine(getScoreBoard(requete));

            
            for(int i=1;i<=10;i++) {
                playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = "";
                playerRanksObjects.transform.Find("PlayerScore" + i.ToString()).gameObject
                    .GetComponent<Text>()
                    .text = "";

                    
                        playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                            .GetComponent<Text>()
                            .color = new Color(1,1,1);
                        playerRanksObjects.transform.Find("PlayerScore" + i.ToString()).gameObject
                            .GetComponent<Text>()
                            .color = new Color(1,1,1);
                        playerRanksObjects.transform.Find("PlayerIndicator" + i.ToString()).gameObject
                            .GetComponent<Text>()
                            .color = new Color(1,1,1);
            }
        }
    }

    IEnumerator getScoreBoard (WWW requete){
		yield return requete;
		if (requete.error == null){
        
            AffichageOffLine.SetActive(false);
            AffichageOnLine.SetActive(true);
            
            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null)
            {
                int level = scoreManager.GetLevel();
                levelText.text = "L e v e l    " + level;

                Debug.Log( requete.text);

                string[] sepLigne = new string[] {"&;;&"};
                string[] sepJoueurScore = new string[] {"&==&"};

                string[] lignes = requete.text.Split(sepLigne, StringSplitOptions.None);

                bool premiereCorrespondance = true;

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

                    if(score_joueur == (scoreMade+"") && playerName.ToUpper() == nom_joueur && premiereCorrespondance) {
                        playerRanksObjects.transform.Find("PlayerRank" + i.ToString()).gameObject
                            .GetComponent<Text>()
                            .color = new Color(145/255.0f, 245/255.0f, 190/255.0f);
                        playerRanksObjects.transform.Find("PlayerScore" + i.ToString()).gameObject
                            .GetComponent<Text>()
                            .color = new Color(145/255.0f, 245/255.0f, 190/255.0f);
                        playerRanksObjects.transform.Find("PlayerIndicator" + i.ToString()).gameObject
                            .GetComponent<Text>()
                            .color = new Color(145/255.0f, 245/255.0f, 190/255.0f);
                        
                        premiereCorrespondance = false;
                    }
                }

                //quand même enregistrer le record personnel
                if (!PlayerPrefs.HasKey("record" + level)) {
                    PlayerPrefs.SetInt("record" + level, (int)scoreMade);
                } else {
                    int rec = PlayerPrefs.GetInt("record" + level);

                    if (rec < scoreMade) {
                        PlayerPrefs.SetInt("record" + level, (int)scoreMade);
                    }
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

            AffichageOffLine.SetActive(true);
            AffichageOnLine.SetActive(false);


            ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

            if (scoreManager != null) {
                int level = scoreManager.GetLevel();

                scoreGO.text = scoreMade + "";

                if (!PlayerPrefs.HasKey("record"+level)) {
                    recordGO.text = "-";
                    PlayerPrefs.SetInt("record"+level, (int)scoreMade);
                } else {
                    int rec = PlayerPrefs.GetInt("record"+level);

                    if (rec < scoreMade) {
                        PlayerPrefs.SetInt("record"+level, (int)scoreMade);
                    }
                    recordGO.text = rec + "";
                }
            }

		}
	}
}

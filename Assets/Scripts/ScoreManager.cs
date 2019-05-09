using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    private static ScoreManager ScoreManagerInstance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (ScoreManagerInstance == null)
        {
            ScoreManagerInstance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }

        //récupérer pseudo enregistré
        if (PlayerPrefs.HasKey("pseudo")) {
            playerName = PlayerPrefs.GetString("pseudo");
        }
    }

    [SerializeField] private string playerName = "Billy";
    [SerializeField] private int scoreMade = 0;
    [SerializeField] private int level = 0;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public void SaveScore()
    {
        //Get name of player
        playerName = FindObjectOfType<Canvas>().transform.GetChild(1).transform.Find("Text").GetComponent<Text>().text;
        PlayerPrefs.SetString("pseudo", playerName);
        

        Debug.Log(playerName);
        if (level == 0)
            return;

        if (!PlayerPrefs.HasKey(level + "Name1"))
        {
            InitScore();
        }

        // Parcours chaque ocurence du tableau des scores
        int indexToReplace = 0;
        for (int i = 1; i <= 10; i++)
        {
            Debug.Log(PlayerPrefs.GetInt(level + "Score" + i.ToString()));
            if (PlayerPrefs.GetInt(level + "Score" + i.ToString()) < scoreMade)
            {
                Debug.Log("On remplace !");
                indexToReplace = i;
                break;
            }
        }
        Debug.Log(indexToReplace);
        if (indexToReplace != 0)
        {
            for (int j = 10; j >= indexToReplace; j--)
            {
                PlayerPrefs.SetInt(level + "Score" + j.ToString(), PlayerPrefs.GetInt(level + "Score" + (j - 1).ToString()));
                PlayerPrefs.SetString(level + "Name" + j.ToString(), PlayerPrefs.GetString(level + "Name" + (j - 1).ToString()));
            }
            PlayerPrefs.SetInt(level + "Score" + indexToReplace.ToString(), scoreMade);
            PlayerPrefs.SetString(level + "Name" + indexToReplace.ToString(), playerName);
        }

        //pour insérer (sauver) un nouveau score
        WWWForm form = new WWWForm();
			form.AddField("pseudo", playerName);
			form.AddField("score", scoreMade);
			form.AddField("level", level);
		WWW requete = new WWW("https://brandygonz12.alwaysdata.net/save_score.php", form);
		StartCoroutine(insertScore(requete));
    }

    private void InitScore()
    {
        PlayerPrefs.SetString(level + "Name1", "Toadysword");
        PlayerPrefs.SetString(level + "Name2", "Fuhria");
        PlayerPrefs.SetString(level + "Name3", "Brand12");
        PlayerPrefs.SetString(level + "Name4", "Surue");
        PlayerPrefs.SetString(level + "Name5", "Echo123");
        PlayerPrefs.SetString(level + "Name6", "Le T Froa");
        PlayerPrefs.SetString(level + "Name7", "Léon");
        PlayerPrefs.SetString(level + "Name8", "Gaëlle");
        PlayerPrefs.SetString(level + "Name9", "Jordan");
        PlayerPrefs.SetString(level + "Name10", "Internet Explorer");

        PlayerPrefs.SetInt(level + "Score1", 134);
        PlayerPrefs.SetInt(level + "Score2", 121);
        PlayerPrefs.SetInt(level + "Score3", 120);
        PlayerPrefs.SetInt(level + "Score4", 90);
        PlayerPrefs.SetInt(level + "Score5", 70);
        PlayerPrefs.SetInt(level + "Score6", 55);
        PlayerPrefs.SetInt(level + "Score7", 43);
        PlayerPrefs.SetInt(level + "Score8", 13);
        PlayerPrefs.SetInt(level + "Score9", 9);
        PlayerPrefs.SetInt(level + "Score10", 5);
    }

    public void UpdateScore(int newScore)
    {
        scoreMade = newScore;
    }

    public int GetScore()
    {
        return scoreMade;
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }

    public int GetLevel()
    {
        return level;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    IEnumerator insertScore (WWW requete){
		yield return requete;

		if (requete.error == null){

            Debug.Log("INSERT REUSSI");
            Debug.Log(requete.text);

            
		}
		else {
			Debug.Log("ERROR: " +requete.error);   
		}
        FindObjectOfType<OSceneManager>().ChangeScene(OSceneManager.SceneNames.SCOREBOARD);
	}
}

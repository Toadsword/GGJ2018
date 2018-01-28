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
    }

    [SerializeField] private string playerName = "Billy";
    [SerializeField] private int scoreMade = 0;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public void SaveScore()
    {
        //Get name of player
        playerName = FindObjectOfType<Canvas>().transform.GetChild(1).transform.Find("Text").GetComponent<Text>().text;
        Debug.Log(playerName);

        if (!PlayerPrefs.HasKey("Name1"))
        {
            InitScore();
        }

        // Parcours chaque ocurence du tableau des scores
        int indexToReplace = 0;
        bool getOut = false;
        for (int i = 1; i <= 10 && getOut == false; i++)
        {
            Debug.Log(PlayerPrefs.GetInt("Score" + i.ToString()));
            if (PlayerPrefs.GetInt("Score" + i.ToString()) < scoreMade)
            {
                Debug.Log("On remplace !");
                indexToReplace = i;
                getOut = true;
            }
        }
        Debug.Log(indexToReplace);
        if (indexToReplace != 0)
        {
            for (int j = 10; j >= indexToReplace; j--)
            {
                PlayerPrefs.SetInt("Score" + j.ToString(), PlayerPrefs.GetInt("Score" + (j - 1).ToString()));
                PlayerPrefs.SetString("Name" + j.ToString(), PlayerPrefs.GetString("Name" + (j - 1).ToString()));
            }
            PlayerPrefs.SetInt("Score" + indexToReplace.ToString(), scoreMade);
            PlayerPrefs.SetString("Name" + indexToReplace.ToString(), playerName);
        }
    }

    private void InitScore()
    {
        PlayerPrefs.SetString("Name1", "Toadysword");
        PlayerPrefs.SetString("Name2", "Fuhria");
        PlayerPrefs.SetString("Name3", "Brand12");
        PlayerPrefs.SetString("Name4", "Surue");
        PlayerPrefs.SetString("Name5", "Echo123");
        PlayerPrefs.SetString("Name6", "Le T Froa");
        PlayerPrefs.SetString("Name7", "Léon");
        PlayerPrefs.SetString("Name8", "Gaëlle");
        PlayerPrefs.SetString("Name9", "Jordan");
        PlayerPrefs.SetString("Name10", "Internet Explorer");

        PlayerPrefs.SetInt("Score1", 134);
        PlayerPrefs.SetInt("Score2", 121);
        PlayerPrefs.SetInt("Score3", 120);
        PlayerPrefs.SetInt("Score4", 90);
        PlayerPrefs.SetInt("Score5", 70);
        PlayerPrefs.SetInt("Score6", 55);
        PlayerPrefs.SetInt("Score7", 43);
        PlayerPrefs.SetInt("Score8", 13);
        PlayerPrefs.SetInt("Score9", 9);
        PlayerPrefs.SetInt("Score10", 5);
    }

    public void UpdateScore(int newScore)
    {
        scoreMade = newScore;
    }

    public int GetScore()
    {
        return scoreMade;
    }
}

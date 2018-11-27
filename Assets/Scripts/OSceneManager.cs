using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OSceneManager : MonoBehaviour {

    public enum SceneNames
    {
        MAIN_GAME,
        DIE_MENU_SCENE,
        SCOREBOARD
    }

    private ScoreManager scoreManager;

	// Use this for initialization
	void Start ()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }
	
    public void ChangeScene(SceneNames scene)
    {
        string nameScene = "";
        switch (scene)
        {
            case SceneNames.MAIN_GAME:
                nameScene = "MainGame";
                break;
            case SceneNames.DIE_MENU_SCENE:
                nameScene = "DieScene";
                break;
            case SceneNames.SCOREBOARD:
                nameScene = "Scoreboard";
                break;
        }
        Debug.Log(nameScene);
        if(nameScene != "")
            SceneManager.LoadScene(nameScene);
    }

    public void SaveScore()
    {
        scoreManager.SaveScore();
        ChangeScene(SceneNames.SCOREBOARD);
    }
    public void UpdateScore(int score)
    {
        scoreManager.UpdateScore(score);
    }

    public void UpdateLevel(int level)
    {
        scoreManager.SetLevel(level);
    }

    public void PlayGame()
    {
        ChangeScene(SceneNames.MAIN_GAME);
    }
    
    public void QuitGame()
    {
        //Debug.Log(""+GameObject.Find("GameManager").GetComponent<GameManager>().level);
        Application.Quit();
    }
}

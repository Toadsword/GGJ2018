using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call
{
    private static int ID=0;
    public int id{get;private set;}

    public Call(){
        id = ID;
        ID++;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Debug.Log("Create call");
    }

    //premier chrono en seconde,
    //temps avant que la personne abandonne l'appel (-1 vie)
    float randomCountDown = Random.Range(10, 20);

    public NodeController caller{get;set;}
    public NodeController reciever { get; set; }

    public enum Status{calling, inCall};
    public Status status{get;set;}

    GameManager gameManager;

    public float Timers()
    {
        randomCountDown -= Time.deltaTime;

        return randomCountDown;
    }

    public void SetInCall()
    {
        status = Status.inCall;

        //second chrono en seconde,
        //temps avant que la communication s'achève
        randomCountDown = Random.Range(10, 20);
    }

    public bool Update()
    {
        if(randomCountDown<0)
        {
            if(status == Status.inCall)
            {
                gameManager.Liberer(caller);
                gameManager.Liberer(reciever);
                gameManager.EndCall(true);
                return true;
            } 
            else if (status == Status.calling)
            {
                gameManager.Liberer(caller);
                gameManager.Liberer(reciever);
                gameManager.EndCall(false);
                return true;
            }
        }
        else
        {
            randomCountDown -= Time.deltaTime;
            Debug.Log(randomCountDown);
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    private int lives = 3;

    [SerializeField]
    Color[] colorArray;

    [SerializeField]
    List<GameObject> availableHosts;

    private List<GameObject> unavailableHosts;

    private int[] id;

    private void Start()
    {
        Call();
    }

    private void Call()
    {

        if (availableHosts.Count > 2)
        {
            int randomCaller = Random.Range(0, availableHosts.Count);
            int randomReciever = Random.Range(0, availableHosts.Count);

            while(randomCaller == randomReciever)
            {
                randomReciever = Random.Range(0, availableHosts.Count);
            }


            GameObject reciever = availableHosts[randomReciever];
            availableHosts.Remove(reciever);
            unavailableHosts.Add(reciever);

            GameObject caller = availableHosts[randomCaller];
            availableHosts.Remove(caller);
            unavailableHosts.Add(caller);
        }
    }

    private void EndCall(bool success)
    {
        if(success)
        {
            score++;
        }
        else
        {
            lives--;
        }      
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Call
{
    private static int ID=0;
    public int id{get;private set;}

    public bool isInfinite{get;private set;}

    public int size{get;private set;}//nb de edges 

    private GameObject timerTextGameObject;
    private Text timerText;

    private float timer_dialog=1.0f;

    //node which must be used to validate the transmission
    //if null, no node is obligatory
    public NodeController node_obligatory = null;


    public Call(bool isInf=false){
        id = ID;
        ID++;

        isInfinite = isInf;

        size = 1;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        randomCountDown = durationWaiting();
        previousCountDown =0;
    }

    //premier chrono en seconde,
    //temps avant que la personne abandonne l'appel (-1 vie)
    private float randomCountDown;
    private float previousCountDown;

    // Temps avant plusieurs appels d'halo
    private const float HALO_COUNTDOWN_BEFORE_NEW = 1.0f;
    private const float HALO_TIME_BETWEEN_TWO = 0.5f;
    private const float HALO_TIME_CONSECUTIVE = 0.1f;
    private const int HALO_COUNT = 3;

    private float HaloCountDownBeforeNew = 0.0f;
    private float HaloTimeBetweenTwo = HALO_TIME_BETWEEN_TWO;
    private float HaloTimeConsecutive = HALO_TIME_CONSECUTIVE;
    private int HaloCurrentCount = 0;

    private bool passedBetweenTwo = false;

    private GameObject messageBox;

    public NodeController caller{get;set;}
    public NodeController reciever { get; set; }

    public enum Status{calling, transmitting, inCall, interruptedCall};
    public Status status{get;set;}

    GameManager gameManager;

    public float Timers()
    {
        if(!isInfinite)
            randomCountDown -= Time.deltaTime;

        return randomCountDown;
    }

    public void SetInCall()
    {
        //second chrono en seconde,
        //temps avant que la communication s'achève
        if (status == Status.interruptedCall)
        {
            if (gameManager.Score() < 100)
                randomCountDown = previousCountDown + 3.0f;
            else if (gameManager.Score() < 300)
                randomCountDown = previousCountDown + 4.0f;
            else
                randomCountDown = previousCountDown + 5.0f;
        }
        else
            randomCountDown = durationCall();

        status = Status.inCall;
    }

    public bool Update(bool gameIsOver)
    {
        if(!gameIsOver) {
            if(timer_dialog>0 && status==Status.inCall && !isInfinite){
                timer_dialog -= Time.deltaTime;
                if(timer_dialog <= 0 && randomCountDown > 3.0f && !gameManager.pause){//si on doit lancer un dialogue et qu'on aura le temps de la play entier
                    gameManager.soundManager.PlaySound(SoundManager.SoundList.DIALOG);
                    timer_dialog = 5.0f;
                }
            }
            
            if(!isInfinite)
                randomCountDown -= Time.deltaTime;

            if (randomCountDown < 0.0f)
            {
                gameManager.EndCall(this);
                if(!gameManager.gameIsOver) {
                    caller.DisplayMessageBox(false);
                    gameManager.LibererDelivrer(caller);
                    gameManager.LibererDelivrer(reciever);
                    if(node_obligatory!=null) {
                        node_obligatory.ChangeColor(new Color(1.0f,1.0f,1.0f,1.0f));
                        node_obligatory.call = null;
                        node_obligatory.isUsed = false;
                    }
                }
                
                //Debug.Log(randomCountDown);
                caller.UpdateTimer(randomCountDown);
                return true;
                /*if(status == Status.inCall)
                   {
                       gameManager.LibererDelivrer(caller);
                       gameManager.LibererDelivrer(reciever);
                       gameManager.EndCall(true);
                       return true;
                   } 
                   else if (status == Status.calling || status == Status.interruptedCall)
                   {
                       gameManager.LibererDelivrer(caller);
                       gameManager.LibererDelivrer(reciever);
                       gameManager.EndCall(false);
                       return true;
                   }
               */
            }
            else
            {
                if (gameManager.ActualSource() == caller)
                {
                    if(node_obligatory!=null && !node_obligatory.isUsed) {
                        if (status == Status.interruptedCall)
                            HaloManager(node_obligatory, Color.red);
                        else
                            HaloManager(node_obligatory, Color.white);
                    }else {
                        if (status == Status.interruptedCall)
                            HaloManager(reciever, Color.red);
                        else
                            HaloManager(reciever, Color.white);
                    }
                    caller.DisplayMessageBox(false);
                }
                else if (status == Status.calling  || status == Status.interruptedCall)
                {
                    caller.DisplayMessageBox(true);
                    if(status == Status.interruptedCall)
                        HaloManager(caller,Color.red);
                    else
                        HaloManager(caller, Color.white);
                }

            }
            //Debug.Log(randomCountDown);
            caller.UpdateTimer(randomCountDown);
        }else {
            caller.UpdateTimer(0);
        }
        return false;
    }

    private void HaloManager(NodeController location, Color color)
    {
        float timePassed = Time.deltaTime;
        HaloCountDownBeforeNew -= timePassed;
        if (HaloCountDownBeforeNew < 0.0f)
        {
            HaloTimeConsecutive -= timePassed;
            // Alors on crée une vague de halo
            if (HALO_COUNT != HaloCurrentCount)
            {
                if(HaloTimeBetweenTwo < 0.0f)
                {
                    HaloTimeConsecutive = HALO_TIME_CONSECUTIVE;
                }
                else if(HaloTimeConsecutive < 0.0f)
                {
                    //Alors on crée un Halo
                    HaloCurrentCount += 1;
                    gameManager.InstantiateHalo(location, color);
                    HaloTimeConsecutive = HALO_TIME_CONSECUTIVE;
                }
            }
            else
            {
                HaloTimeBetweenTwo -= timePassed;
                if(HaloTimeBetweenTwo <= 0.0f)
                {
                    HaloTimeBetweenTwo = HALO_TIME_BETWEEN_TWO;
                    HaloCurrentCount = 0;
                    if (!passedBetweenTwo)
                    {
                        passedBetweenTwo = true;
                    }
                    else
                    {
                        passedBetweenTwo = false;
                        if(status == Status.interruptedCall){
                            HaloCountDownBeforeNew = HALO_COUNTDOWN_BEFORE_NEW/3.0f;
                        }else{
                            HaloCountDownBeforeNew = HALO_COUNTDOWN_BEFORE_NEW;
                        }
                    }
                }
            }
        }
    }

    public void setSize(int i)
    {
        size = i;
    }

    public void Interrupt()
    {
        //on enregistre la durée de la conversation qu'il restait.
        previousCountDown = randomCountDown;
        status = Status.interruptedCall;
        randomCountDown = durationWaitingInterrupted();
        caller.status=NodeController.Status.calling;
        reciever.status=NodeController.Status.waitingCall;
         
        //remettre le node obligatory comme non utilisé

        if (node_obligatory != null)
            node_obligatory.isUsed = false;
        HaloCountDownBeforeNew = 0;
        gameManager.soundManager.PlaySound(SoundManager.SoundList.DIALOG_FURIOUS);
        gameManager.ResetMultiplier();
    }

    public void Suppress() {
        caller.DisplayMessageBox(false);
        gameManager.LibererDelivrer(caller);
        gameManager.LibererDelivrer(reciever);
        //retirer la couleur de node obligatory
        
        if (node_obligatory != null) {
            node_obligatory.ChangeColor(new Color(1.0f,1.0f,1.0f,1.0f));
            node_obligatory.call = null;
            node_obligatory.isUsed = false;
            
        }
        caller.Suppress();
        reciever.Suppress();
    }

    private float durationWaiting()
    {
        if (gameManager.Score() < 30)
            return Random.Range(15, 20);
        else if(gameManager.Score()<100)
            return Random.Range(10,15);
        else if (gameManager.Score() < 200)
            return Random.Range(7,10);
        else
            return Random.Range(3, 5);
    }

    private float durationCall()
    {
        /*if(gameManager.Score()<30)
            return Random.Range(10, 15);
        else if(gameManager.Score()<100)
            return Random.Range(15, 25);
        else
            return Random.Range(20, 40);*/
        if (gameManager.Score() < 30)
            return Random.Range(10, 15);
        else if(gameManager.Score() < 100)
            return Random.Range(8, 12);
        else if (gameManager.Score() < 300)
            return Random.Range(10, 12);
        else
            return Random.Range(6, 8);

    }
    private float durationWaitingInterrupted()
    {
        if(gameManager.Score()<100)
            return Random.Range(7, 9);
        else if (gameManager.Score() < 300)
            return Random.Range(5,7);
        else
            return Random.Range(3, 5);

    }

    public float timer_call() {
        return randomCountDown;
    }
}

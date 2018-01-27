using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager:MonoBehaviour {
    private int score = 0;
    private int lives = 3;

    private Text scoreText;
    private Text livesText;

    [SerializeField]
    EdgeController edgeCursor;

    public Color[] colorArray;

    [SerializeField]
    List<NodeController> availableHosts;

    List<Call> callsInTransmission = new List<Call>();

    private List<NodeController> unavailableHosts = new List<NodeController>();

    //liste les nodes actuellement utilisés pour la transmission en cours
    [SerializeField]
    private List<NodeController> actualTrajectory = new List<NodeController>();

    private int[] id;

    float timerBeforeNextCall;

    private void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = "Score : " + score;
        livesText = GameObject.Find("LivesText").GetComponent<Text>();
        livesText.text = "Lives : " + lives;
        StartingCall();
    }

    void Update() {
        if(Input.GetMouseButtonUp(0)) {
            edgeCursor.gameObject.SetActive(false);
            //LACHER
            for(int i=0;i<actualTrajectory.Count-1;++i){
                actualTrajectory[i].edge(actualTrajectory[i+1]).ChangeColor(Color.black);
            }
            actualTrajectory.Clear();
        }

        if(actualTrajectory.Count>0){
            //afficher edgecurseur
            Vector3 n = Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10))-actualTrajectory[actualTrajectory.Count-1].transform.position;
            float distance = (n).magnitude;
            edgeCursor.gameObject.SetActive(true);
            float alpha = angle( n.x, n.y);

            edgeCursor.transform.position = (actualTrajectory[actualTrajectory.Count-1].transform.position+Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10)))/2.0f;
            edgeCursor.transform.localScale = new Vector3(distance*1/8.5f,0.6f,1);
            edgeCursor.transform.localEulerAngles = new Vector3(0,0,alpha);

            Call call =null;
            foreach(Call c in callsInTransmission){
                if(c.caller==actualTrajectory[0]){
                    call = c;
                }
            }

            edgeCursor.GetComponent<SpriteRenderer>().color = colorArray[call.id%10];
        }

        for (int i=0;i<callsInTransmission.Count;++i)
        {
            if(callsInTransmission[i].Update()){
                callsInTransmission.RemoveAt(i);
                i--;
            }
        }

        //actualisation timer next call
        timerBeforeNextCall -= Time.deltaTime;
        if(timerBeforeNextCall<0){
            StartingCall();
        }
    }

    private void StartingCall() {
        timerBeforeNextCall = Random.Range(5,10);

        if(availableHosts.Count >= 2 && callsInTransmission.Count<10) {
            Debug.Log("Au moins 2 travaillent");

            int randomCaller = Random.Range(0,availableHosts.Count);

            NodeController caller = availableHosts[randomCaller];
            availableHosts.Remove(caller);
            unavailableHosts.Add(caller);
            caller.status=NodeController.Status.calling;

            caller.GetComponent<SpriteRenderer>().color = Color.green;
            Debug.Log(caller.name + " is calling");

            int randomReciever = Random.Range(0, availableHosts.Count);

            NodeController reciever = availableHosts[randomReciever];
            availableHosts.Remove(reciever);
            unavailableHosts.Add(reciever);
            reciever.status = NodeController.Status.waitingCall;

            reciever.GetComponent<SpriteRenderer>().color = Color.red;
            Debug.Log(reciever.name + " is called");

            Call call = new Call();

            callsInTransmission.Add(call);

            caller.call = call;
            reciever.call= call;
            call.caller = caller;
            call.reciever = reciever;
            call.status = Call.Status.calling;
        }
    }

    public void LibererDelivrer(NodeController node){
        node.status = NodeController.Status.idle;
        unavailableHosts.Remove(node);
        availableHosts.Add(node);
        node.ChangeColor(new Color(1.0f,1.0f,1.0f,1.0f));
    }

    public void EndCall(bool success) {
        if(success) {
            score++;
            //Debug.Log("Score : " +score);
            scoreText.text = "Score : " + score;
        } else {
            lives--;
            if(lives <= 0)
                lives=0;
            //Debug.Log("Lives : " +lives);
            livesText.text = "Lives : " + lives;
        }
    }

    public List<NodeController> Trajectory() {
        return actualTrajectory;
    }

    public void BeginTrajectory(NodeController source) {
        actualTrajectory.Clear();
        actualTrajectory.Add(source);
        Debug.Log("Begin with " + source.name);
    }

    public void AddNodeToTrajectory(NodeController node) 
    {
        //vérifier que le node n'a jamais été ajouté, et qu'une trajectoire est en cours
        if(actualTrajectory.Count>0 && !actualTrajectory.Contains(node)) 
        {
            //vérifier si on respecte les edges
            if(node.IsConnectedTo(actualTrajectory[actualTrajectory.Count - 1]))
            {
                actualTrajectory.Add(node);
                Debug.Log("Add " + node.name);

                Call call = null;
                foreach(Call c in callsInTransmission){
                    if(c.caller==actualTrajectory[0]){
                        call = c;
                    }
                }
                //changer couleur du edge en question
                //node.edge(actualTrajectory[actualTrajectory.Count - 2]).ChangeColor(new Color(1,0,0));
                node.edge(actualTrajectory[actualTrajectory.Count - 2]).TakePath(call.id);
            }
        }
    }

    public void EndTrajectory(NodeController destination) {
        actualTrajectory.Add(destination);
        Debug.Log("Finish with " + destination.name);

        //changer couleur du edge en questionCall call = null;
        Call call = null;
        foreach(Call c in callsInTransmission){
            if(c.caller==actualTrajectory[0]){
                call = c;
            }
        }
        //changer couleur du edge en question
        //node.edge(actualTrajectory[actualTrajectory.Count - 2]).ChangeColor(new Color(1,0,0));
        if(call!=null)
            destination.edge(actualTrajectory[actualTrajectory.Count - 2]).TakePath(call.id);
        //...

        //changer état call et des hosts
        call.SetInCall();
        actualTrajectory[actualTrajectory.Count - 1].status = NodeController.Status.inCall;
        actualTrajectory[0].status = NodeController.Status.inCall;

        actualTrajectory.Clear();

        
    }

    public float angle(float x, float y){
        if(x==0){
            if(y>0){
                return 90;
            }else if(y<0){
                return 270;
            }else{
                return 0;
            }
        }
        if(x<0){
            return Mathf.Atan(y/x)*180/Mathf.PI+180;
        }
        return Mathf.Atan(y/x)*180/Mathf.PI;
    }
}
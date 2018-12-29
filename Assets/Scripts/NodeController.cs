using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour {
    
    const double limit_radius = 0.8;//distance à partir de laquelle, la souris "touche" un node
    float m_timer_movement = 0;

    Vector3 positionInitiale;
    Vector3 positionInitialeText;

    [SerializeField] public List<EdgeController> neighborhoodList;
    [SerializeField] GameObject cursorPrefab;

    [SerializeField] public bool isHost;

    private GameObject timerTextGameObject;
    private Text timerText;

    public enum Status{ idle, calling, waitingCall, inCall};
    public Status status=Status.idle;

    public bool isUsed {get; set;}//représente si un node obligatoire pour un call est utilisé par ledit call.

    public Call call{get;set;}//référence à l'appel associé au host

    private SpriteRenderer render;
    private GameManager gameManager;
    
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        positionInitiale = transform.position;
        m_timer_movement = Random.Range(0, 100);

        if (isHost)
        {
            timerText = transform.Find("Canvas").transform.Find("TextTimer").GetComponent<Text>();
            timerTextGameObject = transform.Find("Canvas").transform.Find("TextTimer").gameObject;
            positionInitialeText = timerText.transform.position;
        }
    }

    public void Initialiser() {
        isUsed = false;
    }

    private void Update()
    {
        m_timer_movement += Time.deltaTime;
        transform.position = positionInitiale + new Vector3(Mathf.Cos(m_timer_movement*0.97f), Mathf.Sin(m_timer_movement*1.02f))*0.2f;
        
        //ajouter la node parcequ'on est proche
        if((Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10))-transform.position).magnitude<limit_radius && !isHost){//si suffisamment proche
            gameManager.AddNodeToTrajectory(this);
        }

        //mouse down
        if(Input.GetMouseButtonDown(0) && (Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10))-transform.position).magnitude<limit_radius){
            if(isHost && (status==Status.calling || status==Status.waitingCall)) {
                /*GameObject iCursor = Instantiate(cursorPrefab, cursorPrefab.transform.position, cursorPrefab.transform.rotation);
                iCursor.GetComponent<CursorBehavior>().SetCurrentNode(this);*/

                gameManager.BeginTrajectory(this);
            } 
        }
        
        //mouse up
        if(/*Input.GetMouseButtonUp(0) &&*/ (Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10))-transform.position).magnitude<limit_radius*2) {
            if(isHost && (status==Status.waitingCall || status==Status.calling)){
                if(gameManager.Trajectory().Count>0 && (gameManager.Trajectory()[0].call.reciever == this  || gameManager.Trajectory()[0].call.caller == this ) && gameManager.Trajectory()[0]!= this && (gameManager.Trajectory()[0].call.node_obligatory==null || gameManager.Trajectory()[0].call.node_obligatory.isUsed)){
                    gameManager.EndTrajectory(this);
                }
            }
        }

    }


    public void AddEdge(EdgeController edge)
    {
        neighborhoodList.Add(edge);
    }

    public bool IsConnectedTo(NodeController otherNode)
    {
        foreach(EdgeController edge in neighborhoodList)
        {
            if(edge.KnowOtherSide(this) == otherNode)
            {
                return true;
            }
        }

        return false;
    }

    public EdgeController edge(NodeController otherNode)
    {
        //renvoie le edge qui lie this à otherNode
        foreach(EdgeController e in neighborhoodList)
        {
            if(e.KnowOtherSide(this) == otherNode)
            {
                return e;
            }
        }

        return null;
    }

    public void ChangeColor(Color newColor)
    {
        render.color = newColor;
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void DisplayMessageBox(bool doDisplay)
    {
        if(isHost)
        {
            transform.Find("MessageBox").gameObject.SetActive(doDisplay);
        }
    }

    public void UpdateTimer(float time)
    {
        if(call==null || !call.isInfinite) {
            timerTextGameObject.SetActive(time >= 0.0f);
        }

        if(time==0) {
            timerTextGameObject.transform.localScale = new Vector3(1, 1, 1)*(Mathf.Cos(m_timer_movement*5.05f)/2.0f+1.5f);
        }
        else {
            timerTextGameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        timerText.text = time.ToString("F1");
        timerText.transform.position = positionInitialeText + new Vector3(Mathf.Cos(m_timer_movement * 1.05f), Mathf.Sin(m_timer_movement * 0.91f)) * 0.2f;

        if(status == Status.calling)
            timerText.color = Color.red;
        else if(status == Status.inCall)
            timerText.color = Color.green;
        
    }

    public void Suppress() {
        timerTextGameObject.SetActive(false);
    }
}

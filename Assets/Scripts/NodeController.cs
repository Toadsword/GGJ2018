using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour {
    
    const double limit_radius = 0.5;//distance à partir de laquelle, la souris "touche" un node
    float m_timer_movement = 0;

    Vector3 positionInitiale;

    [SerializeField] public List<EdgeController> neighborhoodList;
    [SerializeField] GameObject cursorPrefab;

    [SerializeField] public bool isHost;

    private GameObject timerTextGameObject;
    private Text timerText;

    public enum Status{ idle, calling, waitingCall, inCall};
    public Status status=Status.idle;

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
        }
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
            if(isHost && status==Status.calling) {
                /*GameObject iCursor = Instantiate(cursorPrefab, cursorPrefab.transform.position, cursorPrefab.transform.rotation);
                iCursor.GetComponent<CursorBehavior>().SetCurrentNode(this);*/

                gameManager.BeginTrajectory(this);
            } 
        }
        
        //mouse up
        if(/*Input.GetMouseButtonUp(0) &&*/ (Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10))-transform.position).magnitude<limit_radius*2) {
            if(isHost && status==Status.waitingCall){
                if(gameManager.Trajectory().Count>0 && gameManager.Trajectory()[0].call.reciever == this){
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
        timerTextGameObject.SetActive(time >= 0.0f);
        timerText.text = time.ToString("F1");
    }
}

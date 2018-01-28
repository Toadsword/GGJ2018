using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager:MonoBehaviour {
    private int score = 0;
    private int lives = 3;

    private Text scoreText;
    private Text livesText;

    [SerializeField] EdgeController edgeCursor;
    [SerializeField] GameObject haloPrefab;
    [SerializeField] GameObject messageBoxPrefab;

    public Color[] colorArray;

    List<Call> callsInTransmission = new List<Call>();

    [SerializeField] List<NodeController> availableHosts;
    private List<NodeController> unavailableHosts = new List<NodeController>();

    //liste les nodes actuellement utilisés pour la transmission en cours
    [SerializeField] List<NodeController> actualTrajectory = new List<NodeController>();

    private List<EdgeController> alreadyUsedEdges = new List<EdgeController>();
    private List<int> idUsedEdges = new List <int>();

    [SerializeField] GameObject ListeEdges;
    [SerializeField] GameObject ListeScore;
    [SerializeField] ScoreBehavior prefabScore;

    private int[] id;

    float timerBeforeNextCall;

    //--------------Lancement partie
    Camera camera;

    [SerializeField]
    GameObject MenuPrincipal;

    bool inGame=false;
    float zoomCamera=10;

    [SerializeField]
    NodeController HostHomme;

    [SerializeField]
    NodeController HostFemme;
    
    //---------------PRISE JACK
    bool pause = false;
    bool click=false;
    Vector3 previousPos;
    Vector3 positionJackInitiale;

    [SerializeField] GameObject JackGLobal;
    [SerializeField] Image JackPendu;
    [SerializeField] Image JackTendu;
    [SerializeField] Image Prise;

    [SerializeField] Canvas MenuPause;

    [SerializeField]
    Image BatterieJaune;
    [SerializeField]
    Image BatterieVerte;
    [SerializeField]
    Image BatterieRouge;

    private void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        scoreText.text = "Score : " + score;
        livesText = GameObject.Find("LivesText").GetComponent<Text>();
        livesText.text = "Lives : " + lives;


        positionJackInitiale = JackPendu.transform.position;

        //lancer call de tuto 
        NodeController caller = HostHomme;
        caller.status=NodeController.Status.calling;

        NodeController reciever = HostFemme;
        reciever.status = NodeController.Status.waitingCall;

        Call call = new Call(true);//true = chrono infini

        caller.GetComponent<SpriteRenderer>().color = GetColorFromId(call.id);
        reciever.GetComponent<SpriteRenderer>().color = GetColorFromId(call.id);
        callsInTransmission.Add(call);

        caller.call = call;
        reciever.call= call;
        call.caller = caller;
        call.reciever = reciever;


        caller.DisplayMessageBox(true);

        call.status = Call.Status.calling;



    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
        //gestion démarrage partie
        if(inGame){
            //déplacement menu
            if(camera.transform.position.x<0){
                camera.transform.position += (new Vector3(0,0,-10)-camera.transform.position)/30.0f;
            }else{
                camera.transform.position = new Vector3(0,0,-10);
            }

            MenuPrincipal.transform.position -= new Vector3(20,0,0);

            //zoom caméra
            float zoomCible=6.0f;
            if(zoomCamera>zoomCible){
                zoomCamera += (zoomCible-zoomCamera)/30.0f;
                GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = zoomCamera;
            }else{
                zoomCamera = 5;
            }
            //lancement call

            /*if(timer_starting_call>=0){
                timer_starting_call -=Time.deltaTime;
                if(timer_starting_call<0) {
                    StartingCall();
                }
            }*/
        }

        actualizePositionEdges();
        //Set hosts opacity to know if they are available or not
        foreach (NodeController hosts in availableHosts)
        {
            hosts.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
            hosts.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
        foreach (NodeController hosts in unavailableHosts)
        {
            hosts.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Call call = null;
            if(actualTrajectory.Count > 0)
            {
                foreach (Call c in callsInTransmission)
                {
                    if (c.caller == actualTrajectory[0])
                        call = c;
                }
                call.status = Call.Status.calling;
            }
                  
            edgeCursor.gameObject.SetActive(false);
            //LACHER
            for(int i=0;i<actualTrajectory.Count-1;++i){
                actualTrajectory[i].edge(actualTrajectory[i+1]).TakePath(-1);
            }
            //mais attention de pas vider un edge qui était utile avant mais le supprimer si il doit être delete pour un nouveau call

            if(actualTrajectory.Count>0 && !actualTrajectory[actualTrajectory.Count-1].isHost)
            {
                for(int i=0;i<alreadyUsedEdges.Count;++i) 
                {
                    alreadyUsedEdges[i].TakePath(idUsedEdges[i]);
                }
            }
            actualTrajectory.Clear();
            alreadyUsedEdges.Clear();
            idUsedEdges.Clear();
        }

        if(actualTrajectory.Count>0 && !pause){
            //afficher edgecurseur
            Vector3 n = Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10))-actualTrajectory[actualTrajectory.Count-1].transform.position;
            float distance = (n).magnitude;
            edgeCursor.gameObject.SetActive(true);
            float alpha = angle( n.x, n.y);

            edgeCursor.transform.position = (actualTrajectory[actualTrajectory.Count-1].transform.position+Camera.main.ScreenToWorldPoint(Input.mousePosition+new Vector3(0,0,10)))/2.0f;
            edgeCursor.transform.localScale = new Vector3(distance*1/8.5f,0.6f,1);
            edgeCursor.transform.localEulerAngles = new Vector3(0,0,alpha);

            Call call = null;
            foreach(Call c in callsInTransmission){
                if(c.caller==actualTrajectory[0]){
                    call = c;
                }
            }

            edgeCursor.GetComponent<SpriteRenderer>().color = GetColorFromId(call.id);
        }

        for (int i=0;i<callsInTransmission.Count;++i)
        {
            if(callsInTransmission[i].Update()){//autodestruction du call car terminé
                //si on supprimer call alors qu'on était en cours de transmission, on doit supprimer la trajectoire
                if(actualTrajectory.Count>0 && callsInTransmission[i].caller == actualTrajectory[0]){
                    for(int j=0;j<actualTrajectory.Count-1;++j){
                        actualTrajectory[j].edge(actualTrajectory[j+1]).TakePath(-1);
                    }
                    //mais attention de pas vider un edge qui était utile avant mais le supprimer si il doit être delete pour un nouveau call

                    if(actualTrajectory.Count>0 && !actualTrajectory[actualTrajectory.Count-1].isHost)
                    {
                        for(int j=0;j<alreadyUsedEdges.Count;++j) 
                        {
                            alreadyUsedEdges[j].TakePath(idUsedEdges[j]);
                        }
                    }
                    actualTrajectory.Clear();
                    alreadyUsedEdges.Clear();
                    idUsedEdges.Clear();
                    edgeCursor.gameObject.SetActive(false);
                }
                UnlightPath(callsInTransmission[i].id);
                callsInTransmission.RemoveAt(i);
                i--;
            }
        }

        //actualisation timer next call
        if(inGame){
            Debug.Log("In Game : " + timerBeforeNextCall);
            timerBeforeNextCall -= Time.deltaTime;
            if(timerBeforeNextCall<0){
                StartingCall();
            }
        }

        //-------prise jack


        if(!pause){
            if(click){
                if((Input.mousePosition-Prise.transform.position).magnitude>60) {
                    pause = true;
                    MenuPause.gameObject.SetActive(true);
                    JackGLobal.transform.SetParent(MenuPause.transform); // CA FONCTIONNE

                    Time.timeScale = 0f;
                    JackTendu.gameObject.SetActive(false);
                    JackPendu.gameObject.SetActive(true);
                    

                    JackPendu.transform.position = Input.mousePosition-new Vector3(JackPendu.rectTransform.sizeDelta.x*3/4.0f,JackPendu.rectTransform.sizeDelta.y*2.0f/10.0f,0);

                    previousPos = JackPendu.transform.position-Input.mousePosition;
                }
            }

            if(Input.GetMouseButtonDown(0) && (Input.mousePosition-Prise.transform.position).magnitude<50){
                click=true;
            }
            if(Input.GetMouseButtonUp(0)) {
                click = false;
            }


        }else{

            if(click) {
                //move jack

                JackPendu.transform.position = (Input.mousePosition+previousPos);
                float limitey = Screen.height/2.0f;
                float limitex = Screen.width*0.9f;

                if(JackPendu.transform.position.y<limitey){
                    JackPendu.transform.position = new Vector3(JackPendu.transform.position.x, limitey,0);
                }
                if(JackPendu.transform.position.x<0) {
                    JackPendu.transform.position = new Vector3(0, JackPendu.transform.position.y,0);
                }
                if(JackPendu.transform.position.x>limitex) {
                    JackPendu.transform.position = new Vector3(limitex, JackPendu.transform.position.y,0);
                }
                if(JackPendu.transform.position.y>Screen.height*0.8f) {
                    JackPendu.transform.position = new Vector3(JackPendu.transform.position.x, Screen.height*0.8f,0);
                }
            }

            float marge = 30.0f;
            if(Input.GetMouseButtonDown(0) && 
            Input.mousePosition.x>=JackPendu.transform.position.x-marge && Input.mousePosition.x<=JackPendu.transform.position.x+JackPendu.rectTransform.sizeDelta.x + marge
            &&
            Input.mousePosition.y>=JackPendu.transform.position.y -marge && Input.mousePosition.y<=JackPendu.transform.position.y+JackPendu.rectTransform.sizeDelta.y +marge) {
                
                click=true;
                previousPos = JackPendu.transform.position-Input.mousePosition;
            }


            if(Input.GetMouseButtonUp(0) && click) {
                click=false;
                if((Input.mousePosition-Prise.transform.position).magnitude<50){
                    //quitter pause
                    pause = false;
                    MenuPause.gameObject.SetActive(false);
                    JackGLobal.transform.SetParent(MenuPrincipal.transform);
                    JackGLobal.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    Time.timeScale = 1f;
                    JackTendu.gameObject.SetActive(true);
                    JackPendu.gameObject.SetActive(false);
                }
            }
        }
    }

    private void StartingCall() {
        timerBeforeNextCall = Random.Range(5,10);

        if (availableHosts.Count >= 2 && callsInTransmission.Count<10) {
            Debug.Log("Au moins 2 travaillent");

            int randomCaller = Random.Range(0,availableHosts.Count);

            NodeController caller = availableHosts[randomCaller];
            availableHosts.Remove(caller);
            unavailableHosts.Add(caller);
            caller.status=NodeController.Status.calling;

            Debug.Log(caller.name + " is calling");

            int randomReciever = Random.Range(0, availableHosts.Count);

            NodeController reciever = availableHosts[randomReciever];
            availableHosts.Remove(reciever);
            unavailableHosts.Add(reciever);
            reciever.status = NodeController.Status.waitingCall;

            Debug.Log(reciever.name + " is called");

            Call call = new Call();

            caller.GetComponent<SpriteRenderer>().color = GetColorFromId(call.id);
            reciever.GetComponent<SpriteRenderer>().color = GetColorFromId(call.id);
            callsInTransmission.Add(call);

            caller.call = call;
            reciever.call= call;
            call.caller = caller;
            call.reciever = reciever;

            caller.DisplayMessageBox(true);

            call.status = Call.Status.calling;
        }
    }

    public void LibererDelivrer(NodeController node){
        node.status = NodeController.Status.idle;
        unavailableHosts.Remove(node);
        availableHosts.Add(node);
        node.ChangeColor(new Color(1.0f,1.0f,1.0f,1.0f));
    }

    public void EndCall(Call call) {
        
        if(call.status==Call.Status.inCall) {
            Debug.Log("SUCCESS");
            score+=call.size;
            //Debug.Log("Score : " +score);
            scoreText.text = "Score : " + score;
        } else if(call.status==Call.Status.calling || call.status==Call.Status.interruptedCall || call.status == Call.Status.transmitting) {
            lives--;
            BatterieJaune.gameObject.SetActive(false);
            BatterieVerte.gameObject.SetActive(false);
            BatterieRouge.gameObject.SetActive(false);
            if(lives>=2) {
                BatterieJaune.gameObject.SetActive(true);
            }
            if(lives>=1) {
                BatterieRouge.gameObject.SetActive(true);
            }

            if(lives <= 0)
                lives=0;
            //Debug.Log("Lives : " +lives);
            livesText.text = "Lives : " + lives;
        }
    }

    public List<NodeController> Trajectory() {
        return actualTrajectory;
    }

    public void BeginTrajectory(NodeController source)
    {
        if (!pause)
        {
            Debug.Log("Begin with " + source.name);
            actualTrajectory.Clear();
            alreadyUsedEdges.Clear();
            idUsedEdges.Clear();
            actualTrajectory.Add(source);
            source.call.status = Call.Status.transmitting;
        }
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

                EdgeController actualEdge = node.edge(actualTrajectory[actualTrajectory.Count - 2]);

                if (actualEdge.IsTaken())
                {
                   alreadyUsedEdges.Add(actualEdge);
                   idUsedEdges.Add(actualEdge.idMessage);
                }
                //changer couleur du edge en question
                //node.edge(actualTrajectory[actualTrajectory.Count - 2]).ChangeColor(new Color(1,0,0));
                actualEdge.TakePath(call.id);
            }
        }
    }

    public void EndTrajectory(NodeController destination) {
       //vérifier que le node n'a jamais été ajouté, et qu'une trajectoire est en cours
        if(actualTrajectory.Count>0 && !actualTrajectory.Contains(destination)) 
        {
            //vérifier si on respecte les edges
            if(destination.IsConnectedTo(actualTrajectory[actualTrajectory.Count - 1]))
            {
                actualTrajectory.Add(destination);
                Debug.Log("Finish with " + destination.name);

                //changer couleur du edge en questionCall call = null;
                Call call = null;
                foreach(Call c in callsInTransmission)
                {
                    if(c.caller==actualTrajectory[0])
                        call = c;
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

                call.setSize(actualTrajectory.Count-1);

                if(inGame)
                    launchScore(actualTrajectory.Count-1);

                edgeCursor.gameObject.SetActive(false);

                actualTrajectory.Clear();

                //suppresion de toutes les paths qui posent problemes
                //mais attention de pas vider un edge qui était utile avant
                for(int i=0;i<alreadyUsedEdges.Count;++i){
                    UnlightPath(idUsedEdges[i]);
                    getCallFromId(idUsedEdges[i]).Interrupt();
                }

                if(destination==HostFemme){
                    launchGame();
                }
            }

        }
    }

    public Color GetColorFromId(int id)
    {
        return colorArray[id % 10];
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
    
    public void InstantiateHalo(NodeController reciever, Color color)
    {
        Transform transformReciever = reciever.GetTransform();
        GameObject instance =  Instantiate(haloPrefab, transformReciever.position, transformReciever.rotation);
        instance.GetComponent<SpriteRenderer>().color = color;
    }

    private void UnlightPath(int idCall)
    {
        Call call = null;
        foreach(Call c in callsInTransmission){
            if(c.id==idCall)
                call = c;
        }
        if(call!=null && call.status==Call.Status.inCall){

            foreach(Transform edgeTransform in ListeEdges.transform){
                EdgeController edge = edgeTransform.GetComponent<EdgeController>();
                if(edge.idMessage==call.id){
                    edge.TakePath(-1);
                }
            }

            /*NodeController actualNode = call.caller;

            //chercher prochain node
            int iter=0;
            while(actualNode!=call.reciever && iter<100){
                iter++;
                foreach(EdgeController edge in actualNode.neighborhoodList){
                    if(edge.idMessage == call.id){
                        edge.TakePath(-1);
                        actualNode = edge.KnowOtherSide(actualNode);
                    
                    }
                }
            }*/
        }
    }

    private void actualizePositionEdges()
    {
        foreach(Transform edgeTransform in ListeEdges.transform){
            EdgeController edge = edgeTransform.GetComponent<EdgeController>();
            //pour chaque edge, on récupère les positions des deux bouts
            //et on détermine où le edges doit aller
            Vector3 pos1 = edge.Node1().GetTransform().position;
            Vector3 pos2 = edge.Node2().GetTransform().position;

            Vector3 n = pos2-pos1;

            float distance = (n).magnitude;
            float alpha = angle(n.x,n.y);

            edge.transform.position = (pos1+pos2)/2.0f;
            edge.transform.localScale = new Vector3(distance*1/8.5f,0.6f,1);
            edge.transform.localEulerAngles = new Vector3(0,0,alpha);
        }
    }

    private void launchScore(int nb)
    {
        Debug.Log("AFFICHAGE");
        //create prefab Score
        ScoreBehavior pfScore = Instantiate(prefabScore) ;
        pfScore.transform.SetParent(ListeScore.transform);
        pfScore.GetComponent<Text>().text = "+" + nb;

        Vector2 sizeDelta = pfScore.GetComponent<RectTransform>().sizeDelta;
        pfScore.transform.position = (Input.mousePosition)+new Vector3(sizeDelta.x, -sizeDelta.y,0)/2.0f;
    }

    public NodeController ActualSource()
    {
        if(actualTrajectory.Count >= 1)
        {
            return actualTrajectory[0];
        }
        return null;
    }

    public void launchGame(){
        inGame = true;
        timerBeforeNextCall=3.0f;
    }

    public Call getCallFromId(int id)
    {
        foreach(Call c in callsInTransmission){
            if(c.id==id){
                return c;
            }
        }
        return null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
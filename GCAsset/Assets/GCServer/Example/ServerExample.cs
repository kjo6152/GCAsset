using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * @breif GCAsset을 이용하여 서버를 열고 클라이언트와 연결하는 예제
 * @details GCAsset를 통해 컨트롤러(클라이언트)에서 접속할 서버와 서버에 대한 정보를 얻는다.
 * 또한 컨트롤러가 연결되었을 때 시스템 이벤트 리스너를 등록하는 예제와 컨트롤러에게 전송한 컨트롤러 씬을 로드하도록
 * 메시지를 보내는 예제로 구성되어 있다.
 * @author jiwon
 */
public class ServerExample : MonoBehaviour {
    GCcontext mGCcontext;
	// Use this for initialization

    /**
     * @breif 서버 정보 획득 및 리스너 초기화, 리스너 등록
     * @details 서버의 IP와 Port를 얻어오고 시스템 이벤트를 등록한다.
     * @see GCcontext
     * @see EventManager
     * @see ServerManager
     */
	void Start () {
        mGCcontext = GCcontext.getInstance;

        string[] ipList = mGCcontext.getServerManager().getIPAddress();
        string port = mGCcontext.getServerManager().getPort() + "";
        string str = "";
        foreach (string ip in ipList)
        {
            str += "IP : " + ip + "\r\n";
        }
        GameObject.Find("addressText").GetComponent<Text>().text = str + "port : " + port;

        mGCcontext.getEventManager().clearListener();
        mGCcontext.getEventManager().onControllerConnected += new EventManager.SystemEventListener(mEventManager_onControllerConnected);
        mGCcontext.getEventManager().onControllerComplete += new EventManager.SystemEventListener(mEventManager_onControllerComplete);
        mGCcontext.getEventManager().onControllerDisconnected += new EventManager.SystemEventListener(mEventManager_onControllerDisconnected);
	}

    /**
     * @breif 컨트롤러가 처음 연결되었을 때 발생하는 이벤트의 리스너
     * @see SystemEventListener
     * @see GameController
     */
    void mEventManager_onControllerConnected(GameController gc)
    {
        Debug.Log("onControllerConnected");
    }

    /**
     * @breif 컨트롤러가 연결이 완료(리소스가 모두 전송됨)되었을 때 발생하는 이벤트의 리스너
     * @see SystemEventListener
     * @see GameController
     */
    void mEventManager_onControllerComplete(GameController gc)
    {
        Debug.Log("onControllerComplete");
    }

    /**
     * @breif 컨트롤러가 연결이 끊겼을 때 발생하는 이벤트의 리스너
     * @see SystemEventListener
     * @see GameController
     */
    void mEventManager_onControllerDisconnected(GameController gc)
    {
        Debug.Log("onControllerDisconnected");
    }

	// Update is called once per frame
	void Update () {
        if (mGCcontext.getServerManager().isRunning())
        {
            Sprite image = Resources.Load<Sprite>("powerbutton_on");
            Image OnOffImage = GameObject.Find("ServerButton").GetComponentInChildren<Image>();
            OnOffImage.sprite = image;
        }
        else
        {
            Sprite image = Resources.Load<Sprite>("powerbutton_off");
            if (image == null) Debug.Log("null!!");
            Image OnOffImage = GameObject.Find("ServerButton").GetComponentInChildren<Image>();
            OnOffImage.sprite = image;
        }
	}

    /**
     * @breif 서버를 시작하거나 종료하는 예제
     * @see ServerManager
     */
    public void startOrEndServer()
    {
        mGCcontext = GCcontext.getInstance;
        Debug.Log("startOrEndServer");
        if (mGCcontext.getServerManager().isRunning())
        {
            mGCcontext.getServerManager().stopServer();
        }
        else
        {
            mGCcontext.getServerManager().startServer();
        }
    }

    /**
     * @breif 컨트롤러에게 해당 씬을 로드하라는 메시지를 보내는 예제
     * @see ServerManager
     * @see GameController
     */
    public void changeScene()
    {
        Debug.Log("changeScene");
        ArrayList ControllerList = mGCcontext.getServerManager().getControllerList();
        for (int i = 0; i < ControllerList.Count; i++)
        {
            GameController controller = (GameController)ControllerList[i];
            controller.sendChangeView("controller");
        }
    }

    /**
     * @breif EventExample 예제로 넘어가는 매소드
     * @see EventExample
     */
    public void NextScene()
    {
        Debug.Log("NextScene");
        Application.LoadLevel("EventExample");
    }
}
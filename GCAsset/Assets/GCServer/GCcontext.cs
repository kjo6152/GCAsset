using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * 1. 게임컨트롤러 컨텍스트
 * 기능 :
 *  1) 게임에 컨트롤러 라이브라리에 대한 전체적인 관리를 한다.
 *  2) 여러 매니저들의 초기화와 매니저의 인스턴스를 얻을 수 있다.
 *  3) 매니저 클래스는 다음과 같다.
 *   - 게임서버 매니저, 이벤트 매니저, 리소스 매니저
 * 
 * 매소드 :
 */
public class GCcontext : MonoBehaviour {

	public ResourceManager mResourceManager = null;
    public EventManager mEventManager = null;
    public ServerManager mServerManager = null;

	EventManager.EventQueue mEventQueue;

	void init(){

		mResourceManager = new ResourceManager ();
		mEventManager = new EventManager ();
		mServerManager = new ServerManager ();
		mEventQueue = new EventManager.EventQueue ();

		//ResourceManager 의존성 주입 및 초기화
		mResourceManager.init ();
        mResourceManager.createResourceMap();

		//EventManager 의존성 주입 및 초기화
		mEventManager.setEventQueue (mEventQueue);
		mEventManager.init ();

		//ServerManager 의존성 주입 및 초기화
		mServerManager.setEventManager (mEventManager);
		mServerManager.setResourceMeneager (mResourceManager);
		mServerManager.init ();

	}
    
    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (mServerManager != null && mServerManager.isRunning())
        {
            mServerManager.stopServer();
        }
    }

    public void startOrEndServer()
    {
		
        Debug.Log("startOrEndServer");
        if (mServerManager.isRunning())
        {
            mServerManager.stopServer();
        }
        else
        {
            mServerManager.startServer();
        }
		//GameController gc = new GameController (null);
		//gc.readDeviceDataFromXml (null);
    }

	// Use this for initialization
	void Start () {
        init();
        string[] ipList = mServerManager.getIPAddress();
        string port = mServerManager.getPort() + "";
        string str = "";
        foreach (string ip in ipList)
        {
            str += "IP : " + ip + "\r\n";
        }
        GameObject.Find("addressText").GetComponent<Text>().text = str + "port : " + port;
	}

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {
		// Todo : 이벤트 큐에서 이벤트를 꺼내 처리한다.

	}

}

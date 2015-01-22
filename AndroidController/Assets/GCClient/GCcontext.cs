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
    public ClientManager mClientManager = null;

	EventManager.EventQueue mEventQueue;

	void init(){

		mResourceManager = new ResourceManager ();
		mEventManager = new EventManager ();
        mClientManager = new ClientManager();
		mEventQueue = new EventManager.EventQueue ();

		//ResourceManager 의존성 주입 및 초기화
		mResourceManager.init ();
        mResourceManager.createResourceMap();

		//EventManager 의존성 주입 및 초기화
		mEventManager.setEventQueue (mEventQueue);
		mEventManager.init ();

		//ServerManager 의존성 주입 및 초기화
        mClientManager.setEventManager(mEventManager);
        mClientManager.setResourceMeneager(mResourceManager);
        mClientManager.init();

	}
    
    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (mClientManager != null && mClientManager.isRunning())
        {
            mClientManager.stopClient();
        }
    }

    public void startOrEndServer()
    {
		
        Debug.Log("startOrEndServer");
        if (mClientManager.isRunning())
        {
            mClientManager.stopClient();
        }
        else
        {
            mClientManager.startClient();
        }
		//GameController gc = new GameController (null);
		//gc.readDeviceDataFromXml (null);
    }

	// Use this for initialization
	void Start () {
        init();
	}

    void Awake()
    {

    }
	
	// Update is called once per frame
	void Update () {
		// Todo : 이벤트 큐에서 이벤트를 꺼내 처리한다.

	}

}

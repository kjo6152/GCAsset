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
    private static GCcontext mGCcontext;

    public static GCcontext getInstance
    {
        get
        {
            if (!mGCcontext)
            {
                mGCcontext = (GCcontext)GameObject.FindObjectOfType(typeof(GCcontext));
                if (!mGCcontext)
                {
                    GameObject container = new GameObject();
                    container.name = "GCcontext";
                    mGCcontext = container.AddComponent(typeof(GCcontext)) as GCcontext;
                    mGCcontext.init();
                }
            }

            return mGCcontext;
        }
    } 

	void init(){

		mResourceManager = new ResourceManager ();
		mEventManager = new EventManager ();
        mClientManager = new ClientManager();
		mEventQueue = new EventManager.EventQueue ();

		//ResourceManager 의존성 주입 및 초기화
		mResourceManager.init ();

		//EventManager 의존성 주입 및 초기화
		mEventManager.setEventQueue (mEventQueue);
		mEventManager.init ();

		//ServerManager 의존성 주입 및 초기화
        mClientManager.setEventManager(mEventManager);
        mClientManager.setResourceMeneager(mResourceManager);
        mClientManager.init();

        //센서 관련된 값 확인
        if (SystemInfo.supportsAccelerometer) Debug.Log("supportsAccelerometer");
        else Debug.Log("Not supportsAccelerometer");

        if (SystemInfo.supportsGyroscope)
        {
            Debug.Log("supportsGyroscope");
            Input.gyro.enabled = true;
        }
        else Debug.Log("Not supportsGyroscope");
	}
    
    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (mClientManager != null && mClientManager.isRunning())
        {
            mClientManager.stopClient();
        }
    }

	// Use this for initialization
	void Start () {
        
	}


	
	// Update is called once per frame
	void Update () {
        /*
		// Todo : 이벤트 큐에서 이벤트를 꺼내 처리한다.
        if(SystemInfo.supportsAccelerometer){
            Debug.Log("Accelerometer - x : " + Input.acceleration.x + " / y : " + Input.acceleration.y + " / z : " + Input.acceleration.z);
        }
        if (SystemInfo.supportsGyroscope && Input.gyro.enabled)
        {
            Debug.Log("Gyroscope");
            Debug.Log("rotationRate - x : " + Input.gyro.rotationRate.x + " / y : " + Input.gyro.rotationRate.y + " / z : " + Input.gyro.rotationRate.z);
            Debug.Log("gravity - x : " + Input.gyro.gravity.x + " / y : " + Input.gyro.gravity.y + " / z : " + Input.gyro.gravity.z);
            Debug.Log("attitude - x : " + Input.gyro.attitude.x + " / y : " + Input.gyro.attitude.y + " / z : " + Input.gyro.attitude.z + " / w : " + Input.gyro.attitude.w);
        }
         */
	}

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}

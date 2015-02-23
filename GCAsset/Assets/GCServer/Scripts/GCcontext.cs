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

	ResourceManager mResourceManager = null;
    EventManager mEventManager = null;
    ServerManager mServerManager = null;

    private static GCcontext mGCcontext;
    private static AudioSource mAudioSource;

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
                    mAudioSource = container.AddComponent<AudioSource>();

                    mGCcontext.init();
                }
            }

            return mGCcontext;
        }
    }

	void init(){

		mResourceManager = new ResourceManager ();
		mEventManager = new EventManager ();
		mServerManager = new ServerManager ();

		//ResourceManager 의존성 주입 및 초기화
		mResourceManager.init ();

		//EventManager 의존성 주입 및 초기화
		mEventManager.init ();
        mEventManager.setAudioSource(mAudioSource);

		//ServerManager 의존성 주입 및 초기화
		mServerManager.setEventManager (mEventManager);
		mServerManager.setResourceMeneager (mResourceManager);
		mServerManager.init ();

	}

    public ResourceManager getResourceManager()
    {
        return mResourceManager;
    }
    public EventManager getEventManager()
    {
        return mEventManager;
    }
    public ServerManager getServerManager()
    {
        return mServerManager;
    }

    public AudioSource getAudioSource()
    {
        return mAudioSource;
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (mServerManager != null && mServerManager.isRunning())
        {
            mServerManager.stopServer();
        }
    }


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		// Todo : 이벤트 큐에서 이벤트를 꺼내 처리한다.
        mEventManager.Update();
	}

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

}

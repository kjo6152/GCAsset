using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * @breif GCAsset을 이용하기 위한 여러 객체 및 리소스를 관리하는 클래스
 * @details GCAsset의 기능을 활용하기 위한 모든 객체와 정보들을 가지고 있는 최상위 클래스이다.
 * 싱글톤 패턴으로 하나의 객체만 유지되며 객체 생성시 씬에 자동으로 오브젝트를 생성하여 동작한다.
 * 씬이 변경되어도 해당 오브젝트는 파괴되지 않고 계속 유지된다.
 * 객체 생성시 아래 세 클래스의 객체를 생성한다.
 * @see ResourceManager, getResourceManager
 * @see EventManager, getEventManager
 * @see ServerManager, getServerManager
 * @author jiwon
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

    /** @breif ResourceManager 객체를 얻는 매소드 */
    public ResourceManager getResourceManager()
    {
        return mResourceManager;
    }
    /** @breif EventManager 객체를 얻는 매소드 */
    public EventManager getEventManager()
    {
        return mEventManager;
    }
    /** @breif ServerManager 객체를 얻는 매소드 */
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

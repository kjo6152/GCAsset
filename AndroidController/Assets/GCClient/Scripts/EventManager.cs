using UnityEngine;
using System.Collections;
using System.Threading;
using System;

/**
 * 3. 이벤트 메니저
 * 기능 :
 *  1) 게임서버 매니저로부터 받은 이벤트를 관리한다.
 *  2) 게임 개발자는 이벤트 매니저에 리스너를 등록하여 이벤트를 받아 처리로직을 구현한다. 
 *  3) 이벤트 매니저에 필터를 구현하여 등록할 수 있으며 기본적으로 제공하는 필터를 사용할 수 있다.
 */
public class EventManager {

    public delegate void EventListener();
	public delegate void VibrationListener(int time);
    public delegate void SoundListener(string path);
    public delegate void ViewListener(string SceneName);

    public event EventListener onServerConnected;
    public event EventListener onServerDisconnected;
    public event EventListener onServerComplete;
    public event VibrationListener onVibrationListener;
    public event SoundListener onSoundListener;
    public event ViewListener onViewListener;

    ClientManager mClientManager;
    ResourceManager mResourceManager;
    AudioSource mAudioSource;

    private bool supportsGyroscope;
    private Vector3 preGyroscope;

    Queue EventQueue = new Queue();

    public class Event
    {
        ushort type;
        ushort code;
        object Object;

        public Event(ushort type, ushort code, object Object)
        {
            this.type = type;
            this.code = code;
            this.Object = Object;
        }

        public ushort getType(){
            return type;
        }

        public ushort getCode(){
            return code;
        }

        public string getPath(){
            return (string)Object;
        }

        public int getTime(){
            return (int)Object;
        }
    }

    public void setClientManager(ClientManager mClientManager)
    {
        this.mClientManager = mClientManager;
    }
    public void setResourceManager(ResourceManager mResourceManager)
    {
        this.mResourceManager = mResourceManager;
    }
    public void setAudioSource(AudioSource mAudioSource)
    {
        this.mAudioSource = mAudioSource;
    }
    public void setUpdateInterval(float interval)
    {
        Input.gyro.updateInterval = interval;
    }

    public void setSensorEnabled(bool enabled)
    {
        Input.gyro.enabled = enabled;
    }

	public void init(){
        supportsGyroscope = SystemInfo.supportsGyroscope;
        Debug.Log("supportsGyroscope : " + supportsGyroscope);
        setUpdateInterval(0.5f);
        Input.gyro.enabled = true;
        preGyroscope = Input.gyro.rotationRate;

        onServerConnected = delegate { };
        onServerDisconnected = delegate { };
        onServerComplete = delegate { };
        onVibrationListener = delegate { };
        onSoundListener = delegate { };
        onViewListener = delegate { };
	}

	// Update is called once per frame
	public void Update () {
        if (supportsGyroscope && Input.gyro.enabled)
        {
            if (preGyroscope.x != Input.gyro.rotationRate.x && preGyroscope.y != Input.gyro.rotationRate.y && preGyroscope.z != Input.gyro.rotationRate.z)
            {
                mClientManager.sendSensor(Input.gyro);
                preGyroscope = Input.gyro.rotationRate;
            }
        }
        processEvent();
	}


	/**
	 * 클라이언트로부터 온 이벤트를 ServerManager에게서 받아 처리한다.
	 * 각각 알맞은 Queue에 쌓는 역할을 한다.
	 */
    public void receiveEvent(ushort type, ushort code, object value)
    {
        EventQueue.Enqueue(new Event(type, code, value));
	}

	/**
	 * Queue에 쌓인 이벤트들을 처리한다.
	 * 메인스레드에서 동작시켜야한다.
	 */ 
	public void processEvent(){
		//Todo : 각 이벤트에 대한 필터 제공
        try
        {
            Event mEvent = (Event)EventQueue.Dequeue();

            /**
             * 이벤트 종류에 대한 리스너 처리
             */
            if (mEvent.getType() == GCconst.TYPE_SYSTEM)
            {
                switch (mEvent.getCode())
                {
                    case GCconst.CODE_CONNECTED:
                        onServerConnected();
                        break;
                    case GCconst.CODE_DISCONNECTED:
                        onServerDisconnected();
                        break;
                    case GCconst.CODE_COMPLETE:
                        //AssetBundle 로드
                        mResourceManager.LoadResources();
                        onServerComplete();
                        break;
                }
            }
            else if (mEvent.getType() == GCconst.TYPE_EVENT)
            {
                //Todo : 각 이벤트에 대한 필터 제공
                switch (mEvent.getCode())
                {
                    case GCconst.CODE_VIBRATION:
                        onVibrationListener(mEvent.getTime());
                        break;
                    case GCconst.CODE_SOUND:
                        Debug.Log(mEvent.getPath());
                        playSound(mEvent.getPath());
                        onSoundListener(mEvent.getPath());
                        break;
                    case GCconst.CODE_VIEW:
                        Application.LoadLevel(mEvent.getPath());
                        mResourceManager.setLastScene(mEvent.getPath());
                        onViewListener(mEvent.getPath());
                        break;
                }
            }
        }
        catch (InvalidOperationException e)
        {
            e.ToString();
            //Debug.Log(e);
        }
	}

    public void playSound(string sound){
        Debug.Log("playSound : " + sound);
        mAudioSource.clip = (AudioClip)mResourceManager.getResource(sound);
        mAudioSource.Play();
    }
}

using UnityEngine;
using System.Collections;
using System.Threading;
using System;

/**
 * @breif 이벤트 매니저
 * @detail
 * 다음과 같은 기능을 제공한다.
 * 매 프레임마다 이벤트를 처리하여 사용자에게 제공한다. 사용자는 리스너를 등록하여 이벤트를 받아 사용할 수 있다. 제공되는 이벤트는 다음과 같다.
 * @see onControllerConnected
 * @see onControllerDisconnected
 * @see onControllerComplete
 * @see onAccelerationListener
 * @see onGyroListener
 * @see onButtonListener
 * @see onDirectionKeyListener
 * @see onJoystickListener
 * 
 * 또한 필터를 구현하여 등록할 수 있으며 기본적으로 제공하는 필터를 사용할 수 있다.
 * 기본적으로 제공되는 필터는 아래와 같다.
 * @see AttitudeFilter
 * @see GravityFilter
 * @see KalmanFilter
 * 
 * 자신이 직접 필터를 구현하고 싶다면 다음 인터페이스를 참고하여 등록해야 합니다.
 * @see IEventFiler 
 * @see mGyroFilter
 * @see mAccelerationFilter
 * 
 * @author jiwon
 
 */
public class EventManager {

    /** @breif 시스템 이벤트 리스너 델리게이트 @see GameController */ 
    public delegate void SystemEventListener(GameController gc);
    /** @breif 가속도 센서 이벤트 리스너 델리게이트 @see GameController @see AccelerationEvent */ 
	public delegate void AccelerationListener(GameController gc,AccelerationEvent acceleration);
    /** @breif 자이로 센서 이벤트 리스너 델리게이트 @see GameController @see GyroEvent */ 
	public delegate void GyroListener(GameController gc,GyroEvent gyro);
    /** @breif 버튼 이벤트 리스너 델리게이트 @see GameController @see ButtonEvent */ 
    public delegate void ButtonListener(GameController gc, ButtonEvent buttonEvent);
    /** @breif 방향키 이벤트 리스너 델리게이트 @see GameController @see DirectionKeyEvent */ 
    public delegate void DirectionKeyListener(GameController gc, DirectionKeyEvent directionKeyEvent);
    /** @breif 조이스틱 이벤트 리스너 델리게이트 @see GameController @see JoystickEvent */ 
    public delegate void JoystickListener(GameController gc, JoystickEvent joystickEvent);

    /** @breif 컨트롤러가 연결 되었을 때 호출되는 시스템 이벤트 리스너 @see SystemEventListener */ 
    public event SystemEventListener onControllerConnected;
    /** @breif 컨트롤러가 연결이 끊겼을 때 호출되는 시스템 이벤트 리스너 @see SystemEventListener */ 
    public event SystemEventListener onControllerDisconnected;
    /** @breif 컨트롤러가 연결되어 리소스 전송이 완료 되었을 때 호출되는 시스템 이벤트 리스너 @see SystemEventListener */ 
    public event SystemEventListener onControllerComplete;
    /** @breif 가속도 센서 이벤트 리스너 @see AccelerationListener */ 
    public event AccelerationListener onAccelerationListener;
    /** @breif 자이로 센서 이벤트 리스너 @see GyroListener */ 
    public event GyroListener onGyroListener;
    /** @breif 버튼 이벤트 리스너 @see ButtonListener */ 
    public event ButtonListener onButtonListener;
    /** @breif 방향키 센서 이벤트 리스너 @see DirectionKeyListener */ 
    public event DirectionKeyListener onDirectionKeyListener;
    /** @breif 조이스틱 센서 이벤트 리스너 @see JoystickListener */ 
    public event JoystickListener onJoystickListener;

    /** 
     * @breif 자이로 센서 필터 
     * @detail 컨트롤러로부터 전송된 자이로 센서가 등록된 필터를 거쳐 가공된 형태로 사용자에게 제공된다.
     * @see IEventFilter @see GravityFilter @see KalmanFilter 
     */ 
    public IEventFilter mGyroFilter;
    /** @breif 가속도 센서 필터  @see IEventFilter */ 
    public IEventFilter mAccelerationFilter;

    private AudioSource mAudioSource;

    float[] sensorBuffer = new float[10];
    int[] eventBuffer = new int[3];

    Queue EventQueue = new Queue();

    /** 
     * @breif GCAsset 이벤트
     * @detail GCAsset 내부 여러 클래스들에서 이벤트를 주고 받기 위해 정의된 이벤트
     */ 
    public class Event
    {
        ushort type;
        ushort code;
        GameController mGameController;
        object Object;

        public GameController getGameController()
        {
            return mGameController;
        }

        public GyroEvent getGyroEvent()
        {
            if(code!=GCconst.CODE_GYRO)return null;
            else return (GyroEvent)Object;
        }

        public AccelerationEvent getAccelerationEvent()
        {
            if (code != GCconst.CODE_ACCELERATION) return null;
            else return (AccelerationEvent)Object;
        }

        public ButtonEvent getButtonEvent()
        {
            if (code != GCconst.CODE_BUTTON) return null;
            else return (ButtonEvent)Object;
        }

        public DirectionKeyEvent getDirectionKeyEvent()
        {
            if (code != GCconst.CODE_DIRECTION_KEY) return null;
            else return (DirectionKeyEvent)Object;
        }

        public JoystickEvent getJoystickEvent()
        {
            if (code != GCconst.CODE_JOYSTICK) return null;
            else return (JoystickEvent)Object;
        }

        public int getType(){
            return type;
        }

        public int getCode()
        {
            return code;
        }

        public Event(GameController gc, ushort type, ushort code, object Object)
        {
            this.mGameController = gc;
            this.code = code;
            this.type = type;
            this.Object = Object;
        }
    }

    /** @breif 자이로 센서 이벤트 */ 
    public class GyroEvent
    {
        /** @breif 각 축에 대한 센서 값 */ 
        public float x, y, z, w;
        public GyroEvent(float[] sensor)
        {
            x = sensor[0];
            y = sensor[1];
            z = sensor[2];
            w = sensor[3];
        }
    }

    /** @breif 가속도 센서 이벤트 */ 
    public class AccelerationEvent
    {
        /** @breif 각 축에 대한 센서 값 */ 
        public float x, y, z;
        public AccelerationEvent(float[] sensor)
        {
            x = sensor[0];
            y = sensor[1];
            z = sensor[2];
        }
    }

    /** @breif 버튼 이벤트 */ 
    public class ButtonEvent
    {
        /** @breif 버튼에 지정된 id값 */ 
        public int id;
        public ButtonEvent(int[] eventBuffer){
            id = eventBuffer[0];
        }
    }

    /** @breif 방향키 이벤트 */ 
    public class DirectionKeyEvent
    {
        /** @breif 방향키에 지정된 id값 */
        public int id;
        /** @breif 방향키의 상,하,좌,우 값 @see GCconst.VALUE_UP @see GCconst.VALUE_RIGHT @see GCconst.VALUE_DOWN @see GCconst.VALUE_LEFT */
        public int key;
        public DirectionKeyEvent(int[] eventBuffer)
        {
            id = eventBuffer[0];
            key = eventBuffer[1];
        }
    }

    /** @breif 조이스틱 이벤트 */ 
    public class JoystickEvent
    {
        /** @breif 조이스틱에 지정된 id값 */
        public int id;
        /** @breif 조이스틱 중앙을 원점으로 했을 때 x,y 좌표 값 */
        public int x,y;
        public JoystickEvent(int[] eventBuffer)
        {
            id = eventBuffer[0];
            x = eventBuffer[1];
            y = eventBuffer[2];
        }
    }

    public void setAudioSource(AudioSource mAudioSource)
    {
        this.mAudioSource = mAudioSource;
    }

	public void init(){
        clearListener();
	}

    /** @breif 등록된 리스너를 초기화하는 매소드 */ 
    public void clearListener()
    {
        onControllerConnected = delegate { };
        onControllerDisconnected = delegate { };
        onControllerComplete = delegate { };
        onAccelerationListener = delegate { };
        onGyroListener = delegate { };
        onButtonListener = delegate { };
        onDirectionKeyListener = delegate { };
        onJoystickListener = delegate { };
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void Update () {
        processEvent();
	}

    /**
     * @breif 클라이언트로부터 온 이벤트 저장하는 매소드
     * @detail 클라이언트로부터 온 이벤트를 @see ServerManager 에게서 받아 처리한다.
     * 이벤트 필터를 적용하여 전달된 센서 값들을 필터링한다.
     * 각각 알맞은 Queue에 쌓는 역할을 하며 쌓인 이벤트는 프레임 마다 처리된다.
     * @see processEvent
     */
    public void receiveEvent(GameController gc,ushort type, ushort code,byte[] value){
        if (type == GCconst.TYPE_EVENT)
        {
            switch (code)
            {
                case GCconst.CODE_BUTTON:
                    Buffer.BlockCopy(value, 0, eventBuffer, 0, GCconst.SIZE_BUTTON);
                    EventQueue.Enqueue(new Event(gc, type, code, new ButtonEvent(eventBuffer)));
                    break;
                case GCconst.CODE_DIRECTION_KEY:
                    Buffer.BlockCopy(value, 0, eventBuffer, 0, GCconst.SIZE_DIRECTION_KEY);
                    EventQueue.Enqueue(new Event(gc, type, code, new DirectionKeyEvent(eventBuffer)));
                    break;
                case GCconst.CODE_JOYSTICK:
                    Buffer.BlockCopy(value, 0, eventBuffer, 0, GCconst.SIZE_JOYSTICK);
                    EventQueue.Enqueue(new Event(gc, type, code, new JoystickEvent(eventBuffer)));
                    break;
            }
        }
        else if (type == GCconst.TYPE_SYSTEM)
        {
            EventQueue.Enqueue(new Event(gc, type, code, null));
        }
        else if (type == GCconst.TYPE_SENSOR)
        {
            Buffer.BlockCopy(value, 0, sensorBuffer, 0, GCconst.SIZE_SENSOR);
            switch (code)
            {
                case GCconst.CODE_GYRO:
                    if (mGyroFilter != null) mGyroFilter.filter(ref sensorBuffer);
                    EventQueue.Enqueue(new Event(gc, type, code, new GyroEvent(sensorBuffer)));
                    break;
                case GCconst.CODE_ACCELERATION:
                    if (mAccelerationFilter != null) mAccelerationFilter.filter(ref sensorBuffer);
                    EventQueue.Enqueue(new Event(gc, type, code, new AccelerationEvent(sensorBuffer)));
                    break;
            }
        }
		return;
	}

    /**
     * @breif Queue에 쌓인 이벤트들을 처리한다.
     * @detail 컨트롤러로부터 받은 큐에 저장된 이벤트를 처리하여 적절한 리스너를 호출한다.
     * 메인스레드에서 동작한다.
     * @see receiveEvent
     */
    public void processEvent(){
        try
        {
            while (true)
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
                            onControllerConnected(mEvent.getGameController());
                            break;
                        case GCconst.CODE_DISCONNECTED:
                            onControllerDisconnected(mEvent.getGameController());
                            break;
                        case GCconst.CODE_COMPLETE:
                            onControllerComplete(mEvent.getGameController());
                            break;
                    }
                }
                else if (mEvent.getType() == GCconst.TYPE_EVENT)
                {
                    switch (mEvent.getCode())
                    {
                        case GCconst.CODE_BUTTON:
                            onButtonListener(mEvent.getGameController(), mEvent.getButtonEvent());
                            break;
                        case GCconst.CODE_DIRECTION_KEY:
                            onDirectionKeyListener(mEvent.getGameController(), mEvent.getDirectionKeyEvent());
                            break;
                        case GCconst.CODE_JOYSTICK:
                            onJoystickListener(mEvent.getGameController(), mEvent.getJoystickEvent());
                            break;
                    }
                }
                else if (mEvent.getType() == GCconst.TYPE_SENSOR)
                {
                    switch (mEvent.getCode())
                    {
                        case GCconst.CODE_ACCELERATION:
                            onAccelerationListener(mEvent.getGameController(), mEvent.getAccelerationEvent());
                            break;
                        case GCconst.CODE_GYRO:
                            onGyroListener(mEvent.getGameController(), mEvent.getGyroEvent());
                            break;
                    }
                }

                /**
                 * 게임 컨트롤러에 대한 리스너 처리
                 */
                mEvent.getGameController().receiveEvent(mEvent);
            }
        }
        catch (InvalidOperationException e)
        {
            e.ToString();
            //Debug.Log(e);
        }
        catch (Exception e){
            e.ToString();
            //Debug.Log(e);
        }
	}

    public void playSound(string sound)
    {
        mAudioSource.clip = (AudioClip)Resources.Load(sound);
        mAudioSource.Play();
    }

}

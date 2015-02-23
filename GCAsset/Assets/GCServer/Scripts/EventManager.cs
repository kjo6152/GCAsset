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

    public delegate void SystemEventListener(GameController gc);
	public delegate void AccelerationListener(GameController gc,AccelerationEvent acceleration);
	public delegate void GyroListener(GameController gc,GyroEvent gyro);
    public delegate void ButtonListener(GameController gc, ButtonEvent buttonEvent);
    public delegate void DirectionKeyListener(GameController gc, DirectionKeyEvent directionKeyEvent);
    public delegate void JoystickListener(GameController gc, JoystickEvent joystickEvent);

    public event SystemEventListener onControllerConnected;
    public event SystemEventListener onControllerDisconnected;
    public event SystemEventListener onControllerComplete;
    public event AccelerationListener onAccelerationListener;
    public event GyroListener onGyroListener;
    public event ButtonListener onButtonListener;
    public event DirectionKeyListener onDirectionKeyListener;
    public event JoystickListener onJoystickListener;


    public IEventFilter mGyroFilter;
    public IEventFilter mAccelerationFilter;

    private AudioSource mAudioSource;

    float[] sensorBuffer = new float[10];
    int[] eventBuffer = new int[3];

    Queue EventQueue = new Queue();

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
    public class GyroEvent
    {
        public float x, y, z, w;
        public GyroEvent(float[] sensor)
        {
            x = sensor[0];
            y = sensor[1];
            z = sensor[2];
            w = sensor[3];
        }
    }
    public class AccelerationEvent
    {
        public float x, y, z;
        public AccelerationEvent(float[] sensor)
        {
            x = sensor[0];
            y = sensor[1];
            z = sensor[2];
        }
    }
    public class ButtonEvent
    {
        public int id;
        public ButtonEvent(int[] eventBuffer){
            id = eventBuffer[0];
        }
    }
    public class DirectionKeyEvent
    {
        public int id,key;
        public DirectionKeyEvent(int[] eventBuffer)
        {
            id = eventBuffer[0];
            key = eventBuffer[1];
        }
    }
    public class JoystickEvent
    {
        public int id,x,y;
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
	 * 클라이언트로부터 온 이벤트를 ServerManager에게서 받아 처리한다.
     * 이벤트 필터를 적용하여 전달된 센서 값들을 필터링한다.
	 * 각각 알맞은 Queue에 쌓는 역할을 한다.
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
	 * Queue에 쌓인 이벤트들을 처리한다.
	 * 메인스레드에서 동작시켜야한다.
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

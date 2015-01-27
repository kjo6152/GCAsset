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

    public delegate void EventListener(GameController gc);
	public delegate void AccelerationListener(GameController gc,Acceleration acceleration);
	public delegate void GyroListener(GameController gc,Gyro gyro);

    public event EventListener onControllerConnected;
    public event EventListener onControllerDisconnected;
    public event EventListener onControllerComplete;
    public event AccelerationListener onAccelerationListener;
    public event GyroListener onGyroListener;

    float[] sensorBuffer = new float[10];
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

        public Gyro getGyro()
        {
            if(code!=GCconst.CODE_GYRO)return null;
            else return (Gyro)Object;
        }

        public Acceleration getAcceleration()
        {
            if (code != GCconst.CODE_ACCELERATION) return null;
            else return (Acceleration)Object;
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
    public class Gyro
    {
        public float x, y, z, w;
        public Gyro(float[] sensor)
        {
            x = sensor[0];
            y = sensor[1];
            z = sensor[2];
            w = sensor[3];
        }
    }
    public class Acceleration
    {
        public float x, y, z;
        public Acceleration(float[] sensor)
        {
            x = sensor[0];
            y = sensor[1];
            z = sensor[2];
        }

    }

	


	public void init(){
        onControllerConnected = delegate { };
        onControllerDisconnected = delegate { };
        onControllerComplete = delegate { };
        onAccelerationListener = delegate { };
        onGyroListener = delegate { };
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
	 * 각각 알맞은 Queue에 쌓는 역할을 한다.
	 */ 
	public void receiveEvent(GameController gc,ushort type, ushort code,byte[] value){
		//Todo:이벤트 처리를 어떻게 할 것인지 정해야함
        if (type == GCconst.TYPE_SYSTEM)
        {
            EventQueue.Enqueue(new Event(gc, type, code, null));
        }
        else if (type == GCconst.TYPE_SENSOR)
        {
            Buffer.BlockCopy(value, 0, sensorBuffer, 0, 40);
            switch (code)
            {
                case GCconst.CODE_GYRO:
                    EventQueue.Enqueue(new Event(gc, type, code, new Gyro(sensorBuffer)));
                    break;
                case GCconst.CODE_ACCELERATION:
                    EventQueue.Enqueue(new Event(gc, type, code, new Acceleration(sensorBuffer)));
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
                else if (mEvent.getType() == GCconst.TYPE_SENSOR)
                {
                    //Todo : 각 이벤트에 대한 필터 제공
                    switch (mEvent.getCode())
                    {
                        case GCconst.CODE_ACCELERATION:
                            onAccelerationListener(mEvent.getGameController(), mEvent.getAcceleration());

                            break;
                        case GCconst.CODE_GYRO:
                            onGyroListener(mEvent.getGameController(), mEvent.getGyro());
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

        }
	}

}

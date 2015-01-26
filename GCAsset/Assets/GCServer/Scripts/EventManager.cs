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

    public delegate void EventListener(GameController gc, int code, byte[] value);
	public delegate void AccelerationListener(GameController gc,Acceleration acceleration);
	public delegate void GyroListener(GameController gc,Gyro gyro);

    public event EventListener onEventListener;
    public event AccelerationListener onAccelerationListener;
    public event GyroListener onGyroListener;

    float[] sensorBuffer = new float[10];
    public class Event
    {
        int code;
        string sensorName;
        GameController mGameController;
        object Sensor;

        public GameController getGameController()
        {
            return mGameController;
        }

        public Gyro getGyro()
        {
            if(code!=GCconst.CODE_GYRO)return null;
            else return (Gyro)Sensor;
        }

        public Acceleration getAcceleration()
        {
            if (code != GCconst.CODE_ACCELERATION) return null;
            else return (Acceleration)Sensor;
        }

        public string getSensorName()
        {
            return sensorName;
        }

        public int getCode()
        {
            return code;
        }
        public Event(GameController gc,int code,object sensor)
        {
            this.mGameController = gc;
            this.code = code;
            if (code == GCconst.CODE_GYRO) sensorName = "GYRO";
            else if (code == GCconst.CODE_ACCELERATION) sensorName = "ACCELERATION";
            this.Sensor = sensor;
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

	Queue EventQueue;

    public void setEventQueue(Queue queue)
    {
        this.EventQueue = queue;
	}

	public void init(){
        
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
	public void receiveEvent(GameController gc,int code,byte[] value){
		//Todo:이벤트 처리를 어떻게 할 것인지 정해야함
        Buffer.BlockCopy(value, 0, sensorBuffer, 0, value.Length);
        switch (code){
            case GCconst.CODE_GYRO:
                EventQueue.Enqueue(new Event(gc, code, new Gyro(sensorBuffer)));
                break;
            case GCconst.CODE_ACCELERATION:
                EventQueue.Enqueue(new Event(gc, code, new Acceleration(sensorBuffer)));
                break;
        }
		return;
	}

	/**
	 * Queue에 쌓인 이벤트들을 처리한다.
	 * 메인스레드에서 동작시켜야한다.
	 */ 
	public void processEvent(){
		//Todo : 각 이벤트에 대한 필터 제공
        Event mEvent = (Event)EventQueue.Dequeue();

		/**
		 * 이벤트 종류에 대한 리스너 처리
		 */
        switch (mEvent.getCode())
        {
		    case GCconst.CODE_ACCELERATION:
                onAccelerationListener(mEvent.getGameController(), mEvent.getAcceleration());
                
			    break;
		    case GCconst.CODE_GYRO:
                onGyroListener(mEvent.getGameController(), mEvent.getGyro());
			    break;
		}
		
		/**
		 * 게임 컨트롤러에 대한 리스너 처리
		 */
        mEvent.getGameController().receiveEvent(mEvent);
	}

}

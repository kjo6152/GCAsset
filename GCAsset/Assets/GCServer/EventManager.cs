using UnityEngine;
using System.Collections;
using System.Threading;

/**
 * 3. 이벤트 메니저
 * 기능 :
 *  1) 게임서버 매니저로부터 받은 이벤트를 관리한다.
 *  2) 게임 개발자는 이벤트 매니저에 리스너를 등록하여 이벤트를 받아 처리로직을 구현한다. 
 *  3) 이벤트 매니저에 필터를 구현하여 등록할 수 있으며 기본적으로 제공하는 필터를 사용할 수 있다.
 */
public class EventManager {

	public delegate void onEventListener(GameController gc,int code,byte[] value);
	public delegate void onAccelerationListener(GameController gc,byte[] value);
	public delegate void onGyroListener(GameController gc,byte[] value);

	event onEventListener mEventListener;
	event onAccelerationListener mAccelerationListener;
	event onGyroListener mGyroListener;

	public class EventQueue{
		Queue AccelerationEvent;
		Queue GyroEvent;
		Queue SystemEvent;
		Queue ButtonEvent;
		
		public EventQueue(){
			AccelerationEvent = new Queue();
			GyroEvent = new Queue();
			SystemEvent = new Queue();
			ButtonEvent = new Queue();
		}
	}

	EventQueue mEventQueue;

	public void setEventQueue(EventQueue queue){
		this.mEventQueue = queue;
	}

	public void init(){

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/**
	 * 클라이언트로부터 온 이벤트를 ServerManager에게서 받아 처리한다.
	 * 각각 알맞은 Queue에 쌓는 역할을 한다.
	 */ 
	public void receiveEvent(GameController gc,int code,byte[] value){
		//Todo:이벤트 처리를 어떻게 할 것인지 정해야함
		return;
	}

	/**
	 * Queue에 쌓인 이벤트들을 처리한다.
	 * 메인스레드에서 동작시켜야한다.
	 */ 
	public void processEvent(GameController gc,int code,byte[] value){
		//Todo : 각 이벤트에 대한 필터 제공
		
		mEventListener (gc, code, value);
		
		/**
		 * 이벤트 종류에 대한 리스너 처리
		 */ 
		switch (code) {
		case GCconst.CODE_ACCELERATION:
			mAccelerationListener(gc,value);
			break;
		case GCconst.CODE_GYRO:
			mGyroListener(gc,value);
			break;
		}
		
		/**
		 * 게임 컨트롤러에 대한 리스너 처리
		 */
	}

}

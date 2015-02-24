using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

/**
 * @breif 게임 컨트롤러(클라이언트)에 대한 정보를 갖는 클래스 
 * @details 클라이언트의 디바이스 이름, 해상도를 갖는다.
 * @see getDeviceName
 * @see getResolutionX
 * @see getResolutionY
 * 이벤트 매니저에 등록한 것과 같이 컨트롤러에 대해서만 이벤트를 받을 수 있으며 이벤트 매니저 호출 직후 호출된다.
 * 컨트롤러에게 전송한 씬을 로드하게 하거나 효과음, 진동에 대한 이펙트를 발생시키고 싶은 경우 이 객체를 이용해야 한다.
 * @see ServerManager.getControllerList
 * @see sendSound
 * @see sendVibration
 * @see sendChangeView
 * @author jiwon
 */
public class GameController {
	const string XML_ELEMENT_DEVICE = "Device";
	const string XML_ELEMENT_NAME = "Name";
	const string XML_ELEMENT_RESOLUTIONX = "ResolutionX";
	const string XML_ELEMENT_RESOLUTIONY = "ResolutionY";
	
	ServerManager.GCPacketProcessor processor;
	string mDeviceName;
	int mResolutionX, mResolutionY;
	
	public GameController(ServerManager.GCPacketProcessor processor){
		this.processor = processor;
        onControllerConnected = delegate { };
        onControllerDisconnected = delegate { };
        onControllerComplete = delegate { };
        onAccelerationListener = delegate { };
        onGyroListener = delegate { };
        onButtonListener = delegate { };
        onDirectionKeyListener = delegate { };
        onJoystickListener = delegate { };

	}

    /** @breif 컨트롤러 디바이스의 이름을 얻는 매소드 */ 
	public string getDeviceName(){
		return mDeviceName;
	}
	
    /** @breif 컨트롤러 디바이스의 x 해상도를 얻는 매소드 */ 
	public int getResolutionX(){
		return mResolutionX;
	}
	
    /** @breif 컨트롤러 디바이스의 y 해상도를 얻는 매소드 */ 
	public int getResolutionY(){
		return mResolutionY;
	}

    public void removeFrom(ArrayList list)
    {
        list.Remove(this.processor);
    }

    /** @breif 컨트롤러와 연결을 끊는다. */ 
    public void disconnect()
    {
        processor.stopProcessor();
    }

	/**
	 * Xml 스트링으로부터 게임 컨트롤러 변수를 설정한다.
     * 컨트롤러가 연결되어 정보를 전송하면 해당 정보를 활용한다.
	 */ 
	public void readDeviceDataFromXml(string xml){
		Debug.Log ("readDeviceDataFromXml");
		try{
			XElement device = XElement.Parse (xml);
			mDeviceName = device.Element (XML_ELEMENT_NAME).Value;
			mResolutionX = Convert.ToInt32(device.Element (XML_ELEMENT_RESOLUTIONX).Value);
			mResolutionY = Convert.ToInt32(device.Element (XML_ELEMENT_RESOLUTIONY).Value);
			Debug.Log ("Name : " + mDeviceName);
			Debug.Log ("ResolutionX : " + mResolutionX);
			Debug.Log ("ResolutionY : " + mResolutionY);
		}catch {
			return;
		}
	}

    /** @breif 컨트롤러에게 효과음을 재생하도록 요청한다. AssetBundle 형태로 전송된 효과음만 가능하다. */ 
    public void sendSound(string name)
	{
		processor.sendSound (name);
	}

    /** @breif 컨트롤러에게 해당 시간만큼 진동을 일으키도록 요청한다. */ 
    public void sendVibration(int time)
	{
		processor.sendVibration (time);
	}

    /** @breif 컨트롤러에게 씬을 로드하도록 한다. AssetBundle 형태로 전송된 씬만 가능하다. */ 
    public void sendChangeView(string SceneName)
    {
        processor.sendChangeView(SceneName);
    }
	
    /** @see EventManager.SystemEventListener */
    public delegate void SystemEventListener();
    /** @see EventManager.AccelerationListener */
    public delegate void AccelerationListener(EventManager.AccelerationEvent acceleration);
    /** @see EventManager.GyroListener */
    public delegate void GyroListener(EventManager.GyroEvent gyro);
    /** @see EventManager.ButtonListener */
    public delegate void ButtonListener(EventManager.ButtonEvent buttonEvent);
    /** @see EventManager.DirectionKeyListener */
    public delegate void DirectionKeyListener(EventManager.DirectionKeyEvent directionKeyEvent);
    /** @see EventManager.JoystickListener */
    public delegate void JoystickListener(EventManager.JoystickEvent joystickEvent);

    /** @see EventManager.onControllerConnected */
    public event SystemEventListener onControllerConnected;
    /** @see EventManager.onControllerDisconnected */
    public event SystemEventListener onControllerDisconnected;
    /** @see EventManager.onControllerComplete */
    public event SystemEventListener onControllerComplete;
    /** @see EventManager.onAccelerationListener */
    public event AccelerationListener onAccelerationListener;
    /** @see EventManager.onGyroListener */
    public event GyroListener onGyroListener;
    /** @see EventManager.onButtonListener */
    public event ButtonListener onButtonListener;
    /** @see EventManager.onDirectionKeyListener */
    public event DirectionKeyListener onDirectionKeyListener;
    /** @see EventManager.onJoystickListener */
    public event JoystickListener onJoystickListener;

    /** @see EventManager.receiveEvent */
    public void receiveEvent(EventManager.Event mEvent)
    {
        /**
		 * 이벤트 종류에 대한 리스너 처리
		 */
        if (mEvent.getType() == GCconst.TYPE_SYSTEM)
        {
            switch (mEvent.getCode())
            {
                case GCconst.CODE_CONNECTED:
                    onControllerConnected();
                    break;
                case GCconst.CODE_DISCONNECTED:
                    onControllerDisconnected();
                    break;
                case GCconst.CODE_COMPLETE:
                    onControllerComplete();
                    break;
            }
        }
        else if (mEvent.getType() == GCconst.TYPE_EVENT)
        {
            switch (mEvent.getCode())
            {
                case GCconst.CODE_BUTTON:
                    onButtonListener(mEvent.getButtonEvent());
                    break;
                case GCconst.CODE_DIRECTION_KEY:
                    onDirectionKeyListener(mEvent.getDirectionKeyEvent());
                    break;
                case GCconst.CODE_JOYSTICK:
                    onJoystickListener(mEvent.getJoystickEvent());
                    break;
            }
        }
        else if (mEvent.getType() == GCconst.TYPE_SENSOR)
        {
            switch (mEvent.getCode())
            {
                case GCconst.CODE_ACCELERATION:
                    onAccelerationListener(mEvent.getAccelerationEvent());
                    break;
                case GCconst.CODE_GYRO:
                    onGyroListener(mEvent.getGyroEvent());
                    break;
            }
        }
    }
}
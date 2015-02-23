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
* 게임 컨트롤러(클라이언트)에 대한 정보를 갖는 클래스 
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
	
	public string getDeviceName(){
		return mDeviceName;
	}
	
	public int getResolutionX(){
		return mResolutionX;
	}
	
	public int getResolutionY(){
		return mResolutionY;
	}

    public void removeFrom(ArrayList list)
    {
        list.Remove(this.processor);
    }

    public void disconnect()
    {
        processor.stopProcessor();
    }
	/**
	 * Xml 스트링으로부터 게임 컨트롤러 변수를 설정한다.
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

    public void sendSound(string name)
	{
		processor.sendSound (name);
	}

    public void sendVibration(int time)
	{
		processor.sendVibration (time);
	}
    public void sendChangeView(string SceneName)
    {
        processor.sendChangeView(SceneName);
    }
	
	/**
	 * 게임 컨트롤러를 통한 이벤트 처리
	 */
    public delegate void SystemEventListener();
    public delegate void AccelerationListener(EventManager.AccelerationEvent acceleration);
    public delegate void GyroListener(EventManager.GyroEvent gyro);
    public delegate void ButtonListener(EventManager.ButtonEvent buttonEvent);
    public delegate void DirectionKeyListener(EventManager.DirectionKeyEvent directionKeyEvent);
    public delegate void JoystickListener(EventManager.JoystickEvent joystickEvent);

    public event SystemEventListener onControllerConnected;
    public event SystemEventListener onControllerDisconnected;
    public event SystemEventListener onControllerComplete;
    public event AccelerationListener onAccelerationListener;
    public event GyroListener onGyroListener;
    public event ButtonListener onButtonListener;
    public event DirectionKeyListener onDirectionKeyListener;
    public event JoystickListener onJoystickListener;

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
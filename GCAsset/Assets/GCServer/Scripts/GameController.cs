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

	public void sendSoundEffect(string name)
	{
		processor.sendEffect (GCconst.CODE_SOUND, name);
	}

	public void  sendVibrationEffect(int count)
	{
		processor.sendEffect (GCconst.CODE_VIBRATION, count);
	}
	
	/**
	 * 게임 컨트롤러를 통한 이벤트 처리
	 */
    public delegate void EventListener(int code, byte[] value);
    public delegate void AccelerationListener(EventManager.Acceleration acceleration);
    public delegate void GyroListener(EventManager.Gyro gyro);
	
	public event EventListener onEventListener;
    public event AccelerationListener onAccelerationListener;
    public event GyroListener onGyroListener;

    public void receiveEvent(EventManager.Event mEvent)
    {
        /**
		 * 이벤트 종류에 대한 리스너 처리
		 */
        switch (mEvent.getCode())
        {
            case GCconst.CODE_ACCELERATION:
                onAccelerationListener(mEvent.getAcceleration());
                break;
            case GCconst.CODE_GYRO:
                onGyroListener(mEvent.getGyro());
                break;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.IO;

public class ClientManager : MonoBehaviour {
    string mResourcesPath;
    int mResourceLength = 0;

    const string NAME_RESOURCE_DIR = "/RemoteGameController/Resources";
    const string NAME_RESOURCE_MAP = "/ResourceMap.xml";

    private AsyncCallback mRecvPacketCallback;
    private AsyncCallback mRecvFileCallback;
    private AsyncCallback mRecvEventCallback;
    /**
	 * XML에서 쓰이는 엘리먼트 이름과 속성 이름
	 */
    const string XML_ELEMENT_RESOURCE = "Resource";
    const string XML_ATTR_ID = "id";
    const string XML_ATTR_NAME = "name";
    const string XML_ATTR_TYPE = "type";
    const string XML_ATTR_SIZE = "size";

    XElement mResourceMap = null;

    public Text ipAddress;
    private Socket mSocket;  /* client Socket */
    
    int recvSize,remainSize, fileSize;

    private const int MAXSIZE   = 4096;		/* 4096  */
    private byte[] recvBuffer = new byte[MAXSIZE];

    //private string HOST ="211.189.19.49";
    private string HOST ="127.0.0.1";
    private int PORT = 6000;
    




    // Use this for initialization
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    string getDeviceInfo()
    {
        XElement Device = new XElement("Device");
        XElement Name = new XElement("Name");
        XElement ResolutionX = new XElement("ResolutionX");
        XElement ResolutionY = new XElement("ResolutionY");

        Name.Value = SystemInfo.deviceModel;
        ResolutionX.Value = Screen.resolutions[0].width.ToString();
        ResolutionY.Value = Screen.resolutions[0].height.ToString();

        Device.Add(Name);
        Device.Add(ResolutionX);
        Device.Add(ResolutionY);

        Debug.Log(Device.ToString());
        return Device.ToString();
    }

    public void onClick()
    {
        Debug.Log("onClick");
        connect();
    }
        
    public void init()
    {
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        mRecvPacketCallback = new AsyncCallback(recvPacketCallback);
        mRecvFileCallback = new AsyncCallback(recvFileCallback);
        mRecvEventCallback = new AsyncCallback(recvEventCallback);
        mResourcesPath = Application.dataPath + NAME_RESOURCE_DIR;
    }

        

    public void connect()
    {
        try
        {
            mSocket.BeginConnect(HOST, PORT, new AsyncCallback(ConnectCallBack), mSocket);
        }
        catch (SocketException se)
        {
            Debug.Log("서버 접속 실패 ; " + se.NativeErrorCode);
        }
    }


    /*****************************************
    * 
    * 
    *              전송 관련 콜백
    * 
    * 
    *****************************************/
    /**
     * 서버와 컨넥션시 호출
     * 초기 연결을 수행한다.
     */ 
    void ConnectCallBack(IAsyncResult IAR)
    {
        try
        {
            Socket socket = (Socket)IAR.AsyncState;
            IPEndPoint svrEP = (IPEndPoint)socket.RemoteEndPoint;
            Debug.Log("address : " + svrEP.Address);

            socket.EndConnect(IAR);
            setupResources();
            
        }
        catch (SocketException se)
        {
            if (se.SocketErrorCode == SocketError.NotConnected)
            {
                Debug.Log("\r\n서버 접속 실패 CallBack " + se.Message);
            }
        }
    }

    /**
     * 리소스 파일을 받는 경우 호출한다.
     */ 
    void recvFileCallback(IAsyncResult IAR)
    {
        FileStream sw = (FileStream)IAR.AsyncState;
        recvSize = mSocket.EndReceive(IAR);
        if (recvSize > 0)
        {
            sw.Write(recvBuffer, 0, recvSize);
            remainSize -= recvSize;
            if (remainSize > 0)
            {
                mSocket.BeginReceive(recvBuffer, 0, remainSize > MAXSIZE ? MAXSIZE : remainSize, SocketFlags.None, mRecvFileCallback, mSocket);
                return;
            }
            else
            {
                sw.Close();
            }
        }
        mSocket.BeginReceive(recvBuffer, 0, 16, SocketFlags.None, mRecvPacketCallback, mSocket);
    }

    /**
     * 이벤트를 받는 경우 호출한다.
     */
    void recvEventCallback(IAsyncResult IAR)
    {
        mSocket.BeginReceive(recvBuffer, 0, 16, SocketFlags.None, mRecvPacketCallback, mSocket);
    }

    /**
     * 패킷을 받는 경우 호출한다.
     */ 
    void recvPacketCallback(IAsyncResult IAR)
    {
        recvSize = mSocket.EndReceive(IAR);
        if (recvSize != 0)
        {
            PacketData packet = getPacketData(recvBuffer);

            Debug.Log("Receive Packet !!");
            Debug.Log("type : "+packet.type);
            Debug.Log("code : " + packet.code);
            Debug.Log("value : " + packet.value);

            switch (packet.type)
            {
                //리소스에 대한 전송
                case GCconst.TYPE_RESOURCE:
                    string path = "";
                    remainSize = packet.value;
                    //리소스맵에 대한 전송일 경우
                    if (packet.code == 0)
                    {
                        //리소스 맵의 경로 설정
                        path = mResourcesPath + NAME_RESOURCE_MAP;
                    }
                    else
                    {
                        //리소스맵에 대한 정보가 없을경우 파일로부터 리소스맵 리드
                        if (mResourceMap == null)
                        {
                            string xml = new StreamReader(mResourcesPath+NAME_RESOURCE_MAP).ReadToEnd();
                            mResourceMap = XElement.Parse(xml);
                            IEnumerator elements = mResourceMap.Elements(XML_ELEMENT_RESOURCE).GetEnumerator();
                            mResourceLength = 0;
                            while (elements.MoveNext())
                            {
                                mResourceLength++;
                            }
                        }
                        //파일 경로 설정
                        path = getResourcePath(packet.code);
                    }
                    FileStream fs = new FileStream(path, FileMode.Create);
                    mSocket.BeginReceive(recvBuffer, 0, remainSize > MAXSIZE ? MAXSIZE : remainSize, SocketFlags.None, mRecvFileCallback, fs);
                    break;
                case GCconst.TYPE_EVENT:
                    mSocket.BeginReceive(recvBuffer, 0, remainSize > MAXSIZE ? MAXSIZE : remainSize, SocketFlags.None, mRecvEventCallback, null);
                    break;
            }
        }
    }

    /**
     * 서버와 초기 연결시 디바이스 정보 전송 및 리소스 다운로드
     */
    void setupResources()
    {
        //디바이스 정보 버퍼
        byte[] deviceInfo = new UTF8Encoding().GetBytes(getDeviceInfo());
        //패킷 버퍼
        byte[] sendBuffer = getPacketByteArray(GCconst.TYPE_CONTROLLER, 0, deviceInfo.Length);
        //정보 전송
        mSocket.BeginSend(sendBuffer, 0, recvBuffer.Length, SocketFlags.None, null, mSocket);
        mSocket.BeginSend(deviceInfo, 0, deviceInfo.Length, SocketFlags.None, null, mSocket);
        //리소스 다운로드
        mSocket.BeginReceive(recvBuffer, 0, 16, SocketFlags.None, mRecvPacketCallback, mSocket);
    }

    void destory()
    {
        if (mSocket.IsBound) mSocket.Close();
        mSocket = null;
    }
    
    /*****************************************
     * 
     * 
     *              이벤트 전송
     * 
     * 
     *****************************************/

    /**
     * 서버로 자이로 센서 이벤트를 보낸다.
     * Todo : 추가 데이터 크기 및 형식 결정
     */
    public void sendSensor(Gyroscope gyroscope)
    {
        //Todo : 보낼 패킷 생성 -> 추가 데이터 크기는?
        byte[] sendBuffer = getPacketByteArray(GCconst.TYPE_EVENT, GCconst.CODE_GYRO, 0);
        //패킷 전송
        mSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, null, mSocket);
        //Todo : 자이로 이벤트를 보내야 한다. 어떻게 보낼 것인가? 형식
        //mSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, null, mSocket);
    }

    /**
     * 서버로 가속도 센서 이벤트를 보낸다.
     * Todo : 추가 데이터 크기 및 형식 결정
     */
    public void sendSensor(Vector3 acceleration)
    {
        //Todo : 보낼 패킷 생성 -> 추가 데이터 크기는?
        byte[] sendBuffer = getPacketByteArray(GCconst.TYPE_EVENT, GCconst.CODE_ACCELERATION, 0);
        //패킷 전송
        mSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, null, mSocket);
        //Todo : 가속도 이벤트를 보내야 한다. 어떻게 보낼 것인가? 형식
        //mSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, null, mSocket);
    }

    /**
     * 서버로 버튼 등의 
     * Todo : 추가 데이터 크기 및 형식 결정
     */
    public void sendEvent(ushort type, ushort code, int value)
    {

    }




    /*****************************************
     * 
     * 
     *              패킷 처리 
     * 
     * 
     *****************************************/

    /**
	 * 안드로이드 게임 컨트롤러와 주고 받을 패킷
	 * 시간의 경우 운영체제의 종류에 따라 크기 차이가 있을 수 있지만 여서는 8byte로 고정한다.
	 */
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct PacketData
    {
        public uint tv_sec;
        public uint tv_usec;
        public ushort type;
        public ushort code;
        public int value;
    }

    /**
	* type, code, value를 가진 패킷의 Byte Array를 만든다.
	*/
    byte[] getPacketByteArray(ushort type, ushort code, int value)
    {
        PacketData packet = new PacketData();
        packet.type = type;
        packet.code = code;
        packet.value = value;
        return getByteArray(packet);
    }

	/**
		 * byte 배열을 통해 GCPacket을 만든다.
		 */ 
	static PacketData getPacketData(byte[] data){
		return (PacketData)(ByteToStructure (data, typeof(PacketData)));
	}
	
	/**
		 * byte 배열을 구조체로 만드는 매소드
		 */
	static object ByteToStructure(byte[] data, Type type)
	{
		IntPtr buff = Marshal.AllocHGlobal(data.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
		Marshal.Copy(data, 0, buff, data.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
		object obj = Marshal.PtrToStructure(buff, type); // 복사된 데이터를 구조체 객체로 변환한다.
		Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
		return obj; // 구조체 리턴
	}
	
	/**
		 * 클래스에 포함된 PacketData를 배열로 만든다.
		 */ 
	byte[] getByteArray(PacketData packet){
		return StructureToByte (packet);
	}
	
	
	/**
		 * 구조체로 byte 배열을 만드는 매소드
		 */
	static byte[] StructureToByte(object obj)
	{
		int datasize = Marshal.SizeOf(obj);//((PACKET_DATA)obj).TotalBytes; // 구조체에 할당된 메모리의 크기를 구한다.
		IntPtr buff = Marshal.AllocHGlobal(datasize); // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
		Marshal.StructureToPtr(obj, buff, false); // 할당된 구조체 객체의 주소를 구한다.
		byte[] data = new byte[datasize]; // 구조체가 복사될 배열
		Marshal.Copy(buff, data, 0, datasize); // 구조체 객체를 배열에 복사
		Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
		return data; // 배열을 리턴
	}

    /**
	 * 코드(아이디)를 가진 리소스 엘리먼트를 리턴한다.
	 */
    XElement getElement(int code)
    {
        IEnumerator elements = mResourceMap.Elements(XML_ELEMENT_RESOURCE).GetEnumerator();
        XElement element;
        XAttribute attr;
        while (elements.MoveNext())
        {
            element = (XElement)elements.Current;
            attr = element.Attribute(XML_ATTR_ID);
            if (attr != null && Convert.ToInt32(attr.Value) == code) return element;
        }
        return null;
    }


    /**
     * 코드(리소스의 아이디)를 통해 해당 리소스가 저장 패스를 리턴해준다.
     */
    public string getResourcePath(int code)
    {

        XElement element = getElement(code);
        if (element == null) return null;
        return mResourcesPath + NAME_RESOURCE_DIR + element.Attribute(XML_ATTR_NAME).Value;
    }

    /**
     * 코드(아이디)를 가진 리소스의 크기를 리턴한다.
     */
    public int getResourceSize(int code)
    {
        XElement element = getElement(code);
        if (element == null) return -1;
        return Convert.ToInt32(element.Attribute(XML_ATTR_SIZE).Value);
    }

    /**
     * 리소스가 저장된 위치를 리턴한다.
     */
    public string getResourceDirectory()
    {
        return mResourcesPath;
    }

    /**
     * 리소스의 개수를 리턴한다.
     */
    public int getResourceLength()
    {
        return mResourceLength;
    }

    /**
     * 이름을 가진 리소스 엘리먼트를 리턴한다.
     */
    XElement getElement(string name)
    {
        IEnumerator elements = mResourceMap.Elements(XML_ELEMENT_RESOURCE).GetEnumerator();
        XElement element;
        XAttribute attr;
        while (elements.MoveNext())
        {
            element = (XElement)elements.Current;
            attr = element.Attribute(XML_ATTR_NAME);
            if (attr != null && attr.Value == name) return element;
        }
        return null;
    }

    /**
     * 이름을 통해 리소스 타입 속성을 얻는다.
     */
    public string getResourceType(string name)
    {
        XElement element = getElement(name);
        if (element == null) return null;
        return element.Attribute(XML_ATTR_TYPE).Value;
    }

    /**
     * 이름을 통해 리소스 아이디 속성을 얻는다.
     */
    public int getResourceCode(string name)
    {
        XElement element = getElement(name);
        if (element == null) return -1;
        return Convert.ToInt32(element.Attribute(XML_ATTR_ID).Value);
    }

    /**
     * 이름을 통해 리소스 파일 크기 속성을 얻는다.
     */
    public int getResourceSize(string name)
    {
        XElement element = getElement(name);
        if (element == null) return -1;
        return Convert.ToInt32(element.Attribute(XML_ATTR_SIZE).Value);
    }

    public string getResourceMap()
    {
        return mResourcesPath + NAME_RESOURCE_MAP;
    }
}

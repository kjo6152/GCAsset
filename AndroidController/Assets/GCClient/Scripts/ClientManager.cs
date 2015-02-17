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
using System.Xml.Serialization;
/**
 * 2. 게임서버 매니저
 * 기능 :
 *  1) 게임 서버에 대한 관리를 한다. 
 *  2) IP 통신에 대한 서버를 오픈한다.
 *  3) 클라이언트가 접속하면 디바이스에 대한 정보를 받아 컨트롤러 객체를 생성한다. 생성된 컨트롤러 객체로부터 디바이스에 대한 정보를 얻을 수 있다.
 *  4) 리소스 매니저를 통해 초기 리소스를 전송한다.
 *  5) 컨트롤러로부터 여러 데이터들을 전송받고 이벤트를 이벤트 매니저로 전달해준다.
 *  6) 게임 서버에 대한 정보를 얻는다. ( 아이피, 맥 어드레스 및 디바이스명 등등)
 * 
 */
public class ClientManager {
	/**
	 * 할당될 포트의 최소갑과 최대값 및 여러 서버에 관련된 설정 변수들
	 */
	const int minPort = 6000, maxPort = 7000, maxPlayer = 5;
	const int fixPort = 6000;
    const int gyroDataSize = sizeof(float) * 10;
    const int accelDataSize = sizeof(float) * 3;
	/**
	 * 게임 서버의 기본 아이피(자기 자신)과 포트
	 * 포트는 게임 서버 초기화시 임의로 정해진다.
	 */
    string mIPAddress;
    int mPort;
    bool isReady;
	Socket mClientSocket;
	Thread mClientThread;
	IPEndPoint mServerEndPoint;
	ResourceManager mResourceManager;
	EventManager mEventManager;
	GCPacketProcessor mProcessor;
	// Use this for initialization
	void Start () {
		Debug.Log ("start");
		this.init ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/**
	 * 의존성 주입
	 * Todo : 의존성을 어떻게 떨어뜨릴 것인가?
	 */
	public void setResourceMeneager(ResourceManager rm){
		this.mResourceManager = rm;
	}

	public void setEventManager(EventManager em){
		this.mEventManager = em;
	}
	/**
	 * 서버 초기화 작업을 수행한다.
	 * 랜덤 포트 생성 작업 수행
	 */
	public void init(){
        Debug.Log("init");
		mProcessor = null;
        mClientThread = null;
        isReady = false;
	}

	/**
	 * 생성된 소켓을 실제 서버에 바인딩하고 Accept하는 스레드를 생성한다.
	 */
    public void startClient(string ip,int port)
    {
        mIPAddress = ip;
        mPort = port;
        Debug.Log("startClient");
		
		if (mClientThread != null) {
			Debug.Log("Client already started");
			return;
		}
		
		mClientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        mServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        mClientThread = new Thread(connectServer);
		mClientThread.Start ();
	}

	/**
	 * 클라이언트를 기다린다.
	 * 접속이 될 경우 초기화 작업을 수행한다.
	 */ 
	void connectServer(){
        Debug.Log("connectServer");
		try{
            mClientSocket.Connect(mServerEndPoint);
            onServerConnected();
			//패킷프로세서 및 GCClient 객체 생성
			mProcessor = new GCPacketProcessor();
            mProcessor.setSocket(mClientSocket);
            mProcessor.setEventManager(mEventManager);
            mProcessor.setResourceMeneager(mResourceManager);
			//게임 컨트롤러에 대한 이벤트 추가
            mProcessor.onConnectListener += new GCPacketProcessor.Listener(onServerConnected);
            mProcessor.onDisconnectListener += new GCPacketProcessor.Listener(onServerDisconnected);
            mProcessor.onCompleteListener += new GCPacketProcessor.Listener(onServerCompleted);
			//프로세서 시작
            mProcessor.startProcessor();
		}
		//서버를 종료시키고자 인터럽트가 발생했을 때 수행한다.
		catch(ThreadAbortException){
            Debug.Log("ThreadAbortException");
			Thread.ResetAbort();
		}
	}

	/**
	 * 게임컨트롤러가 연결되었을 경우 발생하는 이벤트
	 */ 
	void onServerConnected(){
        Debug.Log("onServerConnected");
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
        mEventManager.receiveEvent(GCconst.TYPE_SYSTEM, GCconst.CODE_CONNECTED, null);
	}

	/**
	 * 게임컨트롤러가 연결이 끊겼을 경우 발생하는 이벤트
	 */
    void onServerDisconnected()
    {
        Debug.Log("onServerDisconnected");
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
        mEventManager.receiveEvent(GCconst.TYPE_SYSTEM, GCconst.CODE_DISCONNECTED, null);
	}

	/**
	 * 게임 컨트롤러 연결이 완료되었을 경우 발생하는 이벤트
	 */
    void onServerCompleted()
    {
        Debug.Log("onServerCompleted");
        isReady = true;
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
        mEventManager.receiveEvent(GCconst.TYPE_SYSTEM, GCconst.CODE_COMPLETE, null);
	}

	/**
	 * 스레드에 abort를 호출하여 종료시킨다.
	 */ 
	public void stopClient(){
        Debug.Log("stopClient");
		
		if (mClientThread == null) {
			Debug.Log ("Client already stopped");
			return;
		}
        mClientThread.Abort();
        destroyClient();
	}

	/**
	 * 서버에 관련된 리소스를 해제한다.
	 */
    void destroyClient()
    {
        Debug.Log("destroyClient");
        if(mProcessor!=null)mProcessor.stopProcessor();
        mClientSocket = null;
        mProcessor = null;
		mClientThread = null;
	}

	/**
	 * 서버 관려 변수 get 매소드
	 */ 
	public int getPort(){
		return mPort; 
	}

	public string getIPAddress(){
        return mIPAddress;
	}

    public bool isRunning()
    {
        return mClientThread == null ? false : true;
    }

    /**
     * 서버로 이벤트를 보낸다.
     * 버튼 클릭이나 조이스틱 관련 이벤트
     */
    public void sendEvent(ushort code, int value)
    {
        if (mProcessor != null && isReady) mProcessor.sendEvent(code, value);
    }

    /**
     * 서버로 자이로 센서 이벤트를 보낸다.
     */
    public void sendSensor(Gyroscope gyro)
    {
        if (mProcessor != null && isReady) mProcessor.sendSensor(gyro);
    }

    /**
     * 서버로 자이로 센서 이벤트를 보낸다.
     */
    public void sendSensor(AccelerationEvent acceleration)
    {
        if (mProcessor != null && isReady) mProcessor.sendSensor(acceleration);
    }

	/**
	 * 클라이언트 소켓으로부터 패킷을 받아 처리하는 클래스
	 */
	public class GCPacketProcessor{
        const int BUFFER_SIZE = 4096;
		Socket mSocket;
		Thread mThread;
		byte[] recvBuffer;
        byte[] sendBuffer;

		ResourceManager mResourceManager;
		EventManager mEventManager;

		public delegate void Listener();
		public event Listener onConnectListener;
		public event Listener onDisconnectListener;
		public event Listener onCompleteListener;

		/**
	 	* 안드로이드 게임 컨트롤러와 주고 받을 패킷
	 	* 시간의 경우 운영체제의 종류에 따라 크기 차이가 있을 수 있지만 여서는 8byte로 고정한다.
	 	*/
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct PacketData{
			public uint tv_sec;
			public uint tv_usec;
			public ushort type;
			public ushort code;
			public int value;
		}

		public GCPacketProcessor(){
			mThread = new Thread(receivePacket);
            recvBuffer = new byte[BUFFER_SIZE];
            sendBuffer = new byte[BUFFER_SIZE];
		}

		public void setResourceMeneager(ResourceManager rm){
			this.mResourceManager = rm;
		}
		
		public void setEventManager(EventManager em){
			this.mEventManager = em;
		}

		public void setSocket(Socket socket){
			this.mSocket = socket;
		}

		/**
		 * PacketData 구조체의 크기를 리턴한다. (고정값)
		 */ 
		public static int getSize(){
			return 16;
		}

		/**
		 * GCPacketProcessor를 동작시킨다.
		 */
        public void startProcessor()
        {
            //먼저 자신의 정보를 전송한다.
            sendDeviceInfo();
			mThread.Start ();
		}

		/**
		 * 클라이언트 소켓으로부터 패킷을 받는다.
		 */ 
		void receivePacket(){
            onConnectListener();
			try{
				while (true) {
					Debug.Log("receivePacket");
                    if (mSocket.Receive(recvBuffer, GCPacketProcessor.getSize(), 0) <= 0)
                    {
                        this.stopProcessor();
                    }
                    processPacket(GCPacketProcessor.getPacketData(recvBuffer));
				}
			}
			catch(Exception e){
                Debug.Log(e);
			}

		}

		/**
		 * 클라이언트 소켓으로부터 받은 패킷을 처리한다.
		 * 주로 리소스와 이벤트를 주고 받는다.
		 * 이벤트와 센서만 ResourceManager로 전달하여 처리하도록 한다.
		 */
		void processPacket(PacketData packet){
			Debug.Log ("processPacket type : "+packet.type+" code : "+packet.code+" value : "+packet.value);
			switch (packet.type) {
			    //일반 이벤트
			    case GCconst.TYPE_EVENT:
                    mSocket.Receive(recvBuffer, packet.value, 0);
                    if (packet.code == GCconst.CODE_SOUND || packet.code == GCconst.CODE_VIEW)
                    {
                        string name = Encoding.Default.GetString(recvBuffer, 0, packet.value);
                        mEventManager.receiveEvent(packet.type, packet.code, name);
                    }
                    else if (packet.code == GCconst.CODE_VIBRATION)
                    {
                        mEventManager.receiveEvent(packet.type, packet.code, BitConverter.ToInt32(recvBuffer, 0));
                    }
                    
                    break;
			    //센서 이벤트
			    case GCconst.TYPE_SENSOR:
				    break;
			    /**
			     * 게임 컨트롤러에 대한 정보 패킷
			     * 첫 연결시 서버로 전송한다.
			     */ 
			    case GCconst.TYPE_CONTROLLER:
				    break;
			    /**
			     * 리소스 패킷
			     * 서버로부터 리소스를 다운받는다.
			     * 컨트롤러에 대한 정보 전송 후 바로 다운받는다.
			     */
			    case GCconst.TYPE_RESOURCE:
                    receiveResource(packet.code, packet.value);
				    break;
			    /**
			     * 리소스를 다 받았을 때 클라이언트에서 보내주는 패킷
			     * 여기서는 호출되지 않는다.
			     */ 
			    case GCconst.TYPE_ACK:
				    //연결 완료 이벤트를 발생한다.
				    break;
			}

		}

        /*
         * 리소스를 받는다.
         */
		void receiveResource(ushort code,int value){
            Debug.Log("receiveResource code : "+code+" value : "+value);
            int remainSize = value,receiveSize=0;
            string path = mResourceManager.getResourcePath(code);
            FileStream writer = new FileStream(path,FileMode.Create);
			while(remainSize>0){
                Debug.Log("receiveResource : " + remainSize);
                receiveSize = mSocket.Receive(recvBuffer, remainSize > BUFFER_SIZE ? BUFFER_SIZE : remainSize, SocketFlags.None);
                writer.Write(recvBuffer,0,receiveSize);
                remainSize -= receiveSize;
            }
            writer.Close();
            if (code == 0)
            {
                Debug.Log("receiveResourceMap");
                mResourceManager.LoadResourceMap(path);
            }
            else if (code == mResourceManager.getResourceLength())
            {
                byte[] packetBuffer = getPacketByteArray(GCconst.TYPE_ACK, 0, 0);
                mSocket.Send(packetBuffer, packetBuffer.Length, 0);
                onCompleteListener();
            }
            Debug.Log("End receiveResource");
		}

        /*
         * 디바이스의 정보를 전송한다.
         */
		void sendDeviceInfo(){
            Debug.Log("sendDeviceInfo");
            string xml = mResourceManager.getDeviceXml();
			byte[] xmlBuffer = new UTF8Encoding().GetBytes(xml);
			//보낼 패킷 생성
			byte[] packetBuffer = getPacketByteArray (GCconst.TYPE_CONTROLLER, 0, xmlBuffer.Length);
			//패킷 전송
			mSocket.Send(packetBuffer,packetBuffer.Length,0);
			//데이터 전송
			mSocket.Send(xmlBuffer,xmlBuffer.Length,0);
		}

		/**
		 * 서버로 이벤트를 보낸다.
         * 버튼 클릭이나 조이스틱 관련 이벤트
		 */ 
		public void sendEvent(ushort code,int value)
        {
            //byte[] packet = getPacketByteArray(GCconst.TYPE_EVENT, code, value);
            //mSocket.Send(packet, packet.Length, 0);
		}

        /**
         * 서버로 자이로 센서 이벤트를 보낸다.
         * userAcceleration에 가속도도 포함되어있다.
         * 자이로
         *  - rotationRate : x, y, z
         *  - gravity : x, y, z
         *  - attitude : x, y, z, w
         * 가속도
         *  - userAcceleration : x, y, z
         */
        public void sendSensor(Gyroscope gyro)
        {
            try
            {
                //자이로 센서 전송
                float[] gyroData = new float[10];
                //rotationRate
                gyroData[0] = gyro.rotationRate.x;
                gyroData[1] = gyro.rotationRate.y;
                gyroData[2] = gyro.rotationRate.z;
                //gravity
                gyroData[3] = gyro.gravity.x;
                gyroData[4] = gyro.gravity.y;
                gyroData[5] = gyro.gravity.z;
                //attitude
                gyroData[6] = gyro.attitude.x;
                gyroData[7] = gyro.attitude.y;
                gyroData[8] = gyro.attitude.z;
                gyroData[9] = gyro.attitude.w;

                byte[] packet = getPacketByteArray(GCconst.TYPE_SENSOR, GCconst.CODE_GYRO, gyroDataSize);
                mSocket.Send(packet, packet.Length, 0);

                Buffer.BlockCopy(gyroData, 0, sendBuffer, 0, gyroDataSize);
                mSocket.Send(sendBuffer, gyroDataSize, 0);

                //가속도 센서 전송
                float[] accelData = new float[3];
                accelData[0] = gyro.userAcceleration.x;
                accelData[1] = gyro.userAcceleration.y;
                accelData[2] = gyro.userAcceleration.z;

                packet = getPacketByteArray(GCconst.TYPE_SENSOR, GCconst.CODE_ACCELERATION, accelDataSize);
                mSocket.Send(packet, packet.Length, 0);

                Buffer.BlockCopy(accelData, 0, sendBuffer, 0, accelDataSize);
                mSocket.Send(sendBuffer, accelDataSize, 0);
            }
            catch
            {
                stopProcessor();
            }
            
		}

        /**
		 * 서버로 자이로 센서 이벤트를 보낸다.
		 */
        public void sendSensor(AccelerationEvent acceleration)
        {
            //byte[] packet = getPacketByteArray(GCconst.TYPE_SENSOR, GCconst.CODE_ACCELERATION, 0);
            //mSocket.Send(packet, packet.Length, 0);

        }
        
		/**
		 * 패킷 프로세서를 중단한다.
		 */ 
		public void stopProcessor(){
            Debug.Log("stopProcessor");
			if (mThread == null) {
				Debug.Log ("Server already stopped");
				return;
            }
            destroyProcessor();
		}

		void destroyProcessor(){
            Debug.Log("destroyProcessor");
            onDisconnectListener();
            mSocket.Shutdown(SocketShutdown.Both);
			mSocket.Close ();
            mSocket = null;
			mThread = null;
		}

		/**
		 * type, code, value를 가진 패킷의 Byte Array를 만든다.
		 */
		byte[] getPacketByteArray(ushort type,ushort code, int value)
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
	}
}

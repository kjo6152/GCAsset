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
 * @breif GCAsset과 컨트롤러간 통신을 담당하는 클래스
 * @details 서버 및 컨트롤러 관리, 네트워크를 담당한다. </br>
 * 서버를 시작하거나 종료시킬 수 있으며 서버는 스레드로 동작한다.
 * @see startServer, stopServer
 * 서버에 관련하여 IP, Port, 서버 상태 정보를 얻을 수 있다.
 * @see getIPAddress, getPort, isRunning
 * 또한 연결된 컨트롤러의 목록을 얻을 수 있다.
 * @see getControllerList
 * @author jiwon
 */
public class ServerManager {
	/**
	 * 할당될 포트의 최소갑과 최대값 및 여러 서버에 관련된 설정 변수들
	 */
	const int minPort = 6000, maxPort = 7000, maxPlayer = 5;
	const int fixPort = 6000;
	/**
	 * 게임 서버의 기본 아이피(자기 자신)과 포트
	 * 포트는 게임 서버 초기화시 임의로 정해진다.
	 */
	int mPort;
	Socket mServerSocket;
	Thread mServerThread;
	IPEndPoint mServerEndPoint;
	ResourceManager mResourceManager;
	EventManager mEventManager;
	ArrayList mProcessorList;
	// Use this for initialization
	void Start () {
		Debug.Log ("start");
		this.init ();
        this.startServer();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setResourceMeneager(ResourceManager rm){
		this.mResourceManager = rm;
	}

	public void setEventManager(EventManager em){
		this.mEventManager = em;
	}

    /**
     * @breif 서버 초기화 작업을 수행한다.
     * @details 보안을 위해 포트를 랜덤으로 생성한다.
     */
    public void init(){
		Debug.Log ("initServer");
		System.Random rand = new System.Random ();
		//mPort = rand.Next(minPort, maxPort);
		mPort = fixPort;
		mProcessorList = ArrayList.Synchronized(new ArrayList ());
        mServerThread = null;
	}

    /**
     * @breif 서버를 시작하고 플레이어를 대기하는 상태로 들어간다.
     */
    public void startServer(){
		Debug.Log ("startServer");
		
		if (mServerThread != null) {
			Debug.Log("Server already started");
			return;
		}
		
		mServerSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		mServerEndPoint = new IPEndPoint (IPAddress.Any, mPort);
		mServerSocket.Bind (mServerEndPoint);
		mServerSocket.Listen (maxPlayer);
		mServerThread = new Thread (waitPlayer);
		mServerThread.Start ();
	}

	/**
	 * 클라이언트를 기다린다.
	 * 접속이 될 경우 초기화 작업을 수행한다.
	 */ 
	void waitPlayer(){
		Debug.Log ("waitPlayer");
		try{
			while(true){
				Socket client = mServerSocket.Accept ();
                client.NoDelay = true;
				//패킷프로세서 및 GCClient 객체 생성
				GCPacketProcessor processor = new GCPacketProcessor();
				mProcessorList.Add(processor);
				processor.setSocket(client);
				processor.setEventManager(mEventManager);
				processor.setResourceMeneager(mResourceManager);
				//게임 컨트롤러에 대한 이벤트 추가
                processor.onConnectListener += new GCPacketProcessor.Listener(onGameControllerConnected);
                processor.onDisconnectListener += new GCPacketProcessor.Listener(onGameControllerDisconnected);
                processor.onCompleteListener += new GCPacketProcessor.Listener(onGameControllerCompleted);
				//프로세서 시작
				processor.startProcessor();
			}
		}
        catch (Exception e)
        {
            Debug.Log(e);
        }
        Debug.Log("end waitPlayer");
	}

	/**
	 * 게임컨트롤러가 연결되었을 경우 발생하는 이벤트
	 */ 
	void onGameControllerConnected(GameController gc){
        Debug.Log("onGameControllerConnected : "+gc.getDeviceName());
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
        mEventManager.receiveEvent(gc, GCconst.TYPE_SYSTEM, GCconst.CODE_CONNECTED, null);
	}

	/**
	 * 게임컨트롤러가 연결이 끊겼을 경우 발생하는 이벤트
	 */ 
	void onGameControllerDisconnected(GameController gc){
        Debug.Log("onGameControllerDisconnected : " + gc.getDeviceName());
        gc.removeFrom(this.mProcessorList);
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
        mEventManager.receiveEvent(gc, GCconst.TYPE_SYSTEM, GCconst.CODE_DISCONNECTED, null);
	}

	/**
	 * 게임 컨트롤러 연결이 완료되었을 경우 발생하는 이벤트
	 */ 
	void onGameControllerCompleted(GameController gc){
        Debug.Log("onGameControllerCompleted : " + gc.getDeviceName());
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
        mEventManager.receiveEvent(gc, GCconst.TYPE_SYSTEM, GCconst.CODE_COMPLETE, null);
	}

    /**
     * @breif 서버를 종료하고 자원을 해제한다.
     */
	public void stopServer(){
		Debug.Log ("stopServer");
		
		if (mServerThread == null) {
			Debug.Log ("Server already stopped");
			return;
		}
        destroyServer();
	}

	/**
	 * 서버에 관련된 리소스를 해제한다.
	 */
	void destroyServer(){
        Debug.Log("destroyServer");
        for (int i = 0; i < mProcessorList.Count; i++)
        {
            ((GCPacketProcessor)mProcessorList[i]).stopProcessor();
        }
		mProcessorList.Clear ();
		if (mServerSocket != null) {
			mServerSocket.Close ();
            mServerSocket=null;
		}
		mServerThread = null;
	}

    public ArrayList getControllerList()
    {
        ArrayList ControllerList = new ArrayList();
        for(int i=0;i<mProcessorList.Count;i++){
            GCPacketProcessor processor = (GCPacketProcessor)mProcessorList[i];
            ControllerList.Add(processor.getGameController());
        }
        return ControllerList;
    }

    /**
     * @breif 포트를 리턴하는 매소드
     */
    public int getPort()
    {
		return mPort; 
	}

    /**
     * @breif IP 리스트를 리턴하는 매소드
     */
	public string[] getIPAddress(){
        ArrayList ipList = new ArrayList();
		IPHostEntry host = Dns.GetHostEntry (Dns.GetHostName ());
		foreach (IPAddress ip in host.AddressList) {
			if(ip.AddressFamily == AddressFamily.InterNetwork)
			{
				ipList.Add(ip.ToString());
			}
		}
        string[] ipArray = new string[ipList.Count];
        ipList.CopyTo(ipArray);
        return ipArray;
	}

    /**
     * @breif 현재 서버의 동작 상태를 리턴하는 매소드
     */
    public bool isRunning()
    {
        return mServerThread == null ? false : true;
    }

    /**
     * @breif 컨트롤러에게서 패킷을 받아 처리하는 클래스
     * @details 각 컨트롤러마다 할당되며 실제 통신이 일어나는 클래스이다.
     * 패킷을 분석하여 그에 알맞는 처리를 하거나 다른 객체로 전달해준다.
     * 해당 컨트롤러에게 이벤트를 전달해준다.
     * @see sendSound, sendVibration, sendChangeView
     */
    public class GCPacketProcessor{
        const int BUFFER_SIZE = 4096;
		Socket mSocket;
		StreamWriter mStreamWriter;
        NetworkStream mNetworkStream;
		GameController mGameController;
		Thread mThread;
		byte[] recvBuffer;
        //byte[] sendBuffer;
		ResourceManager mResourceManager;
		EventManager mEventManager;

		public delegate void Listener(GameController gc);
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
            //sendBuffer = new byte[BUFFER_SIZE];
		}

		public void setResourceMeneager(ResourceManager rm){
			this.mResourceManager = rm;
		}
		
		public void setEventManager(EventManager em){
			this.mEventManager = em;
		}

		public void setSocket(Socket socket){
			this.mSocket = socket;
            this.mNetworkStream = new NetworkStream(mSocket);
            this.mStreamWriter = new StreamWriter(mNetworkStream);
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
		public void startProcessor(){
            Debug.Log("startProcessor");
			mThread.Start ();
		}

		/**
		 * 클라이언트 소켓으로부터 패킷을 받는다.
		 */ 
		void receivePacket(){
			try{
				while (true) {
					//Debug.Log("receivePacket");
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
            Debug.Log("end receivePacket");
		}

		/**
		 * 클라이언트 소켓으로부터 받은 패킷을 처리한다.
		 * 주로 리소스와 이벤트를 주고 받는다.
		 * 이벤트와 센서만 ResourceManager로 전달하여 처리하도록 한다.
		 */
		void processPacket(PacketData packet){
            switch (packet.type) {
			//일반 이벤트
			case GCconst.TYPE_EVENT:
			//센서 이벤트
			case GCconst.TYPE_SENSOR:
				//Todo : 이벤트에 대한 처리
				mSocket.Receive(recvBuffer,packet.value,0);
                mEventManager.receiveEvent(mGameController, packet.type, packet.code, recvBuffer);
				break;
			/**
			 * 게임 컨트롤러에 대한 정보 패킷
			 * 해당 xml 파일을 읽어서 게임 컨트롤러 객체를 만든다.
			 * 연결 이벤트를 발생시킨다.
			 */
            case GCconst.TYPE_RESOURCE:
				//컨트롤러에 대한 정보를 받는다.
				int count,remain;
				string xml="";
				remain = packet.value;

				//게임 컨트롤러에 대한 정보
				while(remain>0){
                    count = mSocket.Receive(recvBuffer, remain, 0);
                    xml += Encoding.Default.GetString(recvBuffer, 0, count);
					remain -= count;
				}
				//받은 xml 파일을 읽는다.
				//xml 스트링으로부터 게임 컨트롤러 객체를 만든다.
				mGameController = new GameController(this);
				mGameController.readDeviceDataFromXml(xml);
				//연결 이벤트를 발생한다.
				onConnectListener(this.mGameController);
				//리소스맵과 리소스를 전송한다.
				sendResourceMap();
				sendReousrces();
				break;
			/**
			 * 리소스를 다 받았을 때 클라이언트에서 보내주는 패킷
			 * 완료이벤트를 발생시킨다.
			 */ 
			case GCconst.TYPE_ACK:
				//Debug.Log ("processPacket : TYPE_ACK");
				//연결 완료 이벤트를 발생한다.
                onCompleteListener(this.mGameController);
				break;
			}

		}


		void sendReousrces(){
            Debug.Log("sendReousrces");
			int size=0,count = mResourceManager.getResourceLength ();
            byte[] buf;
            int remain,offset,ret;
			for (ushort i=1; i<=count; i++) {
				size = mResourceManager.getResourceSize(i);
                buf = getPacketByteArray(GCconst.TYPE_RESOURCE, i, size);
                mSocket.Send(buf, buf.Length, 0);
                remain = size;
                offset = 0;
                buf = mResourceManager.getResourceByte(i);
                while (remain > 0)
                {
                    Debug.Log("size : " + remain);
                    ret = mSocket.Send(buf,offset,remain,SocketFlags.None);
                    remain -= ret;
                    offset += ret;
                }
			}
		}

		void sendResourceMap(){
			Debug.Log ("sendResourceMap");
            string xml = mResourceManager.getResourceMap();
			byte[] xmlBuffer = new UTF8Encoding().GetBytes(xml);
			//보낼 패킷 생성
			byte[] packetBuffer = getPacketByteArray (GCconst.TYPE_RESOURCE, 0, xmlBuffer.Length);
			//패킷 전송
			mSocket.Send(packetBuffer,packetBuffer.Length,0);
			//데이터 전송
			mSocket.Send(xmlBuffer,xmlBuffer.Length,0);
		}

        /**
         * @breif 음향 효과 이벤트를 보낸다.
         * @params name 재생할 음향 효과 명 (확장자 제외)
         */
		public void sendSound(string name){
            byte[] strBuffer = new UTF8Encoding().GetBytes(name);
            //보낼 패킷 생성
			byte[] packetBuffer = getPacketByteArray (GCconst.TYPE_EVENT, GCconst.CODE_SOUND, strBuffer.Length);
            //패킷 전송
            mSocket.Send(packetBuffer,packetBuffer.Length,0);
            //데이터 전송
            mSocket.Send(strBuffer,strBuffer.Length,0);
		}

		/**
		 * @breif 진동에 대한 이벤트를 보낸다.
         * @params time 진동 시간이다.
		 */
		public void sendVibration(int time){
			//보낼 패킷 생성
			byte[] packetBuffer = getPacketByteArray (GCconst.TYPE_EVENT, GCconst.CODE_VIBRATION, 4);
			//패킷 전송
            mSocket.Send(packetBuffer, packetBuffer.Length, 0);
			//데이터 전송
			mStreamWriter.Write (time);
		}

        /**
		 * @breif 씬을 로드하라는 이벤트를 보낸다.
         * @params SceneName 로드할 씬 이름 (확장자 제외)
		 */
        public void sendChangeView(string SceneName)
        {
            byte[] strBuffer = new UTF8Encoding().GetBytes(SceneName);
            //보낼 패킷 생성
            byte[] packetBuffer = getPacketByteArray(GCconst.TYPE_EVENT, GCconst.CODE_VIEW, strBuffer.Length);
            //패킷 전송
            mSocket.Send(packetBuffer, packetBuffer.Length, 0);
            //데이터 전송
            mSocket.Send(strBuffer, strBuffer.Length, 0);
        }

        /** @breif 연결된 컨트롤러를 얻는 매소드 */
        public GameController getGameController(){
            return mGameController;
        }
		
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
            mSocket.Shutdown(SocketShutdown.Both);
			mSocket.Close ();
            mSocket = null;
			mThread = null;
            onDisconnectListener(this.mGameController);
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

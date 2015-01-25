﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System;

using System.Text;
using System.Xml;
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
		Debug.Log ("initServer");
		System.Random rand = new System.Random ();
		//mPort = rand.Next(minPort, maxPort);
		mPort = fixPort;
		mProcessorList = ArrayList.Synchronized(new ArrayList ());
        mServerThread = null;
	}

	/**
	 * 생성된 소켓을 실제 서버에 바인딩하고 Accept하는 스레드를 생성한다.
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
				//패킷프로세서 및 GCClient 객체 생성
				GCPacketProcessor processor = new GCPacketProcessor();
				mProcessorList.Add(processor);
				processor.setSocket(client);
				processor.setEventManager(mEventManager);
				processor.setResourceMeneager(mResourceManager);
				//게임 컨트롤러에 대한 이벤트 추가
				processor.mConnectListener += new GCPacketProcessor.onListener(onGameControllerConnected);
				processor.mDisconnectListener += new GCPacketProcessor.onListener(onGameControllerDisconnected);
                processor.mCompleteListener += new GCPacketProcessor.onListener(onGameControllerCompleted);
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
	}

	/**
	 * 게임컨트롤러가 연결이 끊겼을 경우 발생하는 이벤트
	 */ 
	void onGameControllerDisconnected(GameController gc){
        Debug.Log("onGameControllerDisconnected : " + gc.getDeviceName());
        gc.removeFrom(this.mProcessorList);
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
	}

	/**
	 * 게임 컨트롤러 연결이 완료되었을 경우 발생하는 이벤트
	 */ 
	void onGameControllerCompleted(GameController gc){
        Debug.Log("onGameControllerCompleted : " + gc.getDeviceName());
		//Todo : 라이브러리쪽으로 이벤트를 처리할 수 있도록 큐에 저장
	}

	/**
	 * 스레드에 abort를 호출하여 종료시킨다.
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

	/**
	 * 서버 관려 변수 get 매소드
	 */ 
	public int getPort(){
		return mPort; 
	}

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

    public bool isRunning()
    {
        return mServerThread == null ? false : true;
    }
	
	/**
	 * 클라이언트 소켓으로부터 패킷을 받아 처리하는 클래스
	 */
	public class GCPacketProcessor{
        const int BUFFER_SIZE = 4096;
		Socket mSocket;
		StreamWriter mStreamWriter;
		GameController mGameController;
		Thread mThread;
		byte[] buffer;
		ResourceManager mResourceManager;
		EventManager mEventManager;

		public delegate void onListener(GameController gc);
		public event onListener mConnectListener;
		public event onListener mDisconnectListener;
		public event onListener mCompleteListener;

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
            buffer = new byte[BUFFER_SIZE];
		}

		public void setResourceMeneager(ResourceManager rm){
			this.mResourceManager = rm;
		}
		
		public void setEventManager(EventManager em){
			this.mEventManager = em;
		}

		public void setSocket(Socket socket){
			this.mSocket = socket;
			this.mStreamWriter = new StreamWriter (new NetworkStream(mSocket));
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
					Debug.Log("receivePacket");
                    if (mSocket.Receive(buffer, GCPacketProcessor.getSize(), 0) <= 0)
                    {
                        this.stopProcessor();
                    }
                        
					processPacket(GCPacketProcessor.getPacketData (buffer));
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
            Debug.Log("processPacket type : " + packet.type + " code : " + packet.code + " value : " + packet.value);
			switch (packet.type) {
			//일반 이벤트
			case GCconst.TYPE_EVENT:
			//센서 이벤트
			case GCconst.TYPE_SENSOR:
				Debug.Log ("processPacket : event");
				//Todo : 이벤트에 대한 처리
				byte[] eventBuffer = new byte[packet.value];
				mSocket.Receive(eventBuffer,eventBuffer.Length,0);
				mEventManager.receiveEvent(mGameController,packet.code,eventBuffer);
				break;
			/**
			 * 게임 컨트롤러에 대한 정보 패킷
			 * 해당 xml 파일을 읽어서 게임 컨트롤러 객체를 만든다.
			 * 연결 이벤트를 발생시킨다.
			 */ 
			case GCconst.TYPE_CONTROLLER:
				Debug.Log ("processPacket : TYPE_CONTROLLER");
				//컨트롤러에 대한 정보를 받는다.
				int count,remain;
				string xml="";
				remain = packet.value;

				//게임 컨트롤러에 대한 정보
				while(remain>0){
					count = mSocket.Receive(buffer,remain,0);
					xml += Encoding.Default.GetString(buffer,0,count);
					remain -= count;
				}
				//받은 xml 파일을 읽는다.
				//xml 스트링으로부터 게임 컨트롤러 객체를 만든다.
				mGameController = new GameController(this);
				mGameController.readDeviceDataFromXml(xml);
				//연결 이벤트를 발생한다.
				mConnectListener(this.mGameController);
				//리소스맵과 리소스를 전송한다.
				sendResourceMap();
				sendReousrces();
				break;
			/**
			 * 리소스 요청 패킷
			 * 해당하는 리소스를 찾아서 클라이언트로 전송해준다.
			 * 클라이언트 접속 후 바로 리소스를 모두 전송하는 형태로 변경되면서 사용하지 않음
			 */
			case GCconst.TYPE_RESOURCE:
				break;
			/**
			 * 리소스를 다 받았을 때 클라이언트에서 보내주는 패킷
			 * 완료이벤트를 발생시킨다.
			 */ 
			case GCconst.TYPE_ACK:
				Debug.Log ("processPacket : TYPE_ACK");
				//연결 완료 이벤트를 발생한다.
				mCompleteListener(this.mGameController);
				break;
			}

		}


		void sendReousrces(){
            Debug.Log("sendReousrces");
			int size=0,count = mResourceManager.getResourceLength ();
			string path;
            byte[] buf;
			for (ushort i=1; i<=count; i++) {
				path = mResourceManager.getResourcePath(i);
				size = mResourceManager.getResourceSize(i);
                buf = getPacketByteArray(GCconst.TYPE_RESOURCE, i, size);
                mSocket.Send(buf, buf.Length, 0);
                mSocket.SendFile(path);
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
		 * 클라이언트로 이벤트 패킷을 보낸다.
		 * name을 가진 리소스를 검색하여 해당 리소스의 아이디를 전송한다.
		 */ 
		public void sendEffect(ushort code,string name){
			sendEffect (code, mResourceManager.getResourceCode (name));
		}

		/**
		 * 클라이언트로 이벤트 패킷을 보낸다.
		 * 서버측에서는 TYPE_EVENT에 대한 패킷만 생성하여 전송한다.
		 * data는 리소스에 대한 id나 진동 강도이다.
		 */
		public void sendEffect(ushort code,int id){
			//보낼 패킷 생성
			byte[] sendBuffer = getPacketByteArray (GCconst.TYPE_EVENT, code, 4);
			//패킷 전송
			mSocket.Send(sendBuffer,sendBuffer.Length,0);
			//데이터 전송
			mStreamWriter.Write (id);
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
            mSocket.Shutdown(SocketShutdown.Both);
			mSocket.Close ();
            mSocket = null;
			mThread = null;
            mDisconnectListener(this.mGameController);
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

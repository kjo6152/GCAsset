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
/**
 * 2. 게임서버 매니저
 * 기능 :
 *  1) 게임 서버에 대한 관리를 한다. 
 *  2) IP / Bluetooth 통신에 대한 서버를 오픈한다.
 *  3) 클라이언트가 접속하면 디바이스에 대한 정보를 받아 컨트롤러 객체를 생성한다. 생성된 컨트롤러 객체로부터 디바이스에 대한 정보를 얻을 수 있다.
 *  4) 리소스 매니저를 통해 초기 리소스를 전송한다.
 *  5) 컨트롤러로부터 여러 데이터들을 전송받고 이벤트를 이벤트 매니저로 전달해준다.
 *  6) 게임 서버에 대한 정보를 얻는다. ( 아이피, 맥 어드레스 및 디바이스명 등등)
 * 
 * 매소드 :
 *  1) Initialize
 *  2) openBluetoothServer / close ~
 *  3) openInternetServer / close ~
 *  4) getConnectedDevice(s)
 *  5) getServerDevice
 *  6) isOpened 
 */
public class ServerManager : MonoBehaviour {
	/**
	 * 게임 서버의 기본 아이피(자기 자신)과 포트
	 * 포트는 게임 서버 초기화시 임의로 정해진다.
	 */
	int mPort;
	Socket mServerSocket;
	ArrayList mClientSockets;
	Thread mServerThread;
	IPEndPoint mServerEndPoint;

	/**
	 * 할당될 포트의 최소갑과 최대값 및 여러 서버에 관련된 설정 변수들
	 */
	int minPort = 6000, maxPort = 7000;
	int maxPlayer = 5;
	/**
	 * 서버 초기화 작업을 수행한다.
	 * 랜덤 포트 생성 작업 수행
	 */

	void initSever(){
		Debug.Log ("initServer");
		System.Random rand = new System.Random ();
		mPort = rand.Next(minPort, maxPort);
		mClientSockets = new ArrayList ();
	}

	/**
	 * 서버에 관련된 리소스를 해제한다.
	 */
	void destroyServer(){
		if (mServerSocket != null) {
			mServerSocket.Close ();
			mServerSocket=null;
		}
		foreach(Socket client in mClientSockets){
			client.Close();
		}
		mClientSockets = null;
	}

	/**
	 * 생성된 소켓을 실제 서버에 바인딩하고 Accept하는 스레드를 생성한다.
	 */
	void startServer(){
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

	void stopServer(){
		Debug.Log ("stopServer");

		if (mServerThread == null) {
			Debug.Log ("Server already stopped");
			return;
		}

	}

	void waitPlayer(){
		Debug.Log ("waitPlayer");

		try{
			while(true){
				Socket client = mServerSocket.Accept ();
				mClientSockets.Add(client);
			}

		}
		//서버를 종료시키고자 인터럽트가 발생했을 때 수행한다.
		catch(ThreadAbortException){
			Thread.ResetAbort();
			destroyServer();
		}
	}

	/**
	 * 클라이언트와 초기 연결시 수행할 작업들
	 * 1. 클라이언트에 대한 정보 획득
	 * 2. 
	 */ 
	void initClient(Socket client){
		//
		client.Receive ();
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("start");
		this.initSever ();
		this.startServer ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	class GCPacket{
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

		//Nullable Structure
		PacketData? mPacketData;

		/**
		 * byte 배열을 통해 GCPacket을 만든다.
		 */ 
		public static GCPacket getInstance(byte[] data){
			GCPacket tGCPacket = new GCPacket ();
			tGCPacket.mPacketData = (PacketData)(ByteToStructure (data, typeof(PacketData)));
			if (tGCPacket.mPacketData == null)return null;
			return tGCPacket;
		}

		/**
		 * byte 배열을 구조체로 만드는 매소드
		 */
		private static object ByteToStructure(byte[] data, Type type)
		{
			IntPtr buff = Marshal.AllocHGlobal(data.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
			Marshal.Copy(data, 0, buff, data.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
			object obj = Marshal.PtrToStructure(buff, type); // 복사된 데이터를 구조체 객체로 변환한다.
			Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함
			
			if (Marshal.SizeOf(obj) != data.Length)// (((PACKET_DATA)obj).TotalBytes != data.Length) // 구조체와 원래의 데이터의 크기 비교
			{
				return null; // 크기가 다르면 null 리턴
			}
			return obj; // 구조체 리턴
		}

		/**
		 * 클래스에 포함된 PacketData를 배열로 만든다.
		 */ 
		public byte[] getByteArray(){
			return StructureToByte (this.mPacketData);
		}


		/**
		 * 구조체로 byte 배열을 만드는 매소드
		 */
		private static byte[] StructureToByte(object obj)
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

	/**
	 * 게임 컨트롤러(클라이언트)에 대한 정보를 갖는 클래스
	 */ 
	class GameController{
		string mDeviceName;
		int mResolutionX, mResolutionY;

		void init(string xml){
			readDeviceDataFromXml (xml);

		}

		void readDeviceDataFromXml(string xml){
			XmlReader reader = XmlReader.Create (new StringReader (xml));

			reader.ReadToFollowing ("Device");

			//디바이스 이름
			reader.ReadToFollowing ("Name");
			this.mDeviceName = reader.ReadElementContentAsString ();
			//해상도
			reader.ReadToFollowing ("ResolutionX");
			this.mResolutionX = reader.ReadElementContentAsInt ();
			reader.ReadToFollowing ("ResolutionY");
			this.mResolutionY = reader.ReadElementContentAsInt ();
		}
	}
}

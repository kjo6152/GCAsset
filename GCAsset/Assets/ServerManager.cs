using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

public class ServerManager : MonoBehaviour {
	/**
	 * 게임 서버의 기본 아이피(자기 자신)과 포트
	 * 포트는 게임 서버 초기화시 임의로 정해진다.
	 */
	IPAddress mIPAddress;
	int mPort;
	Socket mServerSocket;
	Socket[] mClientSockets;
	Thread mServerThread;
	IPEndPoint mServerEndPoint;

	/**
	 * 할당될 포트의 최소갑과 최대값 및 여러 서버에 관련된 설정 변수들
	 */
	int minPort = 6000, maxPort = 7000;
	int maxPlayer = 5;
	/**
	 * 서버 초기화 작업을 수행한다.
	 * 랜덤 포트 생성, 소켓 생성, 
	 */
	void initSever(){
		Debug.Log ("initServer");
		mPort = Random.Range (minPort, maxPort);

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

	void waitPlayer(){
		mServerSocket.Accept ();
	}

	// Use this for initialization
	void Start () {
		Debug.Log ("start");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

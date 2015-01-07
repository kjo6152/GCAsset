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
	Thread mServerThread;
	IPEndPoint mServerEndPoint;

	/**
	 * 루프백 아이피오 할당될 포트의 최소갑과 최대값
	 */
	string loopback = "127.0.0.1";
	int minPort = 6000, maxPort = 7000;

	void initSever(){
		mIPAddress = IPAddress.Parse(loopback);
		mPort = Random.Range (minPort, maxPort);
		mServerSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		mServerEndPoint = new IPEndPoint (mIPAddress, mPort);

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

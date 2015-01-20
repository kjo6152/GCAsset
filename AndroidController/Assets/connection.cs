using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Xml;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

public class connection : MonoBehaviour {

        public Text ipAddress;
        private Socket clientSock;  /* client Socket */
        private Socket cbSock;      /* client Async Callback Socket */
        

        private const int MAXSIZE   = 4096;		/* 4096  */
        //private string HOST ="211.189.19.49";
		private string HOST ="127.0.0.1";
        private int PORT = 6000;
        public bool connectState = false;
        private byte[] recvBuffer = new byte[MAXSIZE];

        XmlDocument xmldoc;
        
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
         public void connect()
         {


             if (!connectState)
             {
                 xmldoc = new XmlDocument();
                 xmldoc.AppendChild(xmldoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
                 XmlNode root = xmldoc.CreateElement("", "device", "");
                 xmldoc.AppendChild(root);

                 //XmlNode FristNode = xmldoc.DocumentElement;
                 //XmlElement r = xmldoc.CreateElement("Device");
                 root.AppendChild(CreateNode(xmldoc, "Name", SystemInfo.deviceModel));
                 root.AppendChild(CreateNode(xmldoc, "ResolutionX", Screen.resolutions[0].width.ToString()));
                 root.AppendChild(CreateNode(xmldoc, "ResolutionY", Screen.resolutions[0].height.ToString()));
                 //FristNode.AppendChild(r);
                 xmldoc.Save("device.xml");
                 
                
                 Debug.Log( " device Model:" + SystemInfo.deviceModel);
               
                 Debug.Log(" device resulution width :" + Screen.resolutions[0].width);
                 Debug.Log(" device resulution height :" + Screen.resolutions[0].height);
            
                 Debug.Log(" device height:" + Screen.height);
                 Debug.Log(" device width:" + Screen.width);

                 DoInit();
             }
              // xml 파일 읽기

             XmlTextReader reader = new XmlTextReader("device.xml");

             Debug.Log("xml to string : " +GetXMLAsString(xmldoc));
             this.BeginSend(GetXMLAsString(xmldoc));
            
         }
         public string GetXMLAsString(XmlDocument myxml)
         {
             return myxml.OuterXml;
         }
         protected XmlNode CreateNode(XmlDocument xmlDoc, string name, string innerXml)
         {
             XmlNode node = xmlDoc.CreateElement(string.Empty, name, string.Empty);
             node.InnerXml = innerXml;

             return node;
         }

         public void DoInit()
         {
             connectState = true;
             clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
             this.BeginConnect();
         }

         /*----------------------*
          *		Connection		*
          *----------------------*/
         public void BeginConnect()
         {
             Debug.Log("서버 접속 대기 중");
             
             try
             {
                 
                 clientSock.BeginConnect(HOST, PORT, new AsyncCallback(ConnectCallBack), clientSock);
                 Debug.Log( "beginConnect1 : " + clientSock.Connected);
             }
             catch (SocketException se)
             {
                 /*서버 접속 실패 */
                 Debug.Log("서버 접속 실패 ; " + se.NativeErrorCode);
                 
                 this.DoInit();

             }

         }

         /*----------------------*
          * ##### CallBack #####	*
          *		Connection		*
          *----------------------*/
         private void ConnectCallBack(IAsyncResult IAR)
         {
             try
             {
                 // 보류중인 연결을 완성
         
                 Socket tempSock = (Socket)IAR.AsyncState;
           
                 IPEndPoint svrEP = (IPEndPoint)tempSock.RemoteEndPoint;
                 Debug.Log("address : "+svrEP.Address);
                 
                 tempSock.EndConnect(IAR);
                 cbSock = tempSock;
                 cbSock.BeginReceive(this.recvBuffer, 0, recvBuffer.Length, SocketFlags.None,
                                     new AsyncCallback(OnReceiveCallBack), cbSock);

				
			//	mSocket.Receive (buffer,16,0);
			//PacketData packet = getPacketData(buffer);


             }
             catch (SocketException se)
             {
                 if (se.SocketErrorCode == SocketError.NotConnected)
                 {
                     Debug.Log("\r\n서버 접속 실패 CallBack " + se.Message);
                     
                     this.BeginConnect();
                 }
             }
         }

         /*----------------------*
          *		   Send 		*
          *----------------------*/
         public void BeginSend(string message)
         {
             try
             {
                 /* 연결 성공시 */
                 Debug.Log("beginsend : " + clientSock.Connected);
                 if (clientSock.Connected)
                 {

                     byte[] buffer = new UTF8Encoding().GetBytes(message);

				PacketData packet = new PacketData();
				packet.type = GCconst.TYPE_CONTROLLER;
				packet.code = 0;
				packet.value = buffer.Length;
				byte[] packetBuffer = StructureToByte(packet);

				clientSock.BeginSend(packetBuffer, 0, packetBuffer.Length, SocketFlags.None,
				                     new AsyncCallback(SendCallBack), "packet send");

                     Debug.Log("buffer : " + Encoding.Default.GetString(buffer));
                     clientSock.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                                     new AsyncCallback(SendCallBack), message);
                 }
             }
             catch (SocketException e)
             {
                 Debug.Log("\r\n전송 에러 : " + e.Message);
                 
             }
         }

         /*----------------------*
          * ##### CallBack #####	*
          *		   Send 		*
          *----------------------*/
         private void SendCallBack(IAsyncResult IAR)
         {
             string message = (string)IAR.AsyncState;
             Debug.Log("\r\n전송 완료 CallBack : " + message);
             
         }

         /*----------------------*
          *		  Receive 		*
          *----------------------*/
         public void Receive()
         {
             cbSock.BeginReceive(this.recvBuffer, 0, recvBuffer.Length,
                 SocketFlags.None, new AsyncCallback(OnReceiveCallBack), cbSock);
         }

         /*----------------------*
          * ##### CallBack #####	*
          *		  Receive 		*
          *----------------------*/
         private void OnReceiveCallBack(IAsyncResult IAR)
         {
             try
             {
                 Socket tempSock = (Socket)IAR.AsyncState;
                 int nReadSize = tempSock.EndReceive(IAR);
                 if (nReadSize != 0)
                 {
                     string message = new UTF8Encoding().GetString(recvBuffer, 0, nReadSize);
                     Debug.Log("\r\n서버로 데이터 수신 : " + message);
                     
                 }
                 this.Receive();
             }
             catch (SocketException se)
             {
                 if (se.SocketErrorCode == SocketError.ConnectionReset)
                 {
                     this.BeginConnect();
                 }
             }
         }

    

    void OnGUI()
    {
       // GUI.Label(new Rect(70, 70, 300, 300), ipAddress.text);
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

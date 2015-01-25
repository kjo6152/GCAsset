using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GCServerUsage : MonoBehaviour {
    GCcontext mGCcontext;
	// Use this for initialization
	void Start () {
        mGCcontext = GCcontext.getInstance;
        string[] ipList = mGCcontext.mServerManager.getIPAddress();
        string port = mGCcontext.mServerManager.getPort() + "";
        string str = "";
        foreach (string ip in ipList)
        {
            str += "IP : " + ip + "\r\n";
        }
        GameObject.Find("addressText").GetComponent<Text>().text = str + "port : " + port;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void startOrEndServer()
    {
        mGCcontext = GCcontext.getInstance;
        Debug.Log("startOrEndServer");
        if (mGCcontext.mServerManager.isRunning())
        {
            mGCcontext.mServerManager.stopServer();
        }
        else
        {
            mGCcontext.mServerManager.startServer();
        }
    }

    public void changeScene()
    {
        //Application.LoadLevel("testScene");
    }
}

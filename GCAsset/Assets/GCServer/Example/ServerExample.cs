using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ServerExample : MonoBehaviour {
    GCcontext mGCcontext;
	// Use this for initialization
	void Start () {
        mGCcontext = GCcontext.getInstance;

        string[] ipList = mGCcontext.getServerManager().getIPAddress();
        string port = mGCcontext.getServerManager().getPort() + "";
        string str = "";
        foreach (string ip in ipList)
        {
            str += "IP : " + ip + "\r\n";
        }
        GameObject.Find("addressText").GetComponent<Text>().text = str + "port : " + port;

        mGCcontext.getEventManager().clearListener();
        mGCcontext.getEventManager().onControllerConnected += new EventManager.SystemEventListener(mEventManager_onControllerConnected);
        mGCcontext.getEventManager().onControllerComplete += new EventManager.SystemEventListener(mEventManager_onControllerComplete);
        mGCcontext.getEventManager().onControllerDisconnected += new EventManager.SystemEventListener(mEventManager_onControllerDisconnected);
	}

    void mEventManager_onControllerConnected(GameController gc)
    {
        Debug.Log("onControllerConnected");
    }

    void mEventManager_onControllerComplete(GameController gc)
    {
        Debug.Log("onControllerComplete");
    }
    void mEventManager_onControllerDisconnected(GameController gc)
    {
        Debug.Log("onControllerDisconnected");
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void startOrEndServer()
    {
        mGCcontext = GCcontext.getInstance;
        Debug.Log("startOrEndServer");
        if (mGCcontext.getServerManager().isRunning())
        {
            mGCcontext.getServerManager().stopServer();
        }
        else
        {
            mGCcontext.getServerManager().startServer();
        }
    }

    public void changeScene()
    {
        Debug.Log("changeScene");
        ArrayList ControllerList = mGCcontext.getServerManager().getControllerList();
        for (int i = 0; i < ControllerList.Count; i++)
        {
            GameController controller = (GameController)ControllerList[i];
            controller.sendChangeView("controller");
            //controller.sendSound("GunShot");
        }
    }

    public void NextScene()
    {
        Debug.Log("NextScene");
        Application.LoadLevel("EventExample");
    }
}
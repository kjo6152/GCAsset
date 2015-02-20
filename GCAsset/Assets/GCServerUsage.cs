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

        mGCcontext.mEventManager.onAccelerationListener += new EventManager.AccelerationListener(mEventManager_onAccelerationListener);
        mGCcontext.mEventManager.onGyroListener += new EventManager.GyroListener(mEventManager_onGyroListener);

        //TextAsset asset = Resources.Load("ClickOn") as TextAsset;

	}

    void mEventManager_onGyroListener(GameController gc, EventManager.Gyro gyro)
    {
        Debug.Log("Main gyro Listener");
        Debug.Log("x : " + gyro.x + " / y : " + gyro.y + " / z : " + gyro.z);
    }

    void mEventManager_onAccelerationListener(GameController gc, EventManager.Acceleration acceleration)
    {
        Debug.Log("Main Acceleration Listener");
        Debug.Log("x : " + acceleration.x + " / y : " + acceleration.y + " / z : " + acceleration.z);
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
        Debug.Log("changeScene");
        ArrayList ControllerList = mGCcontext.mServerManager.getControllerList();
        for (int i = 0; i < ControllerList.Count; i++)
        {
            GameController controller = (GameController)ControllerList[i];
            controller.sendChangeView("controller");
            //controller.sendSound("GunShot");
        }
    }
}
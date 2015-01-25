using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class GCClientUsage : MonoBehaviour {
    GCcontext mGCcontext;
    ClientManager mClientManager;
    public Text IPaddress;
    public Text Port;
	// Use this for initialization
	void Start () {
        Debug.Log("Gyroscope : " + SystemInfo.supportsGyroscope);

	}
	
	// Update is called once per frame
	void Update () {
        
        mClientManager = GCcontext.getInstance.mClientManager;

        if (SystemInfo.supportsGyroscope)
        {
            
            Debug.Log("rotation rate : " + Input.gyro.rotationRate);
            Debug.Log("gravity : " + Input.gyro.gravity);
            Debug.Log("attitude : " + Input.gyro.attitude);
            Debug.Log("type : " + Input.gyro.GetType());

            mClientManager.sendSensor(Input.gyro);
        }
	}

    public void ConnectOrDisconnectServer()
    {
        mGCcontext = GCcontext.getInstance;
        if (mGCcontext.mClientManager.isRunning())
        {
            mGCcontext.mClientManager.stopClient();
        }
        else
        {
            Debug.Log("ip address : " + IPaddress.text);
            Debug.Log("port : " + Port.text);
            mGCcontext.mClientManager.startClient(IPaddress.text, Convert.ToInt32(Port.text));
            Application.LoadLevel("ViewController");
        }
        
    }
}

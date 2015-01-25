using UnityEngine;
using System.Collections;

public class GCClientUsage : MonoBehaviour {
    GCcontext mGCcontext;
    ClientManager mClientManager;
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
            mGCcontext.mClientManager.startClient("127.0.0.1", 6000);
        }
        
    }
}

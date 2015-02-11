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
        mGCcontext = GCcontext.getInstance;
	}
	
	// Update is called once per frame
	void Update () {
       
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
        }
    }
}

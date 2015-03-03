using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class onConnection : MonoBehaviour {
    GCcontext mGCcontext;
    void Start()
    {
        mGCcontext = GCcontext.getInstance;
    }
    void Update()
    {
        if (mGCcontext.getClientManager().isRunning())
        {
            GameObject.Find("lable_connectionBtn").GetComponent<Text>().text = "Disconnect";
        }
        else
        {
            GameObject.Find("lable_connectionBtn").GetComponent<Text>().text = "Connect";
        }
    }
    public void OnClickConnection()
    {
        string ip = GameObject.Find("input_ip").GetComponent<InputField>().text;
        string port = GameObject.Find("input_port").GetComponent<InputField>().text;

        if (mGCcontext.getClientManager().isRunning())
        {
            mGCcontext.getClientManager().stopClient();
        }
        else
        {
            mGCcontext.getClientManager().startClient(ip, int.Parse(port));
        }
        
    }
}

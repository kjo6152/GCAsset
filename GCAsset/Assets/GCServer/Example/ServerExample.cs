using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * @breif GCAsset�� �̿��Ͽ� ������ ���� Ŭ���̾�Ʈ�� �����ϴ� ����
 * @details GCAsset�� ���� ��Ʈ�ѷ�(Ŭ���̾�Ʈ)���� ������ ������ ������ ���� ������ ��´�.
 * ���� ��Ʈ�ѷ��� ����Ǿ��� �� �ý��� �̺�Ʈ �����ʸ� ����ϴ� ������ ��Ʈ�ѷ����� ������ ��Ʈ�ѷ� ���� �ε��ϵ���
 * �޽����� ������ ������ �����Ǿ� �ִ�.
 * @author jiwon
 */
public class ServerExample : MonoBehaviour {
    GCcontext mGCcontext;
	// Use this for initialization

    /**
     * @breif ���� ���� ȹ�� �� ������ �ʱ�ȭ, ������ ���
     * @details ������ IP�� Port�� ������ �ý��� �̺�Ʈ�� ����Ѵ�.
     * @see GCcontext
     * @see EventManager
     * @see ServerManager
     */
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

    /**
     * @breif ��Ʈ�ѷ��� ó�� ����Ǿ��� �� �߻��ϴ� �̺�Ʈ�� ������
     * @see SystemEventListener
     * @see GameController
     */
    void mEventManager_onControllerConnected(GameController gc)
    {
        Debug.Log("onControllerConnected");
    }

    /**
     * @breif ��Ʈ�ѷ��� ������ �Ϸ�(���ҽ��� ��� ���۵�)�Ǿ��� �� �߻��ϴ� �̺�Ʈ�� ������
     * @see SystemEventListener
     * @see GameController
     */
    void mEventManager_onControllerComplete(GameController gc)
    {
        Debug.Log("onControllerComplete");
    }

    /**
     * @breif ��Ʈ�ѷ��� ������ ������ �� �߻��ϴ� �̺�Ʈ�� ������
     * @see SystemEventListener
     * @see GameController
     */
    void mEventManager_onControllerDisconnected(GameController gc)
    {
        Debug.Log("onControllerDisconnected");
    }

	// Update is called once per frame
	void Update () {
        if (mGCcontext.getServerManager().isRunning())
        {
            Sprite image = Resources.Load<Sprite>("powerbutton_on");
            Image OnOffImage = GameObject.Find("ServerButton").GetComponentInChildren<Image>();
            OnOffImage.sprite = image;
        }
        else
        {
            Sprite image = Resources.Load<Sprite>("powerbutton_off");
            if (image == null) Debug.Log("null!!");
            Image OnOffImage = GameObject.Find("ServerButton").GetComponentInChildren<Image>();
            OnOffImage.sprite = image;
        }
	}

    /**
     * @breif ������ �����ϰų� �����ϴ� ����
     * @see ServerManager
     */
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

    /**
     * @breif ��Ʈ�ѷ����� �ش� ���� �ε��϶�� �޽����� ������ ����
     * @see ServerManager
     * @see GameController
     */
    public void changeScene()
    {
        Debug.Log("changeScene");
        ArrayList ControllerList = mGCcontext.getServerManager().getControllerList();
        for (int i = 0; i < ControllerList.Count; i++)
        {
            GameController controller = (GameController)ControllerList[i];
            controller.sendChangeView("controller");
        }
    }

    /**
     * @breif EventExample ������ �Ѿ�� �żҵ�
     * @see EventExample
     */
    public void NextScene()
    {
        Debug.Log("NextScene");
        Application.LoadLevel("EventExample");
    }
}
using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
    GCcontext mGCcontext;
	// Use this for initialization
    public GameObject[] panelConnection = new GameObject[4];

	void Start () {
        mGCcontext = GCcontext.getInstance;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void onClickQuitYes()
    {
        Debug.Log("onClickQuitYes");
        Application.Quit();
    }

    public void onClickContinue(){
        Debug.Log("onClickContinue");
        string lastScene = mGCcontext.getResourceManager().getLastScene();
        if (lastScene != null) Application.LoadLevel(lastScene);
    }
    public void onClickConnection()
    {
        Debug.Log("onClickConnection");
        panelConnection[1].SetActive(true);
    }
    public void onClickAbout()
    {
        Debug.Log("onClickAbout");
        panelConnection[2].SetActive(true);
    }
}

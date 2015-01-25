using UnityEngine;
using System.Collections;

public class buttonClick : MonoBehaviour {
    private Sprite spr;
	// Use this for initialization
    private SpriteRenderer renderer;

    public void downClick()
    {
        //Handheld.Vibrate();
        GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log("click");
        GCcontext mGCcontext = GCcontext.getInstance;
        mGCcontext.mClientManager.sendEvent(0, 0);
    }
    public void upClick()
    {

        GetComponent<SpriteRenderer>().color = Color.white;

       

    }
}

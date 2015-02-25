#define CONTROLLER

using UnityEngine;
using System.Collections;

public class JoyStickOnClick : MonoBehaviour {

    /**
     *  Joystick Onclick 관련 코드작성.. 
     * 
     **/
    private GCcontext mGCcontext;

    // Use this for initialization
    void Start()
    {
        mGCcontext = GCcontext.getInstance;
    }

    public void OnClick(int id,Vector3 worldTouch)
    {
        Debug.Log("Joystick Onclick :" + worldTouch);

#if CONTROLLER
        int[] values = new int[3];
        values[0] = id; values[1] = (int)worldTouch.x; values[2] = (int)worldTouch.y;
        mGCcontext.getClientManager().sendEvent(GCconst.CODE_JOYSTICK, values);
#endif

    }
}

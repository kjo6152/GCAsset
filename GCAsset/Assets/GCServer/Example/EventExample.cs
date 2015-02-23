using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventExample : MonoBehaviour
{
    GCcontext mGCcontext;
    // Use this for initialization
    void Start()
    {
        mGCcontext = GCcontext.getInstance;
        mGCcontext.getEventManager().clearListener();
        mGCcontext.getEventManager().onAccelerationListener += new EventManager.AccelerationListener(mEventManager_onAccelerationListener);
        mGCcontext.getEventManager().onGyroListener += new EventManager.GyroListener(mEventManager_onGyroListener);
        mGCcontext.getEventManager().onButtonListener += new EventManager.ButtonListener(mEventManager_onButtonListener);
        mGCcontext.getEventManager().onDirectionKeyListener += new EventManager.DirectionKeyListener(mEventManager_onDirectionKeyListener);
        mGCcontext.getEventManager().onJoystickListener += new EventManager.JoystickListener(mEventManager_onJoystickListener);
    }

    void mEventManager_onJoystickListener(GameController gc, EventManager.JoystickEvent joystickEvent)
    {
        Debug.Log("onJoystickListener");
        Debug.Log("id : " + joystickEvent.id + " / x : " + joystickEvent.x + " / y : " + joystickEvent.y);
    }

    void mEventManager_onDirectionKeyListener(GameController gc, EventManager.DirectionKeyEvent directionKeyEvent)
    {
        Debug.Log("onDirectionKeyListener");
        Debug.Log("id : " + directionKeyEvent.id + " / key : " + directionKeyEvent.key);
    }

    void mEventManager_onButtonListener(GameController gc, EventManager.ButtonEvent buttonEvent)
    {
        Debug.Log("onButtonListener");
        Debug.Log("id : " + buttonEvent.id);
    }

    void mEventManager_onGyroListener(GameController gc, EventManager.GyroEvent gyro)
    {
        //Debug.Log("onGyroListener");
        //Debug.Log("x : " + gyro.x + " / y : " + gyro.y + " / z : " + gyro.z);
    }

    void mEventManager_onAccelerationListener(GameController gc, EventManager.AccelerationEvent acceleration)
    {
        //Debug.Log("onAccelerationListener");
        //Debug.Log("x : " + acceleration.x + " / y : " + acceleration.y + " / z : " + acceleration.z);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
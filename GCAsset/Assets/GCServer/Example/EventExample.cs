﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * @breif GCAsset을 이용하여 이벤트를 받아 처리하는 예제
 * @details GCAsset에서 제공하는 5가지 이벤트에 대한 예제이다. 
 * 이벤트는 총 5가지로 자이로와 가속도 센서, 버튼, 방향키, 조이스틱키로 구성되어 있다.
 * GCcontext가 가지고 있는 EventManager를 통해서 리스너를 등록해주면 이벤트가 왔을 때 리스너를 호출해준다.
 * @author jiwon
 */
public class EventExample : MonoBehaviour
{
    GCcontext mGCcontext;
    // Use this for initialization

    /**
     * @breif 초기화 및 리스너 등록
     * @details 5가지 이벤트에 대해서 기존에 등록되었던 리스너를 초기화하고 새 리스너를 등록한다.
     * @see GCcontext
     * @see EventManager
     */
    void Start()
    {
        mGCcontext = GCcontext.getInstance;
        mGCcontext.getEventManager().clearListener();
        mGCcontext.getEventManager().onAccelerationListener += new EventManager.AccelerationListener(mEventManager_onAccelerationListener);
        mGCcontext.getEventManager().onGyroListener += new EventManager.GyroListener(mEventManager_onGyroListener);
        mGCcontext.getEventManager().onButtonListener += new EventManager.ButtonListener(mEventManager_onButtonListener);
        mGCcontext.getEventManager().onJoystickListener += new EventManager.JoystickListener(mEventManager_onJoystickListener);

        mGCcontext.getEventManager().mGyroFilter = new AttitudeFilter();
        mGCcontext.getEventManager().mAccelerationFilter = new MaxAccelerationFilter();
    }

    /**
     * @breif 조이스틱 이벤트
     * @see EventManager.JoystickEvent
     * @see GameController
     */
    void mEventManager_onJoystickListener(GameController gc, EventManager.JoystickEvent joystickEvent)
    {
        Debug.Log("onJoystickListener");
        Debug.Log("id : " + joystickEvent.id + " / x : " + joystickEvent.x + " / y : " + joystickEvent.y);
    }

    /**
     * @breif 버튼 이벤트
     * @see EventManager.ButtonEvent
     * @see GameController
     */
    void mEventManager_onButtonListener(GameController gc, EventManager.ButtonEvent buttonEvent)
    {
        Debug.Log("onButtonListener");
        Debug.Log("id :: " + buttonEvent.id);
    }

    /**
     * @breif 자이로 센서 이벤트
     * @see EventManager.GyroEvent
     * @see GameController
     */
    void mEventManager_onGyroListener(GameController gc, EventManager.GyroEvent gyro)
    {
        //Debug.Log("onGyroListener");
        //Debug.Log("x : " + gyro.x + " / y : " + gyro.y + " / z : " + gyro.z);
        /** 필터를 적용하지 않았을 경우 */
        //transform.position += new Vector3(gyro.y * 5, -gyro.x * 5, 0);
        /** AttitudeFilter를 적용했을 경우 */
        transform.position += new Vector3(-gyro.y * 5, gyro.x * 5, 0);
    }

    /**
     * @breif 가속도 센서 이벤트
     * @see EventManager.AccelerationEvent
     * @see GameController
     */
    void mEventManager_onAccelerationListener(GameController gc, EventManager.AccelerationEvent acceleration)
    {
        //Debug.Log("onAccelerationListener");
        Debug.Log("x : " + acceleration.x + " / y : " + acceleration.y + " / z : " + acceleration.z);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
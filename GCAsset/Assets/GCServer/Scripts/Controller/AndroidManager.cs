﻿#if UNITY_ANDROID || UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidManager : MonoBehaviour
{
    static AndroidManager _instance;
    private AndroidJavaObject curActivity;
    

    public static AndroidManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameObject("AndroidPluginManager").AddComponent<AndroidManager>();
        }

        return _instance;
    }

    void Awake()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        curActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void CallVibrate(int seconds)
    {
        //자바 호출
        curActivity.Call("CallVibrate", seconds);
    }
 

}
#endif
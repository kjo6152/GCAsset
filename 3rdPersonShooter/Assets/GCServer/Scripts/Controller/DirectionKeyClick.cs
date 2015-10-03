//#define CONTROLLER

using UnityEngine;
using System.Collections;

public class DirectionKeyClick : MonoBehaviour
{


    bool result;
    public Sprite downPressSprite;
    public int vibrate_int = 15;
    public AudioClip downPressSound;
    public int id;
    private GCcontext mGCcontext;

    // Use this for initialization
    void Start()
    {
      
        mGCcontext = GCcontext.getInstance;
    }

    public void DownClick(Transform[] DirectionKeyList, int[] minIdx,  Sprite[] pressDownSprite)
    {
        Debug.Log("Button Pressed :" + id);
        this.transform.parent.GetComponent<SpriteRenderer>().sprite = pressDownSprite[minIdx[0]];

#if UNITY_ANDROID
        //진동 - 안드로이드에서만 적용
        AndroidManager.GetInstance().CallVibrate(vibrate_int);
#endif
        //사운드
        mGCcontext.getAudioSource().clip = downPressSound;
        mGCcontext.getAudioSource().Play();

#if CONTROLLER
        int[] values = new int[2];
        values[0] = id; values[1] = GCconst.VALUE_PRESSED;
        mGCcontext.getClientManager().sendEvent(GCconst.CODE_DIRECTION_KEY, values);
#endif
 
    }

 
}


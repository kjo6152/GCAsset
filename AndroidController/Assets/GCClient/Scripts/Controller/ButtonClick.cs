#define CONTROLLER

using UnityEngine;
using System.Collections;

public class ButtonClick : MonoBehaviour {

    /**
     * 
     * Android - Unity vibrate연동. 
     * Unity Vibrate의 한정적 기능 때문에 Android 의 Vibrate 를 이용하는 플러그인 개발.
     * 선택된 버튼 Object 에 대한 Click Event 발생.
     * 서버와의 통신 기능
     * 
     **/
   

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

    public void DownClick(Transform[] buttonList, int[] minIdx, Sprite[] pressDownSprite)
    {
        Debug.Log("Button Pressed :" + id);
        buttonList[minIdx[0]].GetComponent<SpriteRenderer>().sprite = pressDownSprite[minIdx[0]];    
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
        mGCcontext.getClientManager().sendEvent(GCconst.CODE_BUTTON, values);
#endif
    }


    public void UpState(Transform[] buttonList, int[] minIdx, int touchCount, int buttonListLength, Sprite[] upPressSprite, Sprite[] pressDownSprite)
    {
        int i= 0;
        
        for (i = 0; i < buttonListLength; i++)
        {
            buttonList[i].GetComponent<SpriteRenderer>().sprite = upPressSprite[i];
        }
        for (i = 0; i <=touchCount; i++)
        {
            buttonList[minIdx[i]].GetComponent<SpriteRenderer>().sprite = pressDownSprite[minIdx[i]];   
        }

    }

}

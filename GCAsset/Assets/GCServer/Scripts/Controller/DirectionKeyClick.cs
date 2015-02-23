//#define CONTROLLER

using UnityEngine;
using System.Collections;

public class DirectionKeyClick : MonoBehaviour
{


    bool result;
    public Sprite downPressSprite;
    public int vibrate_int = 15;
    public string downPressSound;
    public int id = 0;
    private GCcontext mGCcontext;

    // Use this for initialization
    void Start()
    {
        mGCcontext = GCcontext.getInstance;
    }

    public void DownClick(GameObject[] DirectionKeyList, int[] minIdx, int touchCount, Sprite[] pressDownSprite)
    {

        if (touchCount == 0)
        {

            Debug.Log("touch Count1 :" + touchCount + "click :" + DirectionKeyList[minIdx[0]].name + ", sprite : " + pressDownSprite[minIdx[0]]);
            this.transform.parent.GetComponent<SpriteRenderer>().sprite = pressDownSprite[minIdx[0]];

        }
        else
        {
            for (int i = 1; i <= touchCount; i++)
            {
                // 소리하고 bibrate 설정해야해 ..
                Debug.Log("touch Count2 :" + touchCount + "click" + i + " : " + DirectionKeyList[minIdx[i]] + "sprite : " + pressDownSprite[minIdx[i]]);
                this.transform.parent.GetComponent<SpriteRenderer>().sprite = pressDownSprite[minIdx[i]];

            }
        }
#if UNITY_ANDROID
        AndroidManager.GetInstance().CallVibrate(vibrate_int);
#endif
        //사운드
        mGCcontext.getEventManager().playSound(downPressSound);

#if CONTROLLER
        int[] values = new int[2];
        values[0] = id; values[1] = GCconst.VALUE_PRESSED;
        mGCcontext.getClientManager().sendEvent(GCconst.CODE_DIRECTION_KEY, values);
#endif
    }

 
}


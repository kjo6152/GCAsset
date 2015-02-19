using UnityEngine;
using System.Collections;

public class DirectionKeyClick : MonoBehaviour
{


    bool result;
    public Sprite downPressSprite;
    public int vibrate_int = 15;


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
 
    }

 
}


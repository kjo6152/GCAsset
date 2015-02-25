using UnityEngine;
using System.Collections;

public class ButtonTouchCorllection : MonoBehaviour {


    public float buttonDelay;


    public int ButtonSelectTouchObject(Vector3 touch, int buttonListLength , GameObject[] buttonList)
    {
      
        int minIdx = 0;
        float minDistance;
        float[] Button_TouchDistance = new float[buttonListLength];


        for (int i = 0; i < buttonListLength; i++)
        {
            Button_TouchDistance[i] = Vector3.Distance(touch, buttonList[i].transform.position);
        }

        // 가장 가까운 버튼 선택 후 리턴.
        minDistance = Button_TouchDistance[0];

        for (int i = 1; i < buttonListLength; i++)
        {
            if (minDistance > Button_TouchDistance[i])
            {
                minDistance = Button_TouchDistance[i];
                minIdx = i;
            }
        }
        
        return minIdx;
    }

    

}

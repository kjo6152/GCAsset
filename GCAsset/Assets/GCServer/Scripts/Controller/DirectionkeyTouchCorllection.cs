using UnityEngine;
using System.Collections;

public class DirectionkeyTouchCorllection : MonoBehaviour {

    private float[] DirectionKey_TouchDistance;
    public float directionKeyDelay;
    
    public int DirectionSelectTouchObject(Vector3 touch, int directionKeyLength, Transform[] DirectionKeyList)
    {
        int minIdx = 0;
        float minDistance;
        DirectionKey_TouchDistance = new float[directionKeyLength];


        for (int i = 0; i < directionKeyLength; i++)
        {
            DirectionKey_TouchDistance[i] = Vector3.Distance(touch, DirectionKeyList[i].transform.position);
        }

        // 가장 가까운 버튼 선택 후 리턴.
        minDistance = DirectionKey_TouchDistance[0];

        for (int i = 1; i < directionKeyLength; i++)
        {
            if (minDistance > DirectionKey_TouchDistance[i])
            {
                minDistance = DirectionKey_TouchDistance[i];
                minIdx = i;
            }
        }

        return minIdx;
    }


}

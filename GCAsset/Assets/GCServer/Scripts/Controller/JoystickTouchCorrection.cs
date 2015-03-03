using UnityEngine;
using System.Collections;

public class JoystickTouchCorrection : MonoBehaviour
{

    public int JoystickSelectTouchObject(Vector3 touch, int joystickLength , Transform[] joystickList)
    {
      
        int minIdx = 0;
        float minDistance;
        float[] Joystick_TouchDistance = new float[joystickLength];


        for (int i = 0; i < joystickLength; i++)
        {
            Joystick_TouchDistance[i] = Vector3.Distance(touch, joystickList[i].transform.position);
        }

        // 가장 가까운 버튼 선택 후 리턴.
        minDistance = Joystick_TouchDistance[0];

        for (int i = 1; i < joystickLength; i++)
        {
            if (minDistance > Joystick_TouchDistance[i])
            {
                minDistance = Joystick_TouchDistance[i];
                minIdx = i;
            }
        }
        
        return minIdx;
    }
	
}

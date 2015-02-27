using UnityEngine;
using System.Collections;

/**
 * @breif 자이로 센서를 통해 중력을 얻는 필터
 * @details 컨트롤러로부터 전송된 가속도 값을 필터링하여 가장 큰 값을 구하는 필터
 * 필터를 등록하면 사용자에게 전달되는 값은 필터링된 값이 전달된다.
 * @see EventManager.mAccerationFilter
 * @author jiwon
 */
public class MaxAccelerationFilter : IEventFilter
{
    float max = 0;

    /**
     * @breif 가속도 값을 모두 더해 가장 큰 값을 구하는 필터
     * @details Gyroscope에서 전달되는 값들 중 userAcceleration값을 센서에서 받아 합을 구하고 그동안 값 중 가장 큰 값을 구한다.
     * @see IEventFilter.filter
     */
    public void filter(ref float[] data)
    {
        float value = data[0] + data[1] + data[2];
        if (value > max) max = value;
        data[0] = max;
        data[1] = max;
        data[2] = max;
    }

    /**
     * @breif 최대값을 초기화한다.
     * @see MaxAccelerationFilter.filter
     */
    public void clearMax(){
        max = 0;
    }
}

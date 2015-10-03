using UnityEngine;
using System.Collections;

/**
 * @breif 자이로 센서를 통해 중력을 얻는 필터
 * @details 컨트롤러로부터 전송된 자이로 센서의 값을 필터링하여 중력값을 얻는 필터
 * 필터를 등록하면 사용자에게 전달되는 값은 필터링된 값이 전달된다.
 * @see EventManager.mGyroFilter
 * @author jiwon
 */
public class GravityFilter : IEventFilter {

    /**
     * @breif gravity 값을 필터링하는 매소드
     * @details Gyroscope에서 전달되는 값들 중 gravity 값을 센서에서 받을 수 있도록 설정한다.
     * @see IEventFilter.filter
     */
    public void filter(ref float[] data)
    {
        data[0] = data[3];
        data[1] = data[4];
        data[2] = data[5];
        data[3] = 0.0f;
    }
}

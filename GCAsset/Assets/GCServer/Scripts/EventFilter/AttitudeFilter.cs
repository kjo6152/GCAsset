using UnityEngine;
using System.Collections;

/**
 * @breif 자이로 센서를 통해 Attitude를 얻는 필터
 * @details 컨트롤러로부터 전송된 자이로 센서의 값을 필터링하여 Attitude를 얻는 필터
 * 필터를 등록하면 사용자에게 전달되는 값은 필터링된 값이 전달된다.
 * @see EventManager.mGyroFilter
 * @author jiwon
 */
public class AttitudeFilter : IEventFilter{
    public void filter(ref float[] data)
    {
        data[0] = data[6];
        data[1] = data[7];
        data[2] = data[8];
        data[3] = data[9];
    }
}

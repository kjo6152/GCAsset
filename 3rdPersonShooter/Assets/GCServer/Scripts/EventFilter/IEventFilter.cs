using UnityEngine;
using System.Collections;

/**
 * @breif 필터 인터페이스
 * @details 여러 필터가 구현해야할 인터페이스
 * @see EventManager.mGyroFilter, EventManager.mAccelerationFilter
 * @author jiwon
 */
public interface IEventFilter {

    /**
     * @brief 데이터가 필터링되는 매소드
     * @details 
     *  Unity 자이로 센서와 가속도 센서의 값을 받아서 처리한 후 다시 설정한다.<br>
     * - 자이로 센서<br>
     *  Gyroscope의 rotationRate, gravity, attitude를 활용한 값이며 총 10개의 float 값으로 0~9까지 각각 아래의 값을 갖는다.<br>
     * 받고 싶은 데이터는 data[0-3] 각각의 값에 설정해주면 이벤트 x,y,z,w값에 설정된다.<br>
     * 1) 0 - 2 : ratationRate<br>
     * data[0] : Gyroscope.rotationRate.x<br>
     * data[1] : Gyroscope.rotationRate.y<br>
     * data[2] : Gyroscope.rotationRate.z<br>
     * 2) 3 - 5 : ratationRate<br>
     * data[3] : Gyroscope.gravity.x<br>
     * data[4] : Gyroscope.gravity.y<br>
     * data[5] : Gyroscope.gravity.z<br>
     * 3) 6 - 9 : ratationRate<br>
     * data[6] : Gyroscope.attitude.x<br>
     * data[7] : Gyroscope.attitude.y<br>
     * data[8] : Gyroscope.attitude.z<br>
     * data[9] : Gyroscope.attitude.w<br>
     * - 가속도 센서<br>
     * Gyroscope의 userAcceleration를 활용한 값이며 총 3개의 float 값으로 0~2까지 각각 아래의 값을 갖는다.<br>
     * 받고 싶은 데이터는 data[0-2] 각각의 값에 설정해주면 이벤트 x,y,z값에 설정된다.<br>
     * data[0] : Gyroscope.userAcceleration.x<br>
     * data[1] : Gyroscope.userAcceleration.y<br>
     * data[2] : Gyroscope.userAcceleration.z<br>
     * @param 센서값
     */
    void filter(ref float[] data);
}

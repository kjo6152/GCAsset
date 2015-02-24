using UnityEngine;
using System.Collections;

/**
 * @breif 칼만 필터를 구현한 필터
 * @details 컨트롤러로부터 전송된 자이로 센서의 값을 이용하여 칼만 필터로 필터링 된 값을 제공한다.
 * 필터를 등록하면 사용자에게 전달되는 값은 필터링된 값이 전달된다.
 * @see EventManager.mGyroFilter
 * @see http://ko.wikipedia.org/wiki/%EC%B9%BC%EB%A7%8C_%ED%95%84%ED%84%B0
 * @author jiwon
 */
public class KalmanFilter : IEventFilter
{
    private float Q = 0.00001f;
    private float R = 0.001f;
    private float X = 0, P = 1, K;

    //첫번째값을 입력받아 초기화 한다. 예전값들을 계산해서 현재값에 적용해야 하므로 반드시 하나이상의 값이 필요하므로~
    public KalmanFilter(float initValue)
    {
        X = initValue;
    }

    //예전값들을 공식으로 계산한다
    private void measurementUpdate()
    {
        K = (P + Q) / (P + Q + R);
        P = R * (P + Q) / (R + P + Q);
    }

    //현재값을 받아 계산된 공식을 적용하고 반환한다
    private float update(float measurement)
    {
        measurementUpdate();
        X = X + (measurement - X) * K;

        return X;
    }

    public void filter(ref float[] data)
    {
        data[0] = update(data[0]);
        data[1] = update(data[1]);
    }
}
﻿using UnityEngine;
using System.Collections;

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
using UnityEngine;
using System.Collections;

public class AttitudeFilter : IEventFilter{
    public void filter(ref float[] data)
    {
        data[0] = data[6];
        data[1] = data[7];
        data[2] = data[8];
        data[3] = data[9];
    }
}

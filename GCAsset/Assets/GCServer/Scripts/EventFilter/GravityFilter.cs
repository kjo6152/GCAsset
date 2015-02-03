using UnityEngine;
using System.Collections;

public class GravityFilter : IEventFilter {
    public void filter(ref float[] data)
    {
        data[0] = data[3];
        data[1] = data[4];
        data[2] = data[5];
        data[3] = 0.0f;
    }
}

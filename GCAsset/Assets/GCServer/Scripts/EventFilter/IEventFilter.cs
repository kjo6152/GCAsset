using UnityEngine;
using System.Collections;

public interface IEventFilter {
    void filter(ref float[] data);
}

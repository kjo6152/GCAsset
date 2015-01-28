using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	void Update () 
    {
        transform.Translate(0f, 0f, 0.05f);
	}
}

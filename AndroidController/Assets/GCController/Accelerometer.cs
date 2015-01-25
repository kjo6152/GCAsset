using UnityEngine;
using System.Collections;

public class Accelerometer : MonoBehaviour
{

    private bool accelBool;
    private Vector3 accel;
    // Use this for initialization
    void Start()
    {
        accelBool = SystemInfo.supportsAccelerometer;

    }

    void Update()
    {
        accel = Input.acceleration;

    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, Screen.height / 2 - 50, 100, 100), accelBool.ToString());

        if (accelBool)
        {
            GUI.Label(new Rect(10, Screen.height / 2 - 100, 500, 100), "accel supported");
            GUI.Label(new Rect(10, Screen.height / 2, 500, 100), "x :" + accel.x + " / y : " + accel.y + " / z : " + accel.z);
        }

    }
}
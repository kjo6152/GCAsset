using UnityEngine;
using System.Collections;

public class GyroSensor : MonoBehaviour {

    private bool gyroBool;
    private Gyroscope gyro;
	// Use this for initialization
	void Start () {
        gyroBool = SystemInfo.supportsGyroscope;
        if (gyroBool)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyro.updateInterval = 1.0f;
        }
       
	}

    void Update()
    {
       
        gyro = Input.gyro;

    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, Screen.height / 2 - 50, 100, 100), gyroBool.ToString());

        if (gyroBool)
        {
            GUI.Label(new Rect(10, Screen.height / 2 - 100, 500, 100), "gyro supported");
            GUI.Label(new Rect(10, Screen.height / 2, 500, 100), "rotation rate:" + gyro.rotationRate.ToString());
            GUI.Label(new Rect(10, Screen.height / 2 + 50, 500, 100), "gravity:      " + gyro.gravity.ToString());
            GUI.Label(new Rect(10, Screen.height / 2 + 100, 500, 100), "attitude:     " +gyro.attitude.ToString());
            //GUI.Label(new Rect(10, Screen.height / 2 + 150, 500, 100), "type:         " +gyro.GetType().ToString());
            GUI.Label(new Rect(10, Screen.height / 2 + 150, 500, 100), "x : " +gyro.userAcceleration.x);
            GUI.Label(new Rect(10, Screen.height / 2 + 200, 500, 100), "y : " + gyro.userAcceleration.y);
            GUI.Label(new Rect(10, Screen.height / 2 + 250, 500, 100), "x : " + gyro.userAcceleration.z);
        }
        
    }
}

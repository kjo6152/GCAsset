using UnityEngine;
using System.Collections;

public class gyro : MonoBehaviour {

    private Gyroscope gyo1;
    private bool gyoBool;
    private bool check;
    //private Quaternion rotFix;

    // Use this for initialization
    void Start()
    {
        gyoBool = SystemInfo.supportsGyroscope;

        if (gyoBool)
        {
            gyo1 = Input.gyro;
            gyo1.enabled = true;
        }
        
       // Debug.Log(gyoBool.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (check == true)
        {
           // Debug.Log("check");
        }
        gyo1 = Input.gyro;

    }

    void OnGUI()
    {
        //Debug.Log("log TEst");
        if (gyoBool != null)
        {
            GUI.Label(new Rect(10, Screen.height / 2 - 50, 100, 100), gyoBool.ToString());
            if (gyoBool == true)
            {
               // Debug.Log(gyo1.rotationRate.ToString());
                GUI.Label(new Rect(10, Screen.height / 2 - 100, 500, 100), "gyro supported");
                GUI.Label(new Rect(10, Screen.height / 2, 500, 100), "rotation rate:" + gyo1.rotationRate.ToString());
                GUI.Label(new Rect(10, Screen.height / 2 + 50, 500, 100), "gravity:      " + gyo1.gravity.ToString());
                GUI.Label(new Rect(10, Screen.height / 2 + 100, 500, 100), "attitude:     " + gyo1.attitude.ToString());
                GUI.Label(new Rect(10, Screen.height / 2 + 150, 500, 100), "type:         " + gyo1.GetType().ToString());
            }
            else
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 100, 100), "not supported");
        }
    }

}

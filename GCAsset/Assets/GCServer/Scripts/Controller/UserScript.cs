using UnityEngine;
using System.Collections;

public class UserScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("UserScript : Start");
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("UserScript : Update");
	}

    public void onClick()
    {
        Debug.Log("UserScript : onClick");
    }
}

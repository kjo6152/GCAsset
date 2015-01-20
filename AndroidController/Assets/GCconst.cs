using UnityEngine;
using System.Collections;

public class GCconst {
	/**
	 * Type에 관련된 상수들
	 */ 
	public const int TYPE_EVENT = 0x01;
	public const int TYPE_SENSOR = 0x02;
	public const int TYPE_CONTROLLER = 0x03;
	public const int TYPE_RESOURCE = 0x04;
	public const int TYPE_ACK = 0x05;

	/**
	 * TYPE_SENSOR 관련 CODE
	 */
	public const int CODE_ACCELERATION = 0x01;
	public const int CODE_GYRO = 0x02;

	/**
	 * TYPE_EVENT 관련 CODE
	 */
	public const int CODE_VIBRATION = 0x03;
	public const int CODE_VIEW = 0x04;
	public const int CODE_SOUND = 0x05;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

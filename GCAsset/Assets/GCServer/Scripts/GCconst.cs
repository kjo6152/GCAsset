using UnityEngine;
using System.Collections;

public class GCconst
{
    /**
     * Type에 관련된 상수들
     */
    public const ushort TYPE_EVENT = 0x01;
    public const ushort TYPE_SENSOR = 0x02;
    public const ushort TYPE_CONTROLLER = 0x03;
    public const ushort TYPE_RESOURCE = 0x04;
    public const ushort TYPE_SYSTEM = 0x05;
    public const ushort TYPE_ACK = 0x06;

    /**
     * TYPE_SENSOR 관련 CODE
     */
    public const ushort CODE_ACCELERATION = 0x01;
    public const ushort CODE_GYRO = 0x02;

    /**
     * TYPE_EVENT 관련 CODE
     */
    public const ushort CODE_VIBRATION = 0x11;
    public const ushort CODE_VIEW = 0x12;
    public const ushort CODE_SOUND = 0x13;

    /**
     * TYPE_SYSTEM 관련 CODE
     */
    public const ushort CODE_CONNECTED = 0x21;
    public const ushort CODE_DISCONNECTED = 0x22;
    public const ushort CODE_COMPLETE = 0x23;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

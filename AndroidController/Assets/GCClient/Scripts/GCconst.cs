using UnityEngine;
using System.Collections;

public class GCconst
{
    /**
     * Type에 관련된 상수들
     */
    public const ushort TYPE_EVENT = 0x01;
    public const ushort TYPE_SENSOR = 0x02;
    public const ushort TYPE_RESOURCE = 0x03;
    public const ushort TYPE_SYSTEM = 0x04;
    public const ushort TYPE_ACK = 0x05;

    /**
     * TYPE_EVENT 관련 CODE
     */
    public const ushort CODE_VIEW = 0x11;
    public const ushort CODE_VIBRATION = 0x12;
    public const ushort CODE_SOUND = 0x13;
    public const ushort CODE_BUTTON = 0x14;
    public const ushort CODE_JOYSTICK = 0x15;
    public const ushort CODE_DIRECTION_KEY = 0x16;

    /**
     * TYPE_SENSOR 관련 CODE
     */
    public const ushort CODE_ACCELERATION = 0x21;
    public const ushort CODE_GYRO = 0x22;

    /**
     * TYPE_SYSTEM 관련 CODE
     */
    public const ushort CODE_CONNECTED = 0x31;
    public const ushort CODE_DISCONNECTED = 0x32;
    public const ushort CODE_COMPLETE = 0x33;

    /**
     * CODE_BUTTON 관련 VALUE
     */
    public const int VALUE_PRESSED = 0x41;
    public const int VALUE_UNPRESSED = 0x42;

    /**
     * CODE_DIRECTION_KEY 관련 VALUE
     */
    public const int VALUE_UP = 0x51;
    public const int VALUE_RIGHT = 0x52;
    public const int VALUE_DOWN = 0x53;
    public const int VALUE_LEFT = 0x54;

    /**
     * Event Size 관련 상수
     */
    public const int SIZE_SENSOR = 10 * sizeof(float);
    public const int SIZE_BUTTON = 2 * sizeof(int);
    public const int SIZE_JOYSTICK = 3 * sizeof(int);
    public const int SIZE_DIRECTION_KEY = 2 * sizeof(int);
}

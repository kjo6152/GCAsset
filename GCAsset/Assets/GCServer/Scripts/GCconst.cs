using UnityEngine;
using System.Collections;

/**
 * @breif GCAsset 내부에서 사용되는 상수값들을 갖는 클래스
 * @details GCAsset 내부 통신을 위한 상수나 이벤트에 대한 상수 값을 갖는다.
 * @author jiwon
 */
public class GCconst
{
    /** 
     * @breif Type에 관련된 상수들 
     */
    public const ushort TYPE_EVENT = 0x01;
    public const ushort TYPE_SENSOR = 0x02;
    public const ushort TYPE_RESOURCE = 0x03;
    public const ushort TYPE_SYSTEM = 0x04;
    public const ushort TYPE_ACK = 0x05;

    /**
     * @breif TYPE_EVENT 관련 CODE
     */
    public const ushort CODE_VIEW = 0x11;
    public const ushort CODE_VIBRATION = 0x12;
    public const ushort CODE_SOUND = 0x13;
    public const ushort CODE_BUTTON = 0x14;
    public const ushort CODE_JOYSTICK = 0x15;
    public const ushort CODE_DIRECTION_KEY = 0x16;

    /**
     * @breif TYPE_SENSOR 관련 CODE
     */
    public const ushort CODE_ACCELERATION = 0x21;
    public const ushort CODE_GYRO = 0x22;

    /**
     * @breif TYPE_SYSTEM 관련 CODE
     */
    public const ushort CODE_CONNECTED = 0x31;
    public const ushort CODE_DISCONNECTED = 0x32;
    public const ushort CODE_COMPLETE = 0x33;

    /**
     * CODE_BUTTON 관련 VALUE
     */

    /** @breif 버튼 이벤트에 대한 상수 VALUE @details 버튼이 눌림 */
    public const int VALUE_PRESSED = 0x41;
    /** @breif 버튼 이벤트에 대한 상수 VALUE @details 버튼이 떼어짐 */
    public const int VALUE_UNPRESSED = 0x42;

    /**
     * CODE_DIRECTION_KEY 관련 VALUE
     */

    /** @breif 방향키 이벤트에 대한 상수 VALUE @details 위쪽 방향키가 눌림 */
    public const int VALUE_UP = 0x51;
    /** @breif 방향키 이벤트에 대한 상수 VALUE @details 오른쪽 방향키가 눌림 */
    public const int VALUE_RIGHT = 0x52;
    /** @breif 방향키 이벤트에 대한 상수 VALUE @details 아래쪽 방향키가 눌림 */
    public const int VALUE_DOWN = 0x53;
    /** @breif 방향키 이벤트에 대한 상수 VALUE @details 왼쪽 방향키가 눌림 */
    public const int VALUE_LEFT = 0x54;

    /**
     * Event Size 관련 상수
     */
    public const int SIZE_SENSOR = 10 * sizeof(float);
    public const int SIZE_BUTTON = 2 * sizeof(int);
    public const int SIZE_JOYSTICK = 3 * sizeof(int);
    public const int SIZE_DIRECTION_KEY = 2 * sizeof(int);
}

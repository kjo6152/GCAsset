using UnityEngine;
using System.Collections;

// 3rd person game-like camera controller
// keeps camera behind the player and aimed at aiming point
public class ShooterGameCamera : MonoBehaviour {
    GCcontext mGCcontext;

	public Transform player;
	public Transform aimTarget;
	
	public float smoothingTime = 0.5f;
	public Vector3 pivotOffset = new Vector3(1.3f, 0.4f,  0.0f);
	public Vector3 camOffset   = new Vector3(0.0f, 0.7f, -2.4f);
	public Vector3 closeOffset = new Vector3(0.35f, 1.7f, 0.0f);
	
	public float horizontalAimingSpeed = 270f;
	public float verticalAimingSpeed = 270f;
	public float maxVerticalAngle = 80f;
	public float minVerticalAngle = -80f;
	
	public float mouseSensitivity = 0.1f;
	
	public Texture reticle;
    public WeaponStateController weapon;
    public PlatformCharacterController PCC;
	
	private float angleH = 0;
	private float angleV = 0;
	private Transform cam;
	private float maxCamDist = 1;
	private LayerMask mask;
	private Vector3 smoothPlayerPos;

    private float axisMouseX, axisMouseY, axisMouseZ;

    private float[] m_gravity_data = new float[3];
    private float[] m_accel_data = new float[3];
    const float alpha = (float)0.8;

	// Use this for initialization
	void Start () 
	{
		// Add player's own layer to mask
		mask = 1 << player.gameObject.layer;
		// Add Igbore Raycast layer to mask
		mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
		// Invert mask
		mask = ~mask;
		
		cam = transform;
		smoothPlayerPos = player.position;
		
		maxCamDist = 3;

        mGCcontext = GCcontext.getInstance;
        mGCcontext.getEventManager().clearListener();
        mGCcontext.getEventManager().onAccelerationListener += new EventManager.AccelerationListener(mEventManager_onAccelerationListener);
        mGCcontext.getEventManager().onGyroListener += new EventManager.GyroListener(mEventManager_onGyroListener);
        mGCcontext.getEventManager().onButtonListener += new EventManager.ButtonListener(mEventManager_onButtonListener);
        mGCcontext.getEventManager().onJoystickListener += new EventManager.JoystickListener(mEventManager_onJoystickListener);

        mGCcontext.getEventManager().mGyroFilter = new AttitudeFilter();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (Time.deltaTime == 0 || Time.timeScale == 0 || player == null) 
			return;

        //Debug.Log("Mouse X : " + Input.GetAxis("Mouse X") + ", " + "Mouse Y" + Input.GetAxis("Mouse Y"));
        angleH += Mathf.Clamp(axisMouseX, -1, 1) * horizontalAimingSpeed * Time.deltaTime;
        angleV += Mathf.Clamp(axisMouseY, -1, 1) * verticalAimingSpeed * Time.deltaTime;
		// limit vertical angle
		angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
		
		// Before changing camera, store the prev aiming distance.
		// If we're aiming at nothing (the sky), we'll keep this distance.
		float prevDist = (aimTarget.position - cam.position).magnitude;
		
		// Set aim rotation
		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		cam.rotation = aimRotation;
		
		// Find far and close position for the camera
		smoothPlayerPos = Vector3.Lerp(smoothPlayerPos, player.position, smoothingTime * Time.deltaTime);
		smoothPlayerPos.x = player.position.x;
		smoothPlayerPos.z = player.position.z;
		Vector3 farCamPoint = smoothPlayerPos + camYRotation * pivotOffset + aimRotation * camOffset;
		Vector3 closeCamPoint = player.position + camYRotation * closeOffset;
		float farDist = Vector3.Distance(farCamPoint, closeCamPoint);
		
		// Smoothly increase maxCamDist up to the distance of farDist
		maxCamDist = Mathf.Lerp(maxCamDist, farDist, 5 * Time.deltaTime);
		
		// Make sure camera doesn't intersect geometry
		// Move camera towards closeOffset if ray back towards camera position intersects something 
		RaycastHit hit;
		Vector3 closeToFarDir = (farCamPoint - closeCamPoint) / farDist;
		float padding = 0.3f;
		if (Physics.Raycast(closeCamPoint, closeToFarDir, out hit, maxCamDist + padding, mask)) {
			maxCamDist = hit.distance - padding;
		}
		cam.position = closeCamPoint + closeToFarDir * maxCamDist;
		
		// Do a raycast from the camera to find the distance to the point we're aiming at.
		float aimTargetDist;
		if (Physics.Raycast(cam.position, cam.forward, out hit, 100, mask)) {
			aimTargetDist = hit.distance + 0.05f;
		}
		else {
			// If we're aiming at nothing, keep prev dist but make it at least 5.
			aimTargetDist = Mathf.Max(5, prevDist);
		}
		
		// Set the aimTarget position according to the distance we found.
		// Make the movement slightly smooth.
		aimTarget.position = cam.position + cam.forward * aimTargetDist;
	}
	
	void OnGUI () {
		if (Time.time != 0 && Time.timeScale != 0)
			GUI.DrawTexture(new Rect(Screen.width/2-(reticle.width*0.5f), Screen.height/2-(reticle.height*0.5f), reticle.width, reticle.height), reticle);
	}

    Vector3 zeroAc = new Vector3();
    Vector3 curAc = new Vector3();
    float sensH = 10f;
    float sensV = 10f;
    float smooth = 0.5f;
    float GetAxisH = 0f;
    float GetAxisV = 0f;

    void mEventManager_onAccelerationListener(GameController gc, EventManager.AccelerationEvent acceleration)
    {
        if (acceleration.x > 1f || acceleration.y > 1f || acceleration.z > 1f)
        {
            weapon.ReloadGun();
        }
    }

    void mEventManager_onGyroListener(GameController gc, EventManager.GyroEvent gyro)
    {
        PCC.movePos.x = -gyro.x * 10f;
        PCC.movePos.y = -gyro.y * 10f;
    }

    void mEventManager_onButtonListener(GameController gc, EventManager.ButtonEvent buttonEvent)
    {
        weapon.Fire();
    }

    void mEventManager_onJoystickListener(GameController gc, EventManager.JoystickEvent joystickEvent)
    {
        axisMouseX = joystickEvent.x / 600f;
        axisMouseY = joystickEvent.y / 600f;
    }
}

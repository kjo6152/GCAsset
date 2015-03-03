#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

// Unity Editor 메뉴창에 GUI 툴 등록
[ExecuteInEditMode]
public class GUI_Tool : EditorWindow
{
    static public int id = 0;

    Texture button1_on_texture_circle = Resources.Load<Texture>("Texture/Button1_On_Circle");
    Texture button2_on_texture_circle = Resources.Load<Texture>("Texture/Button2_On_Circle");
    Texture button3_on_texture_circle = Resources.Load<Texture>("Texture/Button3_On_Circle");
    Texture button4_on_texture_circle = Resources.Load<Texture>("Texture/Button4_On_Circle");

    Texture button1_on_texture_square = Resources.Load<Texture>("Texture/Button1_On_Square");
    Texture button2_on_texture_square = Resources.Load<Texture>("Texture/Button2_On_Square");
    Texture button3_on_texture_square = Resources.Load<Texture>("Texture/Button3_On_Square");
    Texture button4_on_texture_square = Resources.Load<Texture>("Texture/Button4_On_Square");

    Texture joystick1_normal_texture = Resources.Load<Texture>("Texture/Joystick1_Normal");
    Texture joystick2_normal_texture = Resources.Load<Texture>("Texture/Joystick_range");

    Texture camera_texture = Resources.Load<Texture>("Texture/Camera");

    void Start()
    {
        //버튼 방향키 조이스틱 태그 강제생성.
    }
    void Update()
    {
        ButtonClick[] normal = GameObject.FindObjectsOfType<ButtonClick>();
        for (int i = 0; i < normal.Length; i++)
        {
            if (normal[i].id > id)
                id = normal[i].id;
        }

        DirectionKeyClick[] joystick1 = GameObject.FindObjectsOfType<DirectionKeyClick>();
        for (int i = 0; i < joystick1.Length; i++)
        {
            if (joystick1[i].id > id)
                id = joystick1[i].id;
        }

        //Joystick[] joystick2 = GameObject.FindObjectsOfType<Joystick>();
        //for (int i = 0; i < joystick2.Length; i++)
        //{
        //    if (joystick2[i].id > id)
        //        id = joystick2[i].id;
        //}
    }

    [MenuItem("GUI Tool/Open GUI Tool &1", false, 0)]
    static void OpenGUITool()
    {
        EditorWindow window = EditorWindow.GetWindow<GUI_Tool>(false);
        window.title = "GUI Tool";
        window.maxSize = new Vector2(150f, 600f);
        window.minSize = new Vector2(140f, 600f);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5f, 5f, 100f, 20f), "Circle Button");
        if (GUI.Button(new Rect(10f, 30f, 40f, 40f), button1_on_texture_circle)) { CreateNormalButton("Circle Button(X)", 1, "Circle"); }
        if (GUI.Button(new Rect(60f, 30f, 40f, 40f), button2_on_texture_circle)) { CreateNormalButton("Circle Button(Y)", 2, "Circle"); }
        if (GUI.Button(new Rect(10f, 80f, 40f, 40f), button3_on_texture_circle)) { CreateNormalButton("Circle Button(A)", 3, "Circle"); }
        if (GUI.Button(new Rect(60f, 80f, 40f, 40f), button4_on_texture_circle)) { CreateNormalButton("Circle Button(B)", 4, "Circle"); }

        GUI.Label(new Rect(5f, 140f, 100f, 20f), "Square Button");
        if (GUI.Button(new Rect(10f, 165, 40f, 40f), button1_on_texture_square)) { CreateNormalButton("Square Button(X)", 1, "Square"); }
        if (GUI.Button(new Rect(60f, 165f, 40f, 40f), button2_on_texture_square)) { CreateNormalButton("Square Button(Y)", 2, "Square"); }
        if (GUI.Button(new Rect(10f, 215f, 40f, 40f), button3_on_texture_square)) { CreateNormalButton("Square Button(A)", 3, "Square"); }
        if (GUI.Button(new Rect(60f, 215f, 40f, 40f), button4_on_texture_square)) { CreateNormalButton("Square Button(B)", 4, "Square"); }

        GUI.Label(new Rect(5f, 275f, 100f, 40f), "Joystick\n(Four Direction)");
        if (GUI.Button(new Rect(10f, 310f, 40f, 40f), joystick1_normal_texture)) { CreateJoystickType1(); }

        GUI.Label(new Rect(5f, 380f, 100f, 40f), "Joystick\n(Free Direction)");
        if (GUI.Button(new Rect(10f, 415f, 40f, 40f), joystick2_normal_texture)) { CreateJoystickType2(); }

        GUI.Label(new Rect(5f, 485f, 100f, 40f), "Camera");
        if (GUI.Button(new Rect(10f, 510f, 40f, 40f), camera_texture)) { CreateCamera(); }
    }

    static void CreateNormalButton(string name, int type, string shape)
    {
        if (FindObjectOfType<Camera>() == null)
        {
            EditorUtility.DisplayDialog("Can not create object.", "Can not find camera object.\nCreate camera object first.", "OK");
            return;
        }

        // 부모 오브젝트
        ButtonTouchCorllection btc = FindObjectOfType<ButtonTouchCorllection>();
        GameObject parentObj = null;
        if (!btc)
        {
            parentObj = new GameObject();
            parentObj.transform.position = new Vector3(0f, 0f, 0f);
            parentObj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            parentObj.transform.localScale = new Vector3(1f, 1f, 1f);
            parentObj.name = "Button";
            parentObj.transform.parent = FindObjectOfType<Camera>().transform;

            parentObj.AddComponent<TouchCorrection>();
            parentObj.GetComponent<TouchCorrection>().MainCamera = FindObjectOfType<Camera>();

            parentObj.AddComponent<ButtonTouchCorllection>();
            parentObj.GetComponent<ButtonTouchCorllection>().buttonDelay = 0.2f;
        }
        else
        {
            parentObj = btc.gameObject;
        }

        // 자식 오브젝트
        GameObject btn = new GameObject();
        btn.transform.parent = parentObj.transform;
        btn.transform.position = new Vector3(0f, 0f, 0f);
        btn.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        btn.transform.localScale = new Vector3(180f, 180f, 1f);
        btn.name = name;
        btn.tag = "Button";

        btn.AddComponent<SpriteRenderer>();
        btn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Button" + type + "_On_" + shape);

        btn.AddComponent<CircleCollider2D>();
        btn.GetComponent<CircleCollider2D>().radius = 1;

        btn.AddComponent<ButtonClick>();
        btn.GetComponent<ButtonClick>().id = ++id;
        btn.GetComponent<ButtonClick>().downPressSprite = Resources.Load<Sprite>("Sprite/Button" + type + "_Off_" + shape);
    }

    static void CreateJoystickType1()
    {
        if (FindObjectOfType<Camera>() == null)
        {
            EditorUtility.DisplayDialog("Can not create object.", "Can not find camera object.\nCreate camera object first.", "OK");
            return;
        }

        string[] directArr = 
        {
            "Up",
            "Down",
            "Left",
            "Right"
        };

        Vector2[] positionArr = 
        {
            new Vector2(0f, 0.75f),
            new Vector2(0f, -0.75f),
            new Vector2(-0.75f, 0f),
            new Vector2(0.75f, 0f)
        };

        // 자식 오브젝트
        GameObject obj = new GameObject();
        obj.transform.parent = FindObjectOfType<Camera>().transform;
        obj.name = "DirectionKey";
        obj.tag = "GameController";
        obj.transform.position = new Vector3(0f, 0f, 1f);
        obj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        obj.transform.localScale = new Vector3(160f, 160f, 1f);

        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Joystick1_Normal");

        obj.AddComponent<TouchCorrection>();
        obj.GetComponent<TouchCorrection>().MainCamera = FindObjectOfType<Camera>();

        obj.AddComponent<DirectionkeyTouchCorllection>();
        obj.GetComponent<DirectionkeyTouchCorllection>().directionKeyDelay = 0.2f;

        for (int i = 0; i < directArr.Length; i++)
        {
            GameObject btn = new GameObject();
            btn.name = directArr[i];
            btn.tag = "DirectionKey";
            btn.transform.parent = obj.transform;
            btn.transform.localPosition = new Vector3(positionArr[i].x, positionArr[i].y, 0f);
            btn.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            btn.transform.localScale = new Vector3(1f, 1f, 1f);

            btn.AddComponent<CircleCollider2D>();
            btn.GetComponent<CircleCollider2D>().radius = 1;

            btn.AddComponent<DirectionKeyClick>();
            btn.GetComponent<DirectionKeyClick>().id = ++id;
            btn.GetComponent<DirectionKeyClick>().downPressSprite = Resources.Load<Sprite>("Sprite/Joystick1_" + directArr[i]);
            btn.GetComponent<DirectionKeyClick>().vibrate_int = 15;
        }
    }

    static void CreateJoystickType2()
    {
        if (FindObjectOfType<Camera>() == null)
        {
            EditorUtility.DisplayDialog("Can not create object.", "Can not find camera object.\nCreate camera object first.", "OK");
            return;
        }


        if (FindObjectOfType<JoystickTouchCorrection>() == null)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = FindObjectOfType<Camera>().transform;
            obj.name = "Joystick";
            obj.tag = "GameController";
            obj.transform.position = new Vector3(0f, 0f, 1f);
            obj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            obj.transform.localScale = new Vector3(120f, 120f, 1f);

            obj.AddComponent<TouchCorrection>();
            obj.GetComponent<TouchCorrection>().MainCamera = FindObjectOfType<Camera>();

            obj.AddComponent<JoystickTouchCorrection>();
        }

        // 첫 번째 자식 오브젝트
        GameObject btn1 = new GameObject();
        btn1.transform.parent = FindObjectOfType<JoystickTouchCorrection>().transform;
        btn1.name = "Basic Joystick";
        btn1.tag = "Joystick";
        btn1.transform.position = new Vector3(0f, 0f, 0f);
        btn1.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        btn1.transform.localScale = new Vector3(1f, 1f, 1f);

        btn1.AddComponent<SpriteRenderer>();
        btn1.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Joystick_range");

        btn1.AddComponent<CircleCollider2D>();
        btn1.GetComponent<CircleCollider2D>().radius = 0.75f;

        // 두 번째 자식 오브젝트
        GameObject btn2 = new GameObject();
        btn2.transform.parent = btn1.transform;
        btn2.name = "Wheel";
        btn2.transform.position = new Vector3(0f, 0f, 0f);
        btn2.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        btn2.transform.localScale = new Vector3(1f, 1f, 1f);

        btn2.AddComponent<SpriteRenderer>();
        btn2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Joystick_wheel");
        btn2.GetComponent<SpriteRenderer>().sortingOrder = 1;

        btn2.AddComponent<JoyStickOnClick>();
        btn2.GetComponent<JoyStickOnClick>().id = ++id;
    }

    static void CreateCamera()
    {
        if (FindObjectsOfType<Camera>().Length == 0)
        {
            EditorApplication.ExecuteMenuItem("GameObject/Camera");
            Camera camera = FindObjectOfType<Camera>();
            camera.transform.position = new Vector3(0f, 0f, 0f);
            camera.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            camera.transform.localScale = new Vector3(1f, 1f, 1f);
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 360f;
            camera.nearClipPlane = -10f;
            camera.farClipPlane = 10f;
            camera.gameObject.AddComponent<AndroidManager>();
        }
        else
        {
            if (FindObjectOfType<Camera>().gameObject.GetComponent<AndroidManager>() == null)
                FindObjectOfType<Camera>().gameObject.AddComponent<AndroidManager>();
        }
    }
}
#endif
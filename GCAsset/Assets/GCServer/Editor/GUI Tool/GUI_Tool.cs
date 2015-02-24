#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

// Unity Editor 메뉴창에 GUI 툴 등록
public class GUI_Tool : EditorWindow {
    Texture button1_on_texture_circle = Resources.Load<Texture>("Texture/Button1_On_Circle");
    Texture button2_on_texture_circle = Resources.Load<Texture>("Texture/Button2_On_Circle");
    Texture button3_on_texture_circle = Resources.Load<Texture>("Texture/Button3_On_Circle");
    Texture button4_on_texture_circle = Resources.Load<Texture>("Texture/Button4_On_Circle");

    Texture button1_on_texture_square = Resources.Load<Texture>("Texture/Button1_On_Square");
    Texture button2_on_texture_square = Resources.Load<Texture>("Texture/Button2_On_Square");
    Texture button3_on_texture_square = Resources.Load<Texture>("Texture/Button3_On_Square");
    Texture button4_on_texture_square = Resources.Load<Texture>("Texture/Button4_On_Square");

    Texture joystick1_normal_texture = Resources.Load<Texture>("Texture/Joystick1_Normal");

    [MenuItem("GUI Tool/Open GUI Tool &1", false, 0)]
    public static void OpenGUITool()
    {
        EditorWindow  window = EditorWindow.GetWindow<GUI_Tool>(false);
        window.title = "GUI Tool";
        window.maxSize = new Vector2(130f, 800f);
        window.minSize = new Vector2(120f, 800f);
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

        GUI.Label(new Rect(5f, 275f, 100f, 20f), "Joystick");
        if(GUI.Button(new Rect(10f, 300f, 40f, 40f), joystick1_normal_texture)) { CreateJoystick(); }
    }

    static void CreateNormalButton(string name, int type, string shape)
    {
        // 오브젝트 생성 및 컴포넌트 추가, 초기화
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>();
        obj.AddComponent<CircleCollider2D>();
        obj.GetComponent<CircleCollider2D>().radius = 1;
        obj.AddComponent<ButtonClick>();
        obj.name = name;
        obj.tag = "GameController";

        obj.transform.position = new Vector3(0f, 0f, 0f);
        obj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        obj.transform.localScale = new Vector3(180f, 180f, 1f);
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Button" + type + "_On_" + shape);
        obj.GetComponent<ButtonClick>().downPressSprite = Resources.Load<Sprite>("Sprite/Button" + type + "_Off_" + shape);

        ButtonTouchCorrection newObj = GameObject.FindObjectOfType<ButtonTouchCorrection>();
        obj.transform.parent = newObj.transform;
    }

    static void CreateJoystick()
    {
        string [] temp = 
        {
            "Normal",
            "Up",
            "Down",
            "Left",
            "Right"
        };

        for (int i = 0; i < 5; i++)
        {
            GameObject obj = new GameObject();
            obj.AddComponent<SpriteRenderer>();
            obj.AddComponent<CircleCollider2D>();
            obj.GetComponent<CircleCollider2D>().radius = 1;
            obj.AddComponent<ButtonClick>();
            obj.name = "Joystick";
            obj.tag = "GameController";

            obj.transform.position = new Vector3(0f, 0f, 0f);
            obj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            obj.transform.localScale = new Vector3(180f, 180f, 1f);
            obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprite/Joystick1_" + temp[i]);

            ButtonTouchCorrection newObj = GameObject.FindObjectOfType<ButtonTouchCorrection>();
            obj.transform.parent = newObj.transform;
        }
    }
}
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
    *  컨트롤러 터치 보정
    *  기능 :
    *   1) Raycast 를 이용하여 생성한 컨틀롤러의 버튼 Object의 터치 영역을 설정.
    *   2) 터치 영역에서 터치한 좌표와 각각의 버튼 Object 와의 거리를 통해 가장 가까운 Object 선택.
    *   3) 버튼 Object들에 대한 초기화 작업.
    */

public class TouchCorrection : MonoBehaviour
{

    /**
     * 버튼관련
     */
    private bool firstTouch;
    public Camera MainCamera;
    private Collider2D collider;
    private Transform[] buttonList = new Transform[10];
  
    private float[] Button_TouchDistance;

    private ButtonClick buttonClick;
  
  
    private Sprite[] pressUpSprite;
    private Sprite[] pressDownSprite;
    
    private int buttonListLength;

    //public float buttonDelay;
    float buttontimer;

    /**
      * 방향키 관련
      */

    private Transform[] DirectionKeyList = new Transform[4];
    

    private float[] DirectionKey_TouchDistance;

    private DirectionKeyClick directionKeyClick;

    private Sprite[] directionPressDownSprite;
    private Sprite NomalDirectionKey;


    private int directionKeyLength;
    private int directionKeyArrayLength;
 
    float directionKeytimer;

    private bool buttonClickState = false;
    private bool directionKeyClickState = false;
    private bool joystickClickState = false;

    private ButtonTouchCorllection buttonTouchCorllection;
    private DirectionkeyTouchCorllection directionKeyTouchCorllection;


    private float buttonDelay;
    private float directionKeyDelay;
    private int buttonTouchCount = 0;
    private int directionTouchCount = 0;
    private int joystickTouchCount = 0;

    /**
      * 조이스틱 관련
      */
    private Transform[] joystickList = new Transform[10];
    private int joystickLength;
    private Sprite[] joystickSprite;
    private GameObject[] joystickChild;


    private int[] buttonMinidx;
    private int[] directionMinidx;
    private int[] joystickMinidx;


    private int buttonChildLength;
    private int directionKeyChildLengh = 4;
    private int joystickChildLength;
    

    /**
     * GameObject 가 생성되면서 Button List들에 대한 초기화.
     * 버튼들의 PressUp 이미지 초기화.
     */
    void Start()
    {
        buttonMinidx = new int[5];
        directionMinidx = new int[5];
        joystickMinidx = new int[5];

        /**
          * 버튼관련
          */
        //생성한 버튼 컴포넌트의 리스트 저장.
        buttonChildLength = GameObject.FindObjectOfType<ButtonTouchCorllection>().transform.childCount;

        if (buttonChildLength > 0)
        {
            for (int i = 0; i < buttonChildLength; i++)
            {

                buttonList[i] = FindObjectOfType<ButtonTouchCorllection>().transform.GetChild(i);
                //Debug.Log(FindObjectOfType<ButtonTouchCorllection>().transform.GetChild(i));

            }

            buttonListLength = buttonList.Length;
            pressUpSprite = new Sprite[buttonListLength];
            pressDownSprite = new Sprite[buttonListLength];

            //pressup 이미지 초기화.
            for (int i = 0; i < buttonChildLength; i++)
            {
                pressUpSprite[i] = buttonList[i].GetComponent<SpriteRenderer>().sprite;
                pressDownSprite[i] = buttonList[i].GetComponent<ButtonClick>().downPressSprite;

            }
            buttontimer = 0;
            firstTouch = true;
        }

        /**
          * 방향키 관련
          */
        if (FindObjectOfType<DirectionkeyTouchCorllection>() != null)
        {
            for (int i = 0; i < directionKeyChildLengh; i++)
            {
                DirectionKeyList[i] = FindObjectOfType<DirectionkeyTouchCorllection>().transform.GetChild(i);
            }

            directionPressDownSprite = new Sprite[directionKeyChildLengh];

            for (int i = 0; i < directionKeyChildLengh; i++)
            {
                directionPressDownSprite[i] = DirectionKeyList[i].GetComponent<DirectionKeyClick>().downPressSprite;
            }
            if (this.name.Equals("DirectionKey"))
            {
                NomalDirectionKey = this.GetComponent<SpriteRenderer>().sprite;
            }
        }

        /**
       * 조이스틱관련.
       */

        joystickChildLength = GameObject.FindObjectOfType<JoystickTouchCorrection>().transform.childCount; ;
        Debug.Log(joystickChildLength);

        if (joystickChildLength > 0)
        {
            for (int i = 0; i < joystickChildLength; i++)
            {

                joystickList[i] = FindObjectOfType<JoystickTouchCorrection>().transform.GetChild(i);


            }



            joystickSprite = new Sprite[joystickChildLength];

        }
        
    }

    

	void Update () {

        
       
       
       // Debug.Log("버튼 딜레이  : " + buttonDelay);
        //Debug.Log(" 방향키 딜레이  : " + directionKeyDelay);

        RaycastHit2D hit;

        if (Input.touchCount == 0) // 터치하지 않을때..
        {
            
            ClickState.buttonClickState = false;
            ClickState.directionClickState = false;
            ClickState.joystickClickState = false;
            
            // 버튼에 대한 upSprite 초기화.
            for (int i = 0; i < buttonChildLength; i++)
            {
                //Debug.Log(i);
                buttonList[i].GetComponent<SpriteRenderer>().sprite = pressUpSprite[i];
            }

            // directionkey 에 대한 노말 버튼 초기화.
            if (this.name.Equals("DirectionKey"))
            {
                this.GetComponent<SpriteRenderer>().sprite = NomalDirectionKey;
            }

            for (int j = 0; j < joystickChildLength; j++)
            {
                
                joystickList[j].GetChild(0).position = joystickList[j].GetChild(0).parent.position;
            }
        }


        for (int i = 0; i < Input.touchCount; i++)
        {

            //Debug.Log("buttonhit");
            Vector3 worldTouch = MainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
            hit = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero, Mathf.Infinity);

            if (hit.collider)
            {
                
                if (hit.collider.transform.parent.name.Equals("Button") && this.name.Equals("Button"))
                {
                    
                    buttontimer += Time.deltaTime;
                    ClickState.buttonClickState = true;
                    buttonTouchCorllection = hit.collider.transform.parent.GetComponent<ButtonTouchCorllection>();
                    buttonDelay = buttonTouchCorllection.buttonDelay;

                    buttonMinidx[buttonTouchCount] = buttonTouchCorllection.ButtonSelectTouchObject(worldTouch, buttonChildLength, buttonList);
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {

                        
                        //buttonMinidx[buttonTouchCount] = ButtonSelectTouchObject(worldTouch);
                        //buttonMinidx[buttonTouchCount] = buttonTouchCorllection.ButtonSelectTouchObject(worldTouch, buttonListLength, buttonList);
                        buttonClick = buttonList[buttonMinidx[buttonTouchCount]].GetComponent<ButtonClick>();
                        buttonClick.DownClick(buttonList, buttonMinidx, pressDownSprite);
                        buttonClick.UpState(buttonList, buttonMinidx, 0, buttonChildLength, pressUpSprite, pressDownSprite);
                        //buttonTouchCount=1;
                        buttontimer = 0;
                    }
                    else if (buttontimer > buttonDelay && (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary))
                    {
                       
                        //Debug.Log(this.gameObject.GetComponent<ButtonTouchCorllection>().buttonDelay);
                        //buttonMinidx[buttonTouchCount] = ButtonSelectTouchObject(worldTouch);
                        //buttonMinidx[buttonTouchCount] = buttonTouchCorllection.ButtonSelectTouchObject(worldTouch, buttonListLength, buttonList);
                        buttonClick = buttonList[buttonMinidx[buttonTouchCount]].GetComponent<ButtonClick>();
                        buttonClick.DownClick(buttonList, buttonMinidx,  pressDownSprite);
                        buttonClick.UpState(buttonList, buttonMinidx, 0, buttonChildLength, pressUpSprite, pressDownSprite);
                        //buttonTouchCount++;
                        buttontimer = 0;

                    }

                    else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        for (int j = 0; j < buttonChildLength; j++)
                        {
                            buttonList[j].GetComponent<SpriteRenderer>().sprite = pressUpSprite[j];
                        }
                        buttonTouchCount = 0;
                    }
                }
                else if (hit.collider.transform.parent.name.Equals("DirectionKey") && this.name.Equals("DirectionKey"))
                {
                    ClickState.directionClickState = true;
                    
                    directionKeytimer += Time.deltaTime;
                    directionKeyTouchCorllection = hit.collider.transform.parent.GetComponent<DirectionkeyTouchCorllection>();
                    directionKeyDelay = directionKeyTouchCorllection.directionKeyDelay;
                    directionMinidx[directionTouchCount] = directionKeyTouchCorllection.DirectionSelectTouchObject(worldTouch, directionKeyChildLengh, DirectionKeyList);
                    
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        Debug.Log("no hit direction");
                        //directionMinidx[directionTouchCount] = directionKeyTouchCorllection.DirectionSelectTouchObject(worldTouch, directionKeyChildLengh, DirectionKeyList);
                        //directionMinidx[directionTouchCount] = DirectionSelectTouchObject(worldTouch);
                        directionKeyClick = DirectionKeyList[directionMinidx[directionTouchCount]].GetComponent<DirectionKeyClick>();
                        directionKeyClick.DownClick(DirectionKeyList, directionMinidx,  directionPressDownSprite);
                      
                        directionKeytimer = 0;
                    }
                    else if (directionKeytimer > directionKeyDelay && (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary))
                    {

                        //directionMinidx[directionTouchCount] = directionKeyTouchCorllection.DirectionSelectTouchObject(worldTouch, directionKeyChildLengh, DirectionKeyList);
                        //directionMinidx[directionTouchCount] = DirectionSelectTouchObject(worldTouch);
                        directionKeyClick = DirectionKeyList[directionMinidx[directionTouchCount]].GetComponent<DirectionKeyClick>();
                        directionKeyClick.DownClick(DirectionKeyList, directionMinidx, directionPressDownSprite);
                    
                        directionKeytimer = 0;
                    }
                    else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        if (this.name.Equals("DirectionKey"))
                        {
                            this.GetComponent<SpriteRenderer>().sprite = NomalDirectionKey;
                        }
                    }
                }

                    
                else if (hit.collider.transform.parent.name.Equals("Joystick") && this.name.Equals("Joystick"))
                {
                    
                    ClickState.joystickClickState = true;
                    joystickMinidx[joystickTouchCount] = hit.collider.transform.parent.GetComponent<JoystickTouchCorrection>().JoystickSelectTouchObject(worldTouch, joystickChildLength, joystickList);
                    
                    if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").transform.position = joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").transform.parent.position;
                        joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").GetComponent<JoyStickOnClick>().OnClick(new Vector3(0f, 0f, 0f));
                    }
                    else
                    {
                        Debug.Log(joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").name);    

                        joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").GetComponent<Transform>().position = new Vector3(worldTouch.x, worldTouch.y, 1f);

                        worldTouch.x -= joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").transform.parent.position.x;
                        worldTouch.y -= joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").transform.parent.position.y;

                        joystickList[joystickMinidx[joystickTouchCount]].transform.Find("Wheel").GetComponent<JoyStickOnClick>().OnClick(worldTouch);
                    }
                }
               
            }

            else
            {
               
               //Debug.Log("object : " + this + ", buttonclickstate : " + ClickState.buttonClickState + " , dirkeystate ; " + ClickState.directionClickState +",joystickstate : " + ClickState.joystickClickState);

               //Debug.Log(hit.collider.transform.parent.name);
               if (ClickState.buttonClickState)
               {
                   for (int j = 0; j < buttonChildLength; j++)
                   {
                       buttonList[j].GetComponent<SpriteRenderer>().sprite = pressUpSprite[j];
                   }

                   buttonClickState = false;
               }
               if (ClickState.directionClickState)
                {
                    
                    if (this.name.Equals("DirectionKey") )
                    {
                        this.GetComponent<SpriteRenderer>().sprite = NomalDirectionKey;
                    }
                    directionKeyClickState = false;

                }
               if (ClickState.joystickClickState)
               {
                   for (int j = 0; j < joystickChildLength; j++)
                   {
                       joystickList[j].transform.Find("Wheel").transform.position = joystickList[j].transform.Find("Wheel").transform.parent.position;
                   }
                   joystickClickState = false;
                }
               

            }


        }
   
	}

    

    
}

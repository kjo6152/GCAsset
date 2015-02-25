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
    private GameObject[] buttonList;
  
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

    private GameObject[] DirectionKeyList;

    private float[] DirectionKey_TouchDistance;

    private DirectionKeyClick directionKeyClick;

    private Sprite[] directionPressDownSprite;
    private Sprite NomalJoystick;


    private int directionKeyLength;
    //public float directionKeyDelay;
    float directionKeytimer;

    private bool buttonClickState = false;
    private bool directionKeyClickState = false;

    private ButtonTouchCorllection buttonTouchCorllection;
    private DirectionkeyTouchCorllection directionKeyTouchCorllection;


    private float buttonDelay;
    private float directionKeyDelay;
    /**
     * GameObject 가 생성되면서 Button List들에 대한 초기화.
     * 버튼들의 PressUp 이미지 초기화.
     */
    void Start()
    {
        /**
          * 버튼관련
          */
        //생성한 버튼 컴포넌트의 리스트 저장.
        buttonList = GameObject.FindGameObjectsWithTag("Button");

        buttonTouchCorllection = GetComponent<ButtonTouchCorllection>();
        buttonListLength = buttonList.Length;
        pressUpSprite = new Sprite[buttonListLength];
        pressDownSprite = new Sprite[buttonListLength];
     
      


        //pressup 이미지 초기화.
        for (int i = 0; i<buttonListLength; i++)
        {
            pressUpSprite[i] = buttonList[i].GetComponent<SpriteRenderer>().sprite;
            pressDownSprite[i] = buttonList[i].GetComponent<ButtonClick>().downPressSprite;
           
        }
        buttontimer = 0;
        firstTouch = true;

        /**
          * 방향키 관련
          */


        DirectionKeyList = GameObject.FindGameObjectsWithTag("DirectionKey");

        directionKeyLength = DirectionKeyList.Length;
        directionPressDownSprite = new Sprite[directionKeyLength];

        for (int i = 0; i < directionKeyLength; i++)
        {
            directionPressDownSprite[i] = DirectionKeyList[i].GetComponent<DirectionKeyClick>().downPressSprite;
        }
        if (this.name.Equals("DirectionKey"))
        {
            NomalJoystick = this.GetComponent<SpriteRenderer>().sprite;
        }

        

    }

    /**
     * 버튼
     * 터치 좌표와 GameObject 간에 가장 가까운 object를 선택하여
     * 그 Object 의 Index 를 반환.
     */
    int ButtonSelectTouchObject(Vector3 touch)
    {
        int minIdx = 0;
        float minDistance;
        Button_TouchDistance = new float[buttonListLength];


        for (int i = 0; i < buttonListLength; i++)
        {
            Button_TouchDistance[i] = Vector3.Distance(touch, buttonList[i].transform.position);
        }

        // 가장 가까운 버튼 선택 후 리턴.
        minDistance = Button_TouchDistance[0];

        for (int i = 1; i < buttonListLength; i++)
        {
            if (minDistance > Button_TouchDistance[i])
            {
                minDistance = Button_TouchDistance[i];
                minIdx = i;
            }
        }

        return minIdx;
    }


    /**
     * 방향키
     * 터치 좌표와 GameObject 간에 가장 가까운 object를 선택하여
     * 그 Object 의 Index 를 반환.
     */
    int DirectionSelectTouchObject(Vector3 touch)
    {
        int minIdx = 0;
        float minDistance;
        DirectionKey_TouchDistance = new float[directionKeyLength];


        for (int i = 0; i < directionKeyLength; i++)
        {
            DirectionKey_TouchDistance[i] = Vector3.Distance(touch, DirectionKeyList[i].transform.position);
        }

        // 가장 가까운 버튼 선택 후 리턴.
        minDistance = DirectionKey_TouchDistance[0];

        for (int i = 1; i < directionKeyLength; i++)
        {
            if (minDistance > DirectionKey_TouchDistance[i])
            {
                minDistance = DirectionKey_TouchDistance[i];
                minIdx = i;
            }
        }

        return minIdx;
    }


	void Update () {

        int buttonTouchCount = 0;
        int[] buttonMinidx = new int[5];

        int directionTouchCount = 0;
        int[] directionMinidx = new int[5];
      
       
       // Debug.Log("버튼 딜레이  : " + buttonDelay);
        //Debug.Log(" 방향키 딜레이  : " + directionKeyDelay);

        RaycastHit2D hit;

        if (Input.touchCount == 0) // 터치하지 않을때..
        {
            // 버튼에 대한 upSprite 초기화.
            for (int i = 0; i < buttonListLength; i++)
            {
                buttonList[i].GetComponent<SpriteRenderer>().sprite = pressUpSprite[i];
            }

            // directionkey 에 대한 노말 버튼 초기화.
            if (this.name.Equals("DirectionKey"))
            {
                this.GetComponent<SpriteRenderer>().sprite = NomalJoystick;
            }

        }


        for (int i = 0; i < Input.touchCount; i++)
        {


            Vector3 worldTouch = MainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
            hit = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero, Mathf.Infinity);

            if (hit.collider)
            {
               
                if (hit.collider.transform.parent.name.Equals("Button") && this.name.Equals("Button"))
                {
                    
                    buttontimer += Time.deltaTime;
                    buttonClickState = true;
                    buttonTouchCorllection = hit.collider.transform.parent.GetComponent<ButtonTouchCorllection>();
                    buttonDelay = buttonTouchCorllection.buttonDelay;

                    //buttonMinidx[buttonTouchCount] = buttonTouchCorllection.ButtonSelectTouchObject(worldTouch, buttonListLength, buttonList);
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                     
                       
                        //buttonMinidx[buttonTouchCount] = ButtonSelectTouchObject(worldTouch);
                        buttonMinidx[buttonTouchCount] = buttonTouchCorllection.ButtonSelectTouchObject(worldTouch, buttonListLength, buttonList);
                        buttonClick = buttonList[buttonMinidx[buttonTouchCount]].GetComponent<ButtonClick>();
                        buttonClick.DownClick(buttonList, buttonMinidx, buttonTouchCount, pressDownSprite);
                        buttonClick.UpState(buttonList, buttonMinidx, buttonTouchCount, buttonListLength, pressUpSprite, pressDownSprite);
                        buttonTouchCount++;
                        buttontimer = 0;
                    }
                    else if (buttontimer > buttonDelay && (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary))
                    {
                        
                        //Debug.Log(this.gameObject.GetComponent<ButtonTouchCorllection>().buttonDelay);
                        //buttonMinidx[buttonTouchCount] = ButtonSelectTouchObject(worldTouch);
                        buttonMinidx[buttonTouchCount] = buttonTouchCorllection.ButtonSelectTouchObject(worldTouch, buttonListLength, buttonList);
                        buttonClick = buttonList[buttonMinidx[buttonTouchCount]].GetComponent<ButtonClick>();
                        buttonClick.DownClick(buttonList, buttonMinidx, buttonTouchCount, pressDownSprite);
                        buttonClick.UpState(buttonList, buttonMinidx, buttonTouchCount, buttonListLength, pressUpSprite, pressDownSprite);
                        buttonTouchCount++;
                        buttontimer = 0;

                    }

                    else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        for (int j = 0; j < buttonListLength; j++)
                        {
                            buttonList[j].GetComponent<SpriteRenderer>().sprite = pressUpSprite[j];
                        }

                    }


                }
                else if (hit.collider.transform.parent.name.Equals("DirectionKey") && this.name.Equals("DirectionKey"))
                {
                    directionKeytimer += Time.deltaTime;
                    directionKeyClickState = true;
                    directionKeyTouchCorllection = hit.collider.transform.parent.GetComponent<DirectionkeyTouchCorllection>();
                    directionKeyDelay = directionKeyTouchCorllection.directionKeyDelay;
                    directionMinidx[directionTouchCount] = directionKeyTouchCorllection.DirectionSelectTouchObject(worldTouch, directionKeyLength, DirectionKeyList);
                    
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                       
                        //directionMinidx[directionTouchCount] = directionKeyTouchCorllection.DirectionSelectTouchObject(worldTouch, directionKeyLength, DirectionKeyList);
                        //directionMinidx[directionTouchCount] = DirectionSelectTouchObject(worldTouch);
                        directionKeyClick = DirectionKeyList[directionMinidx[directionTouchCount]].GetComponent<DirectionKeyClick>();
                        directionKeyClick.DownClick(DirectionKeyList, directionMinidx, directionTouchCount, directionPressDownSprite);
                        directionTouchCount++;
                        directionKeytimer = 0;
                    }
                    else if (directionKeytimer > directionKeyDelay && (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary))
                    {
                        
                        //directionMinidx[directionTouchCount] = directionKeyTouchCorllection.DirectionSelectTouchObject(worldTouch, directionKeyLength, DirectionKeyList);
                        //directionMinidx[directionTouchCount] = DirectionSelectTouchObject(worldTouch);
                        directionKeyClick = DirectionKeyList[directionMinidx[directionTouchCount]].GetComponent<DirectionKeyClick>();
                        directionKeyClick.DownClick(DirectionKeyList, directionMinidx, directionTouchCount, directionPressDownSprite);
                        directionTouchCount++;
                        directionKeytimer = 0;
                    }
                    else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        if (this.name.Equals("DirectionKey"))
                        {
                            this.GetComponent<SpriteRenderer>().sprite = NomalJoystick;
                        }

                    }

                }

            }
            else
            {
                Debug.Log("no hit ");
                if (buttonClickState)
                {
                    for (int j = 0; j < buttonListLength; j++)
                    {
                        buttonList[j].GetComponent<SpriteRenderer>().sprite = pressUpSprite[j];
                    }

                    buttonClickState = false;
                }
                else if (directionKeyClickState)
                {
                    if (this.name.Equals("DirectionKey"))
                    {
                        this.GetComponent<SpriteRenderer>().sprite = NomalJoystick;
                    }
                    directionKeyClickState = false;
                }
                else if (buttonClickState && directionKeyClickState)
                {
                    for (int j = 0; j < buttonListLength; j++)
                    {
                        buttonList[j].GetComponent<SpriteRenderer>().sprite = pressUpSprite[j];
                    }
                    if (this.name.Equals("DirectionKey"))
                    {
                        this.GetComponent<SpriteRenderer>().sprite = NomalJoystick;
                    }
                    buttonClickState = directionKeyClickState = false;
                }
            }


        }
   
	}

    

    
}

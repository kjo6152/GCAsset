using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
    *  컨트롤러 터치 보정
    *  기능 :
    *   1) Raycast 를 이용하여 생성한 컨틀롤러의 버튼 Object의 터치 영역을 설정.
    *   2) 터치 영역에서 터치한 좌표와 각각의 버튼 Object 와의 거리를 통해 가장 가까운 Object 선택.
    *   3) 버튼 Object들에 대한 초기화 작업.
    */

public class TouchCorrection : MonoBehaviour
{

    public Camera MainCamera;
    private Collider2D collider;
    private GameObject[] buttonList;
    private float[] Button_TouchDistance;
    private ButtonClick buttonClick;
    private Sprite[] pressUpSprite;
    private Sprite[] pressDownSprite;
    private int buttonListLength;

    /**
     * GameObject 가 생성되면서 Button List들에 대한 초기화.
     * 버튼들의 PressUp 이미지 초기화.
     */
    void Start()
    {
        //생성한 버튼 컴포넌트의 리스트 저장.
        buttonList = GameObject.FindGameObjectsWithTag("GameController");
        buttonListLength = buttonList.Length;
        pressUpSprite = new Sprite[buttonListLength];
        pressDownSprite = new Sprite[buttonListLength];

        //pressup 이미지 초기화.
        for (int i = 0; i<buttonListLength; i++)
        {
            pressUpSprite[i] = buttonList[i].GetComponent<SpriteRenderer>().sprite;
            pressDownSprite[i] = buttonList[i].GetComponent<ButtonClick>().downPressSprite;
            //Debug.Log("downSprite : " + buttonList[i].GetComponent<buttonClick>().downPressSprite);
        }
        
    }

    /**
     * 터치 좌표와 GameObject 간에 가장 가까운 object를 선택하여
     * 그 Object 의 Index 를 반환.
     */ 
    int SelectTouchObject(Vector3 touch)
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
     * 터치 좌표와 터치 영역간에 충돌확인.
     */
	void Update () {
        int[] minidx = new int[5];
        RaycastHit2D hit;
       
        if (Input.touchCount == 0) // 터치하지 않을때..
        {
            for (int i = 0; i < buttonListLength; i++)
            {
                buttonList[i].GetComponent<SpriteRenderer>().sprite = pressUpSprite[i];
            }
     
        }
       
        for (int i = 0; i < Input.touchCount; i++)
        {
            //Debug.Log(i +" : " +Input.GetTouch(i).position);
            Vector3 worldTouch = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
            hit = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero, Mathf.Infinity);

            if (hit.collider != null)
            {
                // Debug.Log("collider");
                minidx[i] = SelectTouchObject(worldTouch);
                buttonClick = buttonList[minidx[i]].GetComponent<ButtonClick>();
                buttonClick.DownClick(buttonList, minidx, i, pressDownSprite);
                buttonClick.UpState(buttonList, minidx, i, buttonListLength, pressUpSprite , pressDownSprite);
            }

            
        }

        
	}

    
}

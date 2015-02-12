using UnityEngine;
using System.Collections;

public class DirectionKeyTouchCorrection : MonoBehaviour
{
    public Camera MainCamera;
    private Collider2D collider;

    private GameObject[] DirectionKeyList;

    private float[] DirectionKey_TouchDistance;

    private DirectionKeyClick directionKeyClick;

    private Sprite[] directionPressDownSprite;
    private Sprite NomalJoystick;


    private int directionKeyLength;
    public float directionKeyDelay;
    float directionKeytimer;

    
    /**
     * GameObject 가 생성되면서 Button List들에 대한 초기화.
     * 버튼들의 PressUp 이미지 초기화.
     */
    void Start()
    {
        //생성한 버튼 컴포넌트의 리스트 저장.

        DirectionKeyList = GameObject.FindGameObjectsWithTag("DirectionKey");

        directionKeyLength = DirectionKeyList.Length;
        directionPressDownSprite = new Sprite[directionKeyLength];

        for (int i = 0; i < directionKeyLength; i++)
        {
            directionPressDownSprite[i] = DirectionKeyList[i].GetComponent<DirectionKeyClick>().downPressSprite;
        }
        NomalJoystick = this.GetComponent<SpriteRenderer>().sprite;


        



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

    void TouchCorrection()
    {
        int directionTouchCount = 0;

        int[] directionMinidx = new int[5];
        
        RaycastHit2D hit;

        if (Input.touchCount == 0) // 터치하지 않을때..
        {
            if (this.name.Equals("DirectionKey"))
            {
                this.GetComponent<SpriteRenderer>().sprite = NomalJoystick;
            }
        }

        for (int i = 0; i < Input.touchCount; i++)
        {

            Vector3 worldTouch = MainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
            hit = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero, Mathf.Infinity);

            try
            {
                if (hit.collider.transform.parent.name.Equals(this.name))
                {
                    directionKeytimer += Time.deltaTime;
                    
                    if (Input.GetTouch(i).phase == TouchPhase.Began)
                    {
                        // Debug.Log("joy hit ");
                        directionMinidx[directionTouchCount] = DirectionSelectTouchObject(worldTouch);
                        directionKeyClick = DirectionKeyList[directionMinidx[directionTouchCount]].GetComponent<DirectionKeyClick>();
                        directionKeyClick.DownClick(DirectionKeyList, directionMinidx, directionTouchCount, directionPressDownSprite);
                        directionTouchCount++;
                        directionKeytimer = 0;
                    }
                    else if (directionKeytimer > directionKeyDelay && (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary))
                    {
                        directionMinidx[directionTouchCount] = DirectionSelectTouchObject(worldTouch);
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
            catch
            {
                //터치영역이 아닐때 나는 효과음..
            }



        }
    }

    /**
     * 터치 좌표와 터치 영역간에 충돌확인.
     */
    void Update()
    {

        
        TouchCorrection();
        

    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class raycast : MonoBehaviour {
    
    public Camera MainCamera;
    
    private Collider2D collider;
    private GameObject[] buttonList;
    private float[] Button_TouchDistance;
    private buttonClick buttonClick;
    private Sprite sprite;

    void Start()
    {
        buttonList = GameObject.FindGameObjectsWithTag("Button");
        int i = 0;
        for (i = 0; i < 4; i++)
        {
            //Debug.Log("button :" + buttonList[i].name);
            //Debug.Log("button[" + i + "] x : " + buttonList[i].transform.position.x);
            //Debug.Log("button[" + i + "] y : " + buttonList[i].transform.position.y);
            //Debug.Log("center x :" + buttonList[i].gameObject.GetComponent<CircleCollider2D>().center.x);
            //Debug.Log("center y :" + buttonList[i].gameObject.GetComponent<CircleCollider2D>().center.y);


        }
    }

    GameObject SelectTouchObject(Vector3 touch)
    {
        //Debug.Log("touch : " + touch);
        Button_TouchDistance = new float[4];
        int i = 0;
        int minIdx = 0;
        float minDistance ;

        for (i = 0; i < 4; i++)
        {
            Button_TouchDistance[i] = Vector3.Distance(touch, buttonList[i].transform.position);
            //Debug.Log("거리 : " + Button_TouchDistance[i] + "button :+ " + buttonList[i].gameObject.name);
        }
        
        // 가장 가까운 버튼 선택 후 리턴.
        minDistance = Button_TouchDistance[0];
        
        for (i = 1; i < 4; i++)
        {
            if (minDistance > Button_TouchDistance[i])
            {
                minDistance = Button_TouchDistance[i];
                minIdx = i;
            }
        }
       // Debug.Log("minIdx " + minIdx);
        return buttonList[minIdx];
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0) )
        {
            Vector3 worldTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit;
            hit = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero, Mathf.Infinity);

            if (hit.collider != null)
            {
                GameObject selectButton = SelectTouchObject(worldTouch);
                //Debug.Log("select object :    " + selectButton.name);
                //Debug.Log("hit object :    " + hit.transform.gameObject.name);
                buttonClick = selectButton.GetComponent<buttonClick>();
                buttonClick.downClick();

                // if (hit.transform.name.Equals(hit.transform.gameObject.name)) // 여기에 터치 좌표와 비교하여 가장 가까운 object name 터치하도록 조정. 
                //{
                //   Debug.Log("select object :    " + selectButton.name);
                //hit.transform.GetComponent<>()
                //hit.transform.Translate(new Vector3(0.1f, 0, 0));
                //}


            }
            else
            {
                Debug.Log("no Hit");
            }

        }
        else if(Input.GetMouseButtonUp(0))
        {
                buttonClick.upClick();
         
        }
	}

    
}

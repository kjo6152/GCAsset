using UnityEngine;
using System.Collections;

public class Joystick : MonoBehaviour {

    public Camera MainCamera;
    private Sprite wheel;
    public int id;

    void Start()
    {
        Debug.Log(this);
        wheel = this.GetComponent<SpriteRenderer>().sprite;

        this.GetComponent<Transform>().position = new Vector3(0, 0, 0);
    }
	// Update is called once per frame
	void Update () {

        RaycastHit2D hit;

        if (Input.touchCount == 0) // 터치하지 않을때..
        {

            this.GetComponent<Transform>().position = this.transform.parent.position;

        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            
            Vector3 worldTouch = MainCamera.ScreenToWorldPoint(Input.GetTouch(i).position);
            hit = Physics2D.Raycast(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero, Mathf.Infinity);
            
            if(hit.collider)
            {
              
                if (hit.collider.name.Equals(this.transform.parent.name))
                {
                   
                   
                    if (Input.GetTouch(i).phase == TouchPhase.Ended)
                    {
                        this.GetComponent<Transform>().position = this.transform.parent.position;
                    }
                    else
                    {
                        this.GetComponent<Transform>().position = new Vector3(worldTouch.x, worldTouch.y, 1f);

                        worldTouch.x -= this.transform.parent.position.x;
                        worldTouch.y -= this.transform.parent.position.y;

                        this.GetComponent<JoyStickOnClick>().OnClick(id,worldTouch);
                    }

                }
              
            }
            else
            {
                this.GetComponent<Transform>().position = this.transform.parent.position;
                //터치영역이 아닐때 나는 효과음..
            }

        }
        
	}
}

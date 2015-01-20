using UnityEngine;
using System.Collections;

public class button1 : MonoBehaviour
{
      public Texture texture;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {

        //string PATH = "./power.jpeg";    //이미지 위치를 저장하는 변수
        //texture = Resources.Load(PATH) as Texture;  //이미지 로드

        GUI.Button(new Rect(20, 120, 100, 100), texture);


        /*if (GUI.Button(new Rect(10, 10, 100, 100), texture))
        {
            // to do something
        }*/
    }
}

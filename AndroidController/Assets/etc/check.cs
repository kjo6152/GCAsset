using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class check : MonoBehaviour
{
    EventTrigger t;
    public void clickTest()
    {
        
        Debug.Log("check");  
        Debug.Log(this.GetComponent<RectTransform>().anchoredPosition3D.x);

        //this.GetComponent<RectTransform>().anchoredPosition3D.x = 100 이런식은 읽기전용 변수라 변수 대입이안돼.
        // 새로운 객체를 생성해서 값을 float 변수로 넣어줘야함.
        this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(30f, 0f, 0f); // 버튼 x , y ,z 좌표 추가..
           
        Debug.Log(this.GetComponent<RectTransform>().anchoredPosition3D.y);
        Debug.Log(this.GetComponent<RectTransform>().anchoredPosition3D.z);

     
    }

}

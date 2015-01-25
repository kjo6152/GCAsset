using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class touch_coordinate : MonoBehaviour {

    Vector2[] touchPos = new Vector2[5];

    RaycastCollider a;
	// Update is called once per frame
	void Update () {
	// 현재 터치되어 있는 카운트 가져오기
        

        int cnt = Input.touchCount;
        
        // 동시에 여러곳을 터치 할 수 있기 때문.

        for( int i=0; i<cnt; ++i )

        {

            // i 번째로 터치된 값 이라고 보면 된다.

            Touch touch = Input.GetTouch(i);

            Vector2 pos = touch.position;
            GUI.Label(new Rect(70, 70, 300, 300), "touch(" + i + ") : x = " + pos.x + ", y = " + pos.y);
            Debug.Log( "touch(" + i + ") : x = " + pos.x + ", y = " + pos.y );

        }

	}
}
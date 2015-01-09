using UnityEngine;
using System.Collections;

/**
 * 1. 게임컨트롤러 컨텍스트
 * 기능 :
 *  1) 게임에 컨트롤러 라이브라리에 대한 전체적인 관리를 한다.
 *  2) 여러 매니저들의 초기화와 매니저의 인스턴스를 얻을 수 있다.
 *  3) 매니저 클래스는 다음과 같다.
 *   - 게임서버 매니저, 이벤트 매니저, 리소스 매니저
 * 
 * 매소드 :
 */
public class GCcontext : MonoBehaviour {
	string ResourcePath;

	ResourceManager mResourceManager;
	EventManager mEventManager;
	ServerManager mServerManager;

	void init(){
		ResourcePath = Application.dataPath + "GCResources";

		mResourceManager = new ResourceManager ();
		mEventManager = new EventManager ();
		mServerManager = new ServerManager ();

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

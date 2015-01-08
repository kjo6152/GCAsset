using UnityEngine;
using System.Collections;

/**
 * 4. 리소스 매니저
 * 기능 :
 *  1) 게임에서 사용되는 리소스의 목록을 만들고 안드로이드로 전송한다. 
 *  2) 커스텀 컨트롤러의 씬을 파싱하여 안드로이드로 보낼 수 있도록 리소스 형태(xml)로 만든다.
 */

public class ResourceManager : MonoBehaviour {


	void init(string path){
		this.mResourcesPath = path;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

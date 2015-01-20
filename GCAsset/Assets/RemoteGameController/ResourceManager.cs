using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;

/**
 * 4. 리소스 매니저
 * 기능 :
 *  1) 게임에서 사용되는 리소스의 목록을 만들고 안드로이드로 전송한다. 
 *  2) 커스텀 컨트롤러의 씬을 파싱하여 안드로이드로 보낼 수 있도록 리소스 형태(xml)로 만든다.
 */

/**
 * Todo : View를 제외한 리소스가 정상적으로 생성되는지 확인하고 서버 매니저에서
 * 리소스 요청을 받았을 때 해당 파일의 패스를 리턴해주어 서버 매니저에서 전송할 수 있도록 한다.
 */
public class ResourceManager {
	string mResourcesPath;
	int mResourceLength;

	const string NAME_RESOURCE_DIR = "/RemoteGameController/Resources";
	const string NAME_RESOURCE_MAP = "/ResourceMap.xml";
	const string NAME_VIEW_MAP = "/View.xml";

	/**
	 * XML에서 쓰이는 엘리먼트 이름과 속성 이름
	 */ 
	const string XML_ELEMENT_RESOURCE = "Resource";
	const string XML_ATTR_ID = "id";
	const string XML_ATTR_NAME = "name";
	const string XML_ATTR_TYPE = "type";
	const string XML_ATTR_SIZE = "size";

	XElement mResourceMap;
	public void init(){
		Debug.Log ("init");
		mResourcesPath = Application.dataPath + NAME_RESOURCE_DIR;
		mResourceLength = 0;
		createViewMap ();
		createResourceMap ();
	}

	// Use this for initialization
	void Start () {
		init ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/**
	 * 컨트롤러 씬으로부터 오브젝트를 파싱하여 xml 파일로 생성
	 */ 
	void createViewMap()
	{
		StreamWriter sw = new StreamWriter(mResourcesPath + NAME_VIEW_MAP);
		//Todo : 컨트롤러 씬으로부터 오브젝트를 파싱하여 xml 파일로 생성



		sw.Close ();
	}

	/**
	 * 리소스의 정보를 저장하는 리소스 맵 xml 파일을 만든다.
	 */ 
	void createResourceMap(){
		Debug.Log ("createResourceMap");

		if (!Directory.Exists(mResourcesPath))return;
		File.Delete (mResourcesPath + NAME_RESOURCE_MAP);
		string[] resourceList = Directory.GetFiles (mResourcesPath);

		string resourcePath;
		mResourceMap = new XElement ("ResourceList");
		XElement resource;
		XAttribute attr;

		for(mResourceLength=0;mResourceLength<resourceList.Length;mResourceLength++)
		{
			resourcePath = resourceList[mResourceLength];
			resource = new XElement(XML_ELEMENT_RESOURCE);
			attr = new XAttribute(XML_ATTR_ID,mResourceLength);
			resource.Add(attr);
			attr = new XAttribute(XML_ATTR_NAME,Path.GetFileName(resourcePath)); 
			resource.Add(attr);
			attr = new XAttribute(XML_ATTR_TYPE,""); 
			resource.Add(attr);
			attr = new XAttribute(XML_ATTR_SIZE,File.Open (resourcePath,FileMode.Open).Length);
			resource.Add(attr);
			mResourceMap.Add(resource);
		}
		StreamWriter sw = new StreamWriter (mResourcesPath + NAME_RESOURCE_MAP);
		sw.WriteLine (mResourceMap);
		sw.Close ();
	}

	/**
	 * 코드(아이디)를 가진 리소스 엘리먼트를 리턴한다.
	 */
	XElement getElement(int code){
		IEnumerator elements = mResourceMap.Elements (XML_ELEMENT_RESOURCE).GetEnumerator();
		XElement element;
		XAttribute attr;
		while (elements.MoveNext()) {
			element = (XElement)elements.Current;
			attr = element.Attribute(XML_ATTR_ID);
			if(attr!=null&&Convert.ToInt32(attr.Value)==code)return element;
		}
		return null;
	}


	/**
	 * 코드(리소스의 아이디)를 통해 해당 리소스가 저장 패스를 리턴해준다.
	 */
	public string getResourcePath(int code){

		XElement element = getElement (code);
		if (element == null)return null;
		return mResourcesPath + NAME_RESOURCE_DIR + element.Attribute(XML_ATTR_NAME).Value;
	}

	/**
	 * 코드(아이디)를 가진 리소스의 크기를 리턴한다.
	 */ 
	public int getResourceSize(int code){
		XElement element = getElement (code);
		if (element == null)return -1;
		return Convert.ToInt32(element.Attribute(XML_ATTR_SIZE).Value);
	}

	/**
	 * 리소스가 저장된 위치를 리턴한다.
	 */ 
	public string getResourceDirectory(){
		return mResourcesPath;
	}

	/**
	 * 리소스의 개수를 리턴한다.
	 */
	public int getResourceLength(){
		return mResourceLength;
	}

	/**
	 * 이름을 가진 리소스 엘리먼트를 리턴한다.
	 */
	XElement getElement(string name){
		IEnumerator elements = mResourceMap.Elements (XML_ELEMENT_RESOURCE).GetEnumerator();
		XElement element;
		XAttribute attr;
		while (elements.MoveNext()) {
			element = (XElement)elements.Current;
			attr = element.Attribute(XML_ATTR_NAME);
			if(attr!=null&&attr.Value==name)return element;
		}
		return null;
	}

	/**
	 * 이름을 통해 리소스 타입 속성을 얻는다.
	 */ 
	public string getResourceType(string name){
		XElement element = getElement (name);
		if (element == null)return null;
		return element.Attribute(XML_ATTR_TYPE).Value;
	}

	/**
	 * 이름을 통해 리소스 아이디 속성을 얻는다.
	 */ 
	public int getResourceCode(string name){
		XElement element = getElement (name);
		if (element == null)return -1;
		return Convert.ToInt32 (element.Attribute (XML_ATTR_ID).Value);
	}

	/**
	 * 이름을 통해 리소스 파일 크기 속성을 얻는다.
	 */ 
	public int getResourceSize(string name){
		XElement element = getElement (name);
		if (element == null)return -1;
		return Convert.ToInt32 (element.Attribute (XML_ATTR_SIZE).Value);
	}

	public string getResourceMap(){
		return mResourcesPath + NAME_RESOURCE_MAP;
	}
}

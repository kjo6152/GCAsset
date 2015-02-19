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
    /**
     * 리소스 파일의 경로
     */ 
    public static string NAME_RESOURCE_DIR = "Assets/GCServer/Resources/";
    public static string NAME_RESOURCE_MAP = "ResourceMap";

    /**
     * XML에서 쓰이는 엘리먼트 이름과 속성 이름
     */
    public static string XML_ELEMENT_RESOURCE = "Resource";
    public static string XML_ATTR_ID = "id";
    public static string XML_ATTR_NAME = "name";
    public static string XML_ATTR_TYPE = "type";
    public static string XML_ATTR_SIZE = "size";


	string mResourcesPath;
	int mResourceLength;
    byte[][] assetbytes;

	XElement mResourceMap;
	public void init(){
		Debug.Log ("init");
		mResourceLength = 0;
        LoadResourceMap(NAME_RESOURCE_MAP);
        
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
     * 기존의 리소스맵에서부터 정보를 읽어온다.
     */
    public void LoadResourceMap(string name){
        TextAsset asset = Resources.Load(name) as TextAsset;
        string xml = asset.text;
        mResourceMap = XElement.Parse(xml);
        LoadResourceLength();
        LoadResourcesBytes();
    }

    /**
     * 리소스의 개수를 구한다.
     */ 
    void LoadResourceLength()
    {
        IEnumerator elements = mResourceMap.Elements(XML_ELEMENT_RESOURCE).GetEnumerator();
        while (elements.MoveNext())
        {
            mResourceLength++;
        }
    }

    void LoadResourcesBytes()
    {
        assetbytes = new byte[mResourceLength+1][];
        TextAsset asset;
        string name;
        for(int i=1;i<=mResourceLength;i++){
            name = getResourceName(i).Split('.')[0];
            Debug.Log("resource : " + name);
            asset = Resources.Load(name) as TextAsset;
            assetbytes[i] = asset.bytes;
            Debug.Log("LoadResourcesBytes - " + name + " / " + assetbytes[i].Length);
        }
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
     * 코드(아이디)를 가진 리소스의 바이트를 얻는다.
     */
    public byte[] getResourceByte(int code)
    {
        return assetbytes[code];
    }

	/**
	 * 코드(리소스의 아이디)를 통해 해당 리소스의 이름을 리턴해준다.
	 */
	public string getResourceName(int code){
		XElement element = getElement (code);
		if (element == null)return null;
		return element.Attribute(XML_ATTR_NAME).Value;
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

    /**
     * 리소스 맵을 string으로 변환한다.
     */
	public string getResourceMap(){
		return mResourceMap.ToString();
	}
}

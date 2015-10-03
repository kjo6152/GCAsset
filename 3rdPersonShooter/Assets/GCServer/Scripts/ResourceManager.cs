using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;

/**
 * @breif GCAsset와 컨트롤러간 주고 받을 리소스를 관리하는 클래스
 * @details 사용자가 생성한 AssetBundle을 컨트롤러로 전송할 수 있도록 xml을 파싱하여 정보를 가지고 있는 클래스이다.
 * AssetBundle에 대한 경로나 xml 문서의 명칭 등 상수를 가지고 있다.
 * @see ExportGCAssetBundles
 * @author jiwon
 */
public class ResourceManager {

    /** @breif 리소스 파일의 경로 */
    public static string NAME_RESOURCE_DIR = "Assets/GCServer/Resources/";
    /** @breif 리소스 파일 이름 */
    public static string NAME_RESOURCE_MAP = "ResourceMap";

    /** @breif  XML에서 쓰이는 엘리먼트 이름과 속성 이름 */
    public static string XML_ELEMENT_RESOURCE = "Resource";
    /** @breif  XML에서 쓰이는 엘리먼트 이름과 속성 이름 */
    public static string XML_ATTR_ID = "id";
    /** @breif  XML에서 쓰이는 엘리먼트 이름과 속성 이름 */
    public static string XML_ATTR_NAME = "name";
    /** @breif  XML에서 쓰이는 엘리먼트 이름과 속성 이름 */
    public static string XML_ATTR_TYPE = "type";
    /** @breif  XML에서 쓰이는 엘리먼트 이름과 속성 이름 */
    public static string XML_ATTR_SIZE = "size";

	int mResourceLength;
    byte[][] assetbytes;

	XElement mResourceMap;
	public void init(){
		Debug.Log ("init");
		mResourceLength = 0;
        LoadResourceMap(NAME_RESOURCE_MAP);
        
	}

    /**
     * @breif 리소스맵에서부터 정보를 읽어온다.
     * @details 클라이언트와 연결 시 전송할 수 있도록 리소스 개수, 이름, 바이트를 로드한다.
     * @see NAME_RESOURCE_DIR, NAME_RESOURCE_MAP
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
    
    /**
     * 리소스들을 byte형태로 로드해둔다.
     */ 
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

    /** @breif 코드(아이디)를 가진 리소스의 바이트를 리턴하는 매소드 */
    public byte[] getResourceByte(int code)
    {
        return assetbytes[code];
    }

	/** @breif 코드(리소스의 아이디)를 통해 해당 리소스의 이름을 리턴하는 매소드 */
	public string getResourceName(int code){
		XElement element = getElement (code);
		if (element == null)return null;
		return element.Attribute(XML_ATTR_NAME).Value;
	}

	/** @breif 코드(아이디)를 가진 리소스의 크기를 리턴하는 매소드 */ 
	public int getResourceSize(int code){
		XElement element = getElement (code);
		if (element == null)return -1;
		return Convert.ToInt32(element.Attribute(XML_ATTR_SIZE).Value);
	}

	/** @breif 리소스의 개수를 리턴하는 매소드 */
	public int getResourceLength(){
		return mResourceLength;
	}

    /** @breif 리소스 맵을 string으로 변환해주는 매소드 */
	public string getResourceMap(){
		return mResourceMap.ToString();
	}
}

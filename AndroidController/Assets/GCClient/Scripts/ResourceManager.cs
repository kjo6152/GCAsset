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
    string lastScene;
	string mResourcesPath;
	int mResourceLength;

    const string NAME_RESOURCE_DIR = "/GCClient/Resources/";
	const string NAME_RESOURCE_MAP = "ResourceMap.xml";
	const string NAME_VIEW_MAP = "View.xml";

	/**
	 * XML에서 쓰이는 엘리먼트 이름과 속성 이름
	 */ 
	const string XML_ELEMENT_RESOURCE = "Resource";
	const string XML_ATTR_ID = "id";
	const string XML_ATTR_NAME = "name";
	const string XML_ATTR_TYPE = "type";
	const string XML_ATTR_SIZE = "size";

	XElement mResourceMap;
    XElement mDeivceInfo;
    AssetBundle[] asset;

	public void init(){
		Debug.Log ("init");
        lastScene = null;
        mResourcesPath = Application.persistentDataPath + NAME_RESOURCE_DIR;
        Directory.CreateDirectory(mResourcesPath);
		mResourceLength = 0;
        createDeviceInfo();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setLastScene(string scene)
    {
        this.lastScene = scene;
    }

    public string getLastScene()
    {
        return this.lastScene;
    }

    void createDeviceInfo()
    {
        mDeivceInfo = new XElement("Device");
        XElement Name = new XElement("Name");
        XElement ResolutionX = new XElement("ResolutionX");
        XElement ResolutionY = new XElement("ResolutionY");

        Name.Value = SystemInfo.deviceModel;
        if (Screen.resolutions.Length > 0)
        {
            ResolutionX.Value = Screen.resolutions[0].width.ToString();
            ResolutionY.Value = Screen.resolutions[0].height.ToString();
        }
        else
        {
            ResolutionX.Value = "0";
            ResolutionY.Value = "0";
        }
        

        mDeivceInfo.Add(Name);
        mDeivceInfo.Add(ResolutionX);
        mDeivceInfo.Add(ResolutionY);

    }

    public string getDeviceXml()
    {
        return mDeivceInfo.ToString();
    }
	/**
	 * 리소스의 정보를 저장하는 리소스 맵 xml 파일을 만든다.
	 */ 
	public void createResourceMap(){
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
			attr = new XAttribute(XML_ATTR_ID,mResourceLength+1);
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

    /*
     * 기존의 리소스맵에서부터 정보를 읽어온다.
     */
    public void LoadResourceMap(string path){
        string xml = new StreamReader(path).ReadToEnd();
        mResourceMap = XElement.Parse(xml);
        IEnumerator elements = mResourceMap.Elements(XML_ELEMENT_RESOURCE).GetEnumerator();
        mResourceLength = 0;
        while (elements.MoveNext())
        {
            mResourceLength++;
        }
    }

    public void LoadResources()
    {
        string path;
        asset = new AssetBundle[mResourceLength];
        for (int i = 1; i <= mResourceLength; i++)
        {
            path = this.getResourcePath(i);
            Debug.Log("asset path : " + path);
            try
            {
                asset[i - 1] = AssetBundle.CreateFromFile(path);
            }
            catch (Exception e)
            {
                e.ToString();
            }
            
        }
    }

    public void UnloadResources()
    {
        for (int i = 1; i <= mResourceLength; i++)
        {
            try
            {
                asset[i - 1].Unload(true);
            }
            catch (Exception e)
            {
                e.ToString();
            }

        }
    }

    public UnityEngine.Object getResource(string name)
    {
        UnityEngine.Object obj = null;
        for (int i = 0; i < mResourceLength; i++)
        {
            obj = asset[i].Load(name);
            if (obj != null) return obj;
        }
        return obj;
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
        if (code == 0) return getResourceMap();
		XElement element = getElement (code);
		if (element == null)return null;
        return mResourcesPath + element.Attribute(XML_ATTR_NAME).Value;
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

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Xml.Linq;
using System.Text;

public class ExportAssetBundles : Editor
{
    
    [MenuItem("Assets/Build Resources AssetBundle")]
    static void BuildResourceAssetBundle () {
        
        // 선택된 디렉토리를 가져옵니다.
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log("Selected Folder: " + path);
        if (path.Length != 0) {
            ArrayList sceneArrayList = new ArrayList();
            string[] fileEntries = Directory.GetFiles(path);
            foreach(string fileName in fileEntries) {
                //메타 파일 및 리소스 맵 제외
                if (fileName.EndsWith(".meta") || fileName.EndsWith(".bytes") || fileName.EndsWith(ResourceManager.NAME_RESOURCE_MAP)) continue;
                //씬
                if (fileName.EndsWith(".unity"))
                {
                    sceneArrayList.Add(fileName);
                    continue;
                }
                //일반 오브젝트
                Object t = AssetDatabase.LoadMainAssetAtPath(fileName);
                if (t != null) {
                    string bundlePath = ResourceManager.NAME_RESOURCE_DIR + t.name + "_asset.bytes";
                    Debug.Log(t.name + " Building bundle at: " + bundlePath);
                    // 활성화된 선택으로 부터 리소스 파일을 빌드합니다.
                    BuildPipeline.BuildAssetBundle(t, null, bundlePath, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.UncompressedAssetBundle,BuildTarget.Android);
                }
            }
            if (sceneArrayList.Count > 0)
            {
                string[] scenes = (string[])sceneArrayList.ToArray(typeof(string));
                string mainScene = Path.GetFileName(scenes[0]).Replace(".unity","");
                BuildPipeline.BuildStreamedSceneAssetBundle(scenes, ResourceManager.NAME_RESOURCE_DIR + mainScene + "_asset.bytes", BuildTarget.Android, BuildOptions.UncompressedAssetBundle);
            }
        }

        createResourceMap();
    }

    /**
	 * 리소스의 정보를 저장하는 리소스 맵 xml 파일을 만든다.
	 */
    static void createResourceMap()
    {
        Debug.Log("createResourceMap");
        string[] fileList = Directory.GetFiles(ResourceManager.NAME_RESOURCE_DIR, "*.bytes");

        FileStream fs;
        string resourcePath;
        XElement mResourceMap = new XElement("ResourceList");
        XElement resource;
        XAttribute attr;
        for (int i = 0; i < fileList.Length; i++)
        {
            resourcePath = (string)fileList[i];
            Debug.Log("Resource : " + resourcePath);
            resource = new XElement(ResourceManager.XML_ELEMENT_RESOURCE);
            attr = new XAttribute(ResourceManager.XML_ATTR_ID, i + 1);
            resource.Add(attr);
            attr = new XAttribute(ResourceManager.XML_ATTR_NAME, Path.GetFileName(resourcePath));
            resource.Add(attr);
            attr = new XAttribute(ResourceManager.XML_ATTR_TYPE, "");
            resource.Add(attr);
            fs = File.Open(resourcePath, FileMode.Open);
            attr = new XAttribute(ResourceManager.XML_ATTR_SIZE, fs.Length);
            resource.Add(attr);
            mResourceMap.Add(resource);
            fs.Close();
        }
        resourcePath = ResourceManager.NAME_RESOURCE_DIR + ResourceManager.NAME_RESOURCE_MAP;
        Debug.Log("ResourceMap : " + resourcePath);
        Debug.Log(mResourceMap.ToString());
        fs = File.Open(resourcePath+".xml", FileMode.Create);
        byte[] xmlBuffer = new UTF8Encoding().GetBytes(mResourceMap.ToString());
        fs.Write(xmlBuffer, 0, xmlBuffer.Length);
        fs.Close();
    }
}
#endif

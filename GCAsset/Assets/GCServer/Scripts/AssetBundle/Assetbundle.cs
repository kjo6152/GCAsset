#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ExportAssetBundles : Editor
{
    static string ScenesPath = "Assets\\GCServer\\Scenes";
    static string ResourcePath = "Assets\\GCServer\\Resources";

    [MenuItem("Assets/Build Scenes AssetBundle")]
    static void ExportScenesAssetBundle()
    {
        string[] scenes = getSceneList();

        for (int i = 0; i < scenes.Length; i++)
        {
            Debug.Log(scenes[i]);
        }

        BuildPipeline.BuildStreamedSceneAssetBundle(scenes, ResourcePath + "\\Scene.unity3d", BuildTarget.Android, BuildOptions.UncompressedAssetBundle);
    }

    [MenuItem("Assets/Build Resources AssetBundle")]
    static void ExportResourcesAssetBundles()
    {
        //Object[] resourceList = AssetDatabase.LoadAllAssetsAtPath(ResourcePath);
        //Debug.Log(resourceList.Length + "");
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(null, resourceList, ResourcePath + "\\Resources.unity3d", BuildAssetBundleOptions.CollectDependencies);
    }

    static string[] getSceneList()
    {
        string[] fileList = Directory.GetFiles(ScenesPath);
        ArrayList sceneArrayList = new ArrayList();
        for (int i = 0; i < fileList.Length; i++)
        {
            if (fileList[i].EndsWith(".unity")) sceneArrayList.Add(fileList[i]);
        }
        return (string[])sceneArrayList.ToArray(typeof(string));
    }
}
#endif

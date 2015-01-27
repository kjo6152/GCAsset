#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ExportAssetBundles : Editor
{
    static string ScenesPath = "Assets\\GCServer\\Scenes";
    static string ResourcePath = "Assets\\GCServer\\Resources";

    [MenuItem("Assets/Build AssetBundle")]
    static void ExportAssetBundle()
    {
        string[] scenes = getSceneList();

        for (int i = 0; i < scenes.Length; i++)
        {
            Debug.Log(scenes[i]);
        }

        BuildPipeline.BuildStreamedSceneAssetBundle(scenes, ResourcePath + "/GCAssetBundle.unity3d", BuildTarget.Android, BuildOptions.UncompressedAssetBundle);
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

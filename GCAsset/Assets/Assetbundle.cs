#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class ExportAssetBundles : Editor
{
    
    [MenuItem("Assets/Build AssetBundle")]
    static void ExportAssetBundle()
    {
        string[] scenes = { "Assets/testScene.unity" };
        BuildPipeline.BuildStreamedSceneAssetBundle(scenes, "Assets/myAssetBundle.unity3d", BuildTarget.Android,BuildOptions.UncompressedAssetBundle);
    }

}
#endif

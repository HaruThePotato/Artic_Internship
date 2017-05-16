#if UNITY_EDITOR
using UnityEditor;

public class BuildingAssetsBundle {
    [MenuItem("Assets/AssetBundles")]

    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles",
                                        BuildAssetBundleOptions.None,
                                        EditorUserBuildSettings.activeBuildTarget);
    }
}
#endif
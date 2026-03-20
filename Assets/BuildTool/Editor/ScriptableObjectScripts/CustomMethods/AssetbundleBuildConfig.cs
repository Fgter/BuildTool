using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="BuildTool/BuildData/AssetBundle")]
public class AssetbundleBuildConfig : ScriptableObject
{
    public BuildAssetBundleOptions bundleOptions =
          BuildAssetBundleOptions.ForceRebuildAssetBundle |
             BuildAssetBundleOptions.ChunkBasedCompression;
    public string androidPath = @$"{Directory.GetParent(Application.dataPath)}\AssetBundles\Android";
    public string iosPath = @$"{Directory.GetParent(Application.dataPath)}\AssetBundles\iOS";
    public string pcPath = @$"{Directory.GetParent(Application.dataPath)}\AssetBundles\PC";
    public string androidStreamingPath = $"{Application.dataPath}/StreamingAssets/AssetBundles/Android";
    public string iosStreamingPath = $"{Application.dataPath}/StreamingAssets/AssetBundles/iOS";
    public string pcStreamingPath = $"{Application.dataPath}/StreamingAssets/AssetBundles/PC";

    public bool OnBuild(BuildTool.Platform platform)
    {
        string outputPath;
        AssetBundleManifest mainfest;

        if (Directory.Exists(androidStreamingPath))
            Directory.Delete(androidStreamingPath, true);
        if (Directory.Exists(iosStreamingPath))
            Directory.Delete(iosStreamingPath, true);
        if (Directory.Exists(pcStreamingPath))
            Directory.Delete(pcStreamingPath, true);
        switch (platform)
        {
            case BuildTool.Platform.Android:
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                    BuildPipeline.GetBuildTargetGroup(BuildTarget.Android),
                    BuildTarget.Android);

                outputPath = androidPath;
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                mainfest = BuildPipeline.BuildAssetBundles(outputPath,
                     bundleOptions,
                     BuildTarget.Android);
                if (mainfest != null)
                    Directory.Move(outputPath, androidStreamingPath);
                return mainfest != null;
            case BuildTool.Platform.IOS:
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                   BuildPipeline.GetBuildTargetGroup(BuildTarget.iOS),
                   BuildTarget.iOS);

                outputPath = iosPath;
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                mainfest = BuildPipeline.BuildAssetBundles(outputPath,
                    bundleOptions,
                    BuildTarget.iOS);
                if (mainfest != null)
                    Directory.Move(outputPath, iosStreamingPath);
                return mainfest != null;
            case BuildTool.Platform.PC:
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                   BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneWindows64),
                   BuildTarget.StandaloneWindows64);

                outputPath = pcPath;
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                mainfest = BuildPipeline.BuildAssetBundles(outputPath,
                     bundleOptions,
                     BuildTarget.StandaloneWindows64);
                if (mainfest != null)
                {
                    if (Directory.Exists(pcStreamingPath))
                        Directory.Delete(pcStreamingPath, true);
                    Directory.Move(outputPath, pcStreamingPath);
                }
                return mainfest != null;
            default:
                return false;
        }
    }
}

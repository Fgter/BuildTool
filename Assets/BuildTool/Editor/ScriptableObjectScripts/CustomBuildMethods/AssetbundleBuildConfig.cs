using System.IO;
using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildConfig/AssetBundle")]
    public class AssetbundleBuildConfig : ScriptableObject
    {
        public BuildAssetBundleOptions bundleOptions =
              BuildAssetBundleOptions.ForceRebuildAssetBundle |
                 BuildAssetBundleOptions.ChunkBasedCompression;
        public string androidStreamingPath;
        public string iosStreamingPath;
        public string pcStreamingPath;

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

                    outputPath = androidStreamingPath;
                    if (!Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    mainfest = BuildPipeline.BuildAssetBundles(outputPath,
                         bundleOptions,
                         BuildTarget.Android);
                    return mainfest != null;
                case BuildTool.Platform.IOS:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(
                       BuildPipeline.GetBuildTargetGroup(BuildTarget.iOS),
                       BuildTarget.iOS);

                    outputPath = iosStreamingPath;
                    if (!Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    mainfest = BuildPipeline.BuildAssetBundles(outputPath,
                        bundleOptions,
                        BuildTarget.iOS);
                    return mainfest != null;
                case BuildTool.Platform.PC:
                    EditorUserBuildSettings.SwitchActiveBuildTarget(
                       BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneWindows64),
                       BuildTarget.StandaloneWindows64);

                    outputPath = pcStreamingPath;
                    if (!Directory.Exists(outputPath))
                        Directory.CreateDirectory(outputPath);

                    mainfest = BuildPipeline.BuildAssetBundles(outputPath,
                         bundleOptions,
                         BuildTarget.StandaloneWindows64);
                    return mainfest != null;
                default:
                    return false;
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(androidStreamingPath))
                androidStreamingPath = $"{Application.dataPath}/StreamingAssets/AssetBundles/Android";
            if (string.IsNullOrEmpty(iosStreamingPath))
                iosStreamingPath = $"{Application.dataPath}/StreamingAssets/AssetBundles/iOS";
            if (string.IsNullOrEmpty(pcStreamingPath))
                pcStreamingPath = $"{Application.dataPath}/StreamingAssets/AssetBundles/PC";
        }
    }
}

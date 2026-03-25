using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildConfig/Platform/iOS")]
    public class iOSPlatformBuildConfig : PlatformBuildConfigBase
    {
        public override bool OnBuild(BuildPathConfig pathData, string[] scriptingDefines = null)
        {
            if (string.IsNullOrEmpty(Identification))
            {
                EditorUtility.DisplayDialog("提示", "当前平台的打包config文件未设置Identification", "确定");
                return false;
            }

            var buildPath = pathData.iOSLocalBuildPath;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            PrePostProcessConfig.instance.OnPreProcessBuild(BuildTarget.iOS);

            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, Identification);

            var now = DateTime.Now;
            string directoryPath;
            if (scriptingDefines != null && scriptingDefines.Contains("ORBIT_GM"))
                directoryPath = $"{packageName}_GM_iOS_V{PlayerSettings.bundleVersion}({PlayerSettings.iOS.buildNumber})_{now:yyyyMMdd}_{now:HHmm}";
            else
                directoryPath = $"{packageName}_iOS_V{PlayerSettings.bundleVersion}({PlayerSettings.iOS.buildNumber})_{now:yyyyMMdd}_{now:HHmm}";
            string fullName = Path.Combine(buildPath, directoryPath);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = fullName;
            string[] buildSceneNames = GetBuildScenes().ToArray();
            buildPlayerOptions.scenes = buildSceneNames;
            buildPlayerOptions.target = BuildTarget.iOS;
            buildPlayerOptions.options = buildOptions;
            buildPlayerOptions.extraScriptingDefines = scriptingDefines;

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (buildReport.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("打包失败!");
                return false;
            }
            return true;
        }
    }
}

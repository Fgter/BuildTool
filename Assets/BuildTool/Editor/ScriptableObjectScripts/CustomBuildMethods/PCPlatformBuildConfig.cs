using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildConfig/Platform/PC")]
    public class PCPlatformBuildConfig : PlatformBuildConfigBase
    {
        public override bool OnBuild(BuildPathConfig pathData, string[] scriptingDefines = null)
        {
            if (string.IsNullOrEmpty(Identification))
            {
                EditorUtility.DisplayDialog("提示", "当前平台的打包config文件未设置Identification", "确定");
                return false;
            }

            var buildPath = pathData.PCLocalBuildPath;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            PrePostProcessConfig.instance.OnPreProcessBuild(BuildTarget.StandaloneWindows64);

            var now = DateTime.Now;
            string directoryPath;
            if (scriptingDefines != null && scriptingDefines.Contains("ORBIT_GM"))
                directoryPath = Path.Combine(buildPath, $"{packageName}_GM_PC_{now:yyyyMMdd}_{now:HHmm}");
            else
                directoryPath = Path.Combine(buildPath, $"{packageName}_PC_{now:yyyyMMdd}_{now:HHmm}");
            string tmpName = $"{packageName}.exe";
            string fullPath = Path.Combine(directoryPath, tmpName);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = Path.Combine(fullPath);
            string[] buildSceneNames = GetBuildScenes().ToArray();
            buildPlayerOptions.scenes = buildSceneNames;
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
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

using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildConfig/Platform/Android")]
    public class AndroidPlatformBuildConfig : PlatformBuildConfigBase
    {
        public override bool OnBuild(BuildPathConfig pathData, string[] scriptingDefines = null)
        {
            if (string.IsNullOrEmpty(Identification))
            {
                EditorUtility.DisplayDialog("提示", "当前平台的打包config文件未设置Identification", "确定");
                return false;
            }

            var buildPath = pathData.AndroidLocalBuildPath;
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            PrePostProcessConfig.instance.OnPreProcessBuild(BuildTarget.Android);

            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, Identification);

            PlayerSettings.Android.keystoreName = pathData.keyStorePath;
            PlayerSettings.Android.keystorePass = pathData.keyStorePassword;
            PlayerSettings.Android.keyaliasName = pathData.keyaliasName;
            PlayerSettings.Android.keyaliasPass = pathData.keyaliasPassword;

            var now = DateTime.Now;
            string pkgName;
            if (scriptingDefines != null && scriptingDefines.Contains("ORBIT_GM"))
                pkgName = $"{packageName}_GM_V{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode})_{now:yyyyMMdd}_{now:HHmm}.apk";
            else
                pkgName = $"{packageName}_V{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode})_{now:yyyyMMdd}_{now:HHmm}.apk";
            string fullName = Path.Combine(buildPath, pkgName);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = fullName;
            string[] buildSceneNames = GetBuildScenes().ToArray();
            buildPlayerOptions.scenes = buildSceneNames;
            buildPlayerOptions.target = BuildTarget.Android;
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

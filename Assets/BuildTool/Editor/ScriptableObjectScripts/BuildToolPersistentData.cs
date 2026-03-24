using System;
using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildToolPersistentData")]
    public class BuildToolPersistentData : ScriptableObject
    {
        public BuildPathConfig BuildPathConfig { get; set; }
        public AndroidPlatformBuildConfig AndroidConfig { get; set; }
        public IOSPlatformBuildConfig IOSConfig { get; set; }
        public PCPlatformBuildConfig PCConfig { get; set; }
        public AssetbundleBuildConfig assetbundleConfig { get; set; }
        public BuildTool.AfterBuildProcess afterBuildProcess { get; set; }
        public bool BuildAssetBundle { get; set; }
        public CustomScriptingDefineConfig.CustomScriptingDefine customScriptingDefines { get; set; }
        public bool keepVersion { get; set; }

        private void OnEnable()
        {
            if (BuildPathConfig == null)
                BuildPathConfig = AssetDatabase.LoadAssetAtPath<BuildPathConfig>("Assets/BuildTool/Editor/ScriptableObjects/BuidPathConfig.asset");
            if (AndroidConfig == null)
                AndroidConfig = AssetDatabase.LoadAssetAtPath<AndroidPlatformBuildConfig>("Assets/BuildTool/Editor/ScriptableObjects/BuildConfig/Android/AndroidBuildConfig.asset");
            if (IOSConfig == null)
                IOSConfig = AssetDatabase.LoadAssetAtPath<IOSPlatformBuildConfig>("Assets/BuildTool/Editor/ScriptableObjects/BuildConfig/IOS/IOSBuildConfig.asset");
            if (PCConfig == null)
                PCConfig = AssetDatabase.LoadAssetAtPath<PCPlatformBuildConfig>("Assets/BuildTool/Editor/ScriptableObjects/BuildConfig/PC/PCBuildConfig.asset");
            if (assetbundleConfig == null)
                assetbundleConfig = AssetDatabase.LoadAssetAtPath<AssetbundleBuildConfig>("Assets/BuildTool/Editor/ScriptableObjects/BuildConfig/Assetbundle/AssetbundleBuildConfig.asset");
        }
    }
}

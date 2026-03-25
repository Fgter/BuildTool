using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildToolPersistentData")]
    public class BuildToolPersistentData : ScriptableObject
    {
        [HideInInspector]
        public BuildPathConfig BuildPathConfig;
        [HideInInspector]
        public AndroidPlatformBuildConfig AndroidConfig;
        [HideInInspector]
        public IOSPlatformBuildConfig IOSConfig;
        [HideInInspector]
        public PCPlatformBuildConfig PCConfig;
        [HideInInspector]
        public AssetbundleBuildConfig assetbundleConfig;
        [HideInInspector]
        public BuildTool.AfterBuildProcess afterBuildProcess;
        [HideInInspector]
        public bool BuildAssetBundle;
        [HideInInspector]
        public CustomScriptingDefineConfig.CustomScriptingDefine customScriptingDefines;
        [HideInInspector]
        public bool keepVersion;

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

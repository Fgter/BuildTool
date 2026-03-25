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
        [HideInInspector]
        public bool distinguishVersion;
        private string androidVersion;
        private string iosVersion;

        public string AndroidVersion
        {
            get
            {
                if (string.IsNullOrEmpty(androidVersion))
                    androidVersion = PlayerSettings.bundleVersion;
                return androidVersion;
            }
            set
            {
                androidVersion = value;
            }
        }

        public string iOSVersion
        {
            get
            {
                if (string.IsNullOrEmpty(iosVersion))
                    iosVersion = PlayerSettings.bundleVersion;
                return iosVersion;
            }
            set
            {
                iosVersion = value;
            }
        }


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

        private void Reset()
        {
            androidVersion = "";
            iOSVersion = "";
        }
    }
}

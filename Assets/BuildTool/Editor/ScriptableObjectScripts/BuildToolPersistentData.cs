using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildToolPersistentData")]
    public class BuildToolPersistentData : ScriptableObject
    {
        [HideInInspector]
        public BuildPathConfig buildPathConfig;
        [HideInInspector]
        public AndroidPlatformBuildConfig androidConfig;
        [HideInInspector]
        public iOSPlatformBuildConfig iosConfig;
        [HideInInspector]
        public PCPlatformBuildConfig pcConfig;
        [HideInInspector]
        public AssetbundleBuildConfig assetbundleConfig;
        [HideInInspector]
        public BuildTool.AfterBuildProcess afterBuildProcess;
        [HideInInspector]
        public bool buildAssetBundle;
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
    }
}

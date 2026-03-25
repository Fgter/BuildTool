using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildToolPersistentData")]
    public class BuildToolPersistentData : ScriptableObject
    {
        [SerializeField, HideInInspector]
        public BuildPathConfig buildPathConfig;
        [SerializeField, HideInInspector]
        public AndroidPlatformBuildConfig androidConfig;
        [SerializeField, HideInInspector]
        public iOSPlatformBuildConfig iosConfig;
        [SerializeField, HideInInspector]
        public PCPlatformBuildConfig pcConfig;
        [SerializeField, HideInInspector]
        public AssetbundleBuildConfig assetbundleConfig;
        [SerializeField, HideInInspector]
        public BuildTool.AfterBuildProcess afterBuildProcess;
        [SerializeField, HideInInspector]
        public bool buildAssetBundle;
        [SerializeField, HideInInspector]
        public CustomScriptingDefineConfig.CustomScriptingDefine customScriptingDefines;
        [SerializeField, HideInInspector]
        public bool keepVersion;
        [SerializeField, HideInInspector]
        public bool distinguishVersion;
        [SerializeField, HideInInspector]
        public string androidVersion;
        [SerializeField, HideInInspector]
        public string iosVersion;

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

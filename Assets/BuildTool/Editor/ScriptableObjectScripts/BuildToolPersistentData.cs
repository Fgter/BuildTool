using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildToolPersistentData")]
    public class BuildToolPersistentData : ScriptableObject
    {
        [SerializeField,HideInInspector]
        private BuildPathConfig buildPathConfig;
        [SerializeField, HideInInspector]
        private AndroidPlatformBuildConfig androidConfig;
        [SerializeField, HideInInspector]
        private iOSPlatformBuildConfig iosConfig;
        [SerializeField, HideInInspector]
        private PCPlatformBuildConfig pcConfig;
        [SerializeField, HideInInspector]
        private AssetbundleBuildConfig assetbundleConfig;
        [SerializeField, HideInInspector]
        private BuildTool.AfterBuildProcess afterBuildProcess;
        [SerializeField, HideInInspector]
        private bool buildAssetBundle;
        [SerializeField, HideInInspector]
        private CustomScriptingDefineConfig.CustomScriptingDefine customScriptingDefines;
        [SerializeField, HideInInspector]
        private bool keepVersion;
        [SerializeField, HideInInspector]
        private bool distinguishVersion;
        [SerializeField, HideInInspector]
        private string androidVersion;
        [SerializeField, HideInInspector]
        private string iosVersion;

        public BuildPathConfig BuildPathConfig
        {
            get => buildPathConfig;
            set
            {
                buildPathConfig = value;
                SaveChange();
            }
        }
        public AndroidPlatformBuildConfig AndroidConfig
        {
            get => androidConfig;
            set
            {
                androidConfig = value;
                SaveChange();
            }
        }
        public iOSPlatformBuildConfig iOSConfig
        {
            get => iosConfig;
            set
            {
                iosConfig = value;
                SaveChange();
            }
        }
        public PCPlatformBuildConfig PCConfig
        {
            get => pcConfig;
            set
            {
                pcConfig = value;
                SaveChange();
            }
        }
        public AssetbundleBuildConfig AssetbundleConfig
        {
            get => assetbundleConfig;
            set
            {
                assetbundleConfig = value;
                SaveChange();
            }
        }
        public BuildTool.AfterBuildProcess AfterBuildProcess
        {
            get => afterBuildProcess;
            set
            {
                afterBuildProcess = value;
                SaveChange();
            }
        }
        public bool BuildAssetBundle
        {
            get => buildAssetBundle;
            set
            {
                buildAssetBundle = value;
                SaveChange();
            }
        }
        public CustomScriptingDefineConfig.CustomScriptingDefine CustomScriptingDefines
        {
            get => customScriptingDefines;
            set
            {
                customScriptingDefines = value;
                SaveChange();
            }
        }
        public bool KeepVersion
        {
            get => keepVersion;
            set
            {
                keepVersion = value;
                SaveChange();
            }
        }
        public bool DistinguishVersion
        {
            get => distinguishVersion;
            set
            {
                distinguishVersion = value;
                SaveChange();
            }
        }
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
                SaveChange();
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
                SaveChange();
            }
        }

        private void SaveChange()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}

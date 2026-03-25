using System.IO;
using UnityEngine;

namespace BuildTool
{
    [CreateAssetMenu(menuName = "BuildTool/BuildConfig/Path")]
    public class BuildPathConfig : ScriptableObject
    {
        [Header("打包路径")]
        public string AndroidLocalBuildPath;
        public string AndroidRemoteBuildPath = @"";
        public string iOSLocalBuildPath;
        public string iOSRemoteBuildPath = @"";
        public string PCLocalBuildPath;
        public string PCRemoteBuildPath = @"";
        [Space]
        [Header("KeyStore")]
        [SerializeField]
        [Tooltip("相对于项目根目录的路径")]
        [Header("相对于项目根目录的路径")]
        private string keyStorePath = @"";
        public string keyStorePassword = "";
        public string keyaliasName = "";
        public string keyaliasPassword = "";

        public string KeyStorePath
        {
            get
            {
                return Path.Combine(Directory.GetParent(Application.dataPath).FullName, keyStorePath);
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(AndroidLocalBuildPath))
                AndroidLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\AndroidBuilds";
            if (string.IsNullOrEmpty(iOSLocalBuildPath))
                iOSLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\iOSBuilds";
            if (string.IsNullOrEmpty(PCLocalBuildPath))
                PCLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\PCBuilds";
        }
    }
}

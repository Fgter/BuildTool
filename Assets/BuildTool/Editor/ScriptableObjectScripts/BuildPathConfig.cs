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
        public string IOSLocalBuildPath;
        public string IOSRemoteBuildPath = @"";
        public string PCLocalBuildPath;
        public string PCRemoteBuildPath = @"";
        [Space]
        [Header("KeyStore")]
        public string keyStorePath = @"";
        public string keyStorePassword = "";
        public string keyaliasName = "";
        public string keyaliasPassword = "";

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(AndroidLocalBuildPath))
                AndroidLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\AndroidBuilds";
            if (string.IsNullOrEmpty(IOSLocalBuildPath))
                IOSLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\IOSBuilds";
            if (string.IsNullOrEmpty(PCLocalBuildPath))
                PCLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\PCBuilds";
            if (string.IsNullOrEmpty(keyStorePath))
                keyStorePath = $@"{Directory.GetParent(Application.dataPath)}\KeyStore\";
        }
    }
}

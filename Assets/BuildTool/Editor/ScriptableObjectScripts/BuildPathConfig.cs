using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "BuildTool/BuildData/Path")]
public class BuildPathConfig : ScriptableObject
{
    [Header("打包路径")]
    public string AndroidLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\AndroidBuilds";
    public string AndroidRemoteBuildPath = @"";
    public string IOSLocalBuildPath = $@"{Directory.GetParent(Application.dataPath)}\Builds\IOSBuilds";
    public string IOSRemoteBuildPath = @"";
    public string PCLocalBuildPath= $@"{Directory.GetParent(Application.dataPath)}\Builds\PCBuilds";
    public string PCRemoteBuildPath = @"";
    [Space]
    [Header("KeyStore")]
    public string keyStorePath = @"";
    public string keyStorePassword = "";
    public string keyaliasName = "";
    public string keyaliasPassword = "";
}

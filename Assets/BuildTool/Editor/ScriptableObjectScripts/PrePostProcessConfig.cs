using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CreateAssetMenu(menuName = "BuildTool/BuildConfig/PrePostProcess")]
public class PrePostProcessConfig : ScriptableObject
{
    [System.Serializable]
    public class MoveFileItem
    {
        public string sourcePath;
        public string destniationPath;

        public MoveFileItem(string key, string value)
        {
            this.sourcePath = key;
            this.destniationPath = value;
        }
    }

    private static PrePostProcessConfig _instance;
    public static PrePostProcessConfig instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<PrePostProcessConfig>("Assets/BuildTool/Editor/ScriptableObjects/BuildConfig/PrePostProcess/PrePostProcessConfig.asset");
                if (_instance == null)
                {
                    _instance = CreateInstance<PrePostProcessConfig>();
                    AssetDatabase.CreateAsset(instance, "Assets/BuildTool/Editor/ScriptableObjects/BuildConfig/PrePostProcess/PrePostProcessConfig.asset");
                    AssetDatabase.SaveAssets();
                }
            }
            return _instance;
        }
    }

    [Header("移动文件处理\ntips:后处理地址填入OnPostProcessBuild中\npathToBuiltProject下的地址.\npathToBuiltProject为打包出来的文件夹\n如 ios:E:\\UnityProjects\\BuildTool\\Builds\\IOSBuilds\\XXX_IOS_V0.1.16(8)_20260324_1705\\")]
    [Header("Android")]
    public List<MoveFileItem> androidPreMoveFileProcess;
    [Header("IOS")]
    public List<MoveFileItem> iosPreMoveFileProcess;
    public List<MoveFileItem> iosPostMoveFileProcess;
    [Header("PC")]
    public List<MoveFileItem> pcPreMoveFileProcess;
    public List<MoveFileItem> pcPostMoveFileProcess;

    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        List<MoveFileItem> moveFilePostProcess = target switch
        {
            BuildTarget.iOS => PrePostProcessConfig.instance.iosPostMoveFileProcess,
            BuildTarget.StandaloneWindows64 => PrePostProcessConfig.instance.pcPostMoveFileProcess,
            _ => null
        };

        if (moveFilePostProcess == null)
            return;

        foreach (var p in moveFilePostProcess)
        {
            if (string.IsNullOrEmpty(p.sourcePath) || string.IsNullOrEmpty(p.destniationPath))
            {
                Debug.LogError("后处理文件PrePostProcessConfig中当前平台的移动文件后处理的地址有空");
                continue;
            }

            string destinationPath = Path.Combine(pathToBuiltProject, p.destniationPath);
            File.Copy(p.sourcePath, destinationPath, true);
        }
    }

    public void OnPreProcessBuild(BuildTarget target)
    {
        List<MoveFileItem> moveFilePostProcess = target switch
        {
            BuildTarget.Android => PrePostProcessConfig.instance.androidPreMoveFileProcess,
            BuildTarget.iOS => PrePostProcessConfig.instance.iosPreMoveFileProcess,
            BuildTarget.StandaloneWindows64 => PrePostProcessConfig.instance.pcPreMoveFileProcess,
            _ => throw new System.NotImplementedException()
        };

        foreach (var p in moveFilePostProcess)
        {
            Debug.Log(p.sourcePath);
            if(string.IsNullOrEmpty(p.sourcePath) || string.IsNullOrEmpty(p.destniationPath))
            {
                Debug.LogError("后处理文件PrePostProcessConfig中当前平台的移动文件后处理的地址有空");
                continue;
            }

            File.Copy(p.sourcePath, p.destniationPath, true);
        }
    }
}

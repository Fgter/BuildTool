using System.Diagnostics;
using System.IO;
using System.Linq;
using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.IO.Compression;
using System.Collections.Generic;

namespace BuildTool
{
    public class BuildTool : EditorWindow
    {
        public static string BuildWindowPath = "Assets/BuildTool/Editor/BuildWindow.uxml";
        public static string BuildToolDataPath = "Assets/BuildTool/Editor/ScriptableObjects/BuildPersistentData/BuildToolPersistentData.asset";

        public enum Platform
        {
            Android = 1,
            iOS = 2,
            PC = 4
        }
        public enum AfterBuildProcess
        {
            无,
            仅压缩,
            压缩并复制到公共盘
        }

        private BuildToolPersistentData data;

        private ObjectField pathConfigField;
        private ObjectField androidBuildConfigField;
        private ObjectField iosBuildConfigField;
        private ObjectField pcBuildConfigField;
        private ObjectField assetbundleBuildConfigField;
        private EnumFlagsField customScriptingDefineField;
        private Toggle buildAssetBundleToggle;
        private EnumField afterBuildProcessFiled;
        private EnumField platformFiled;
        private TextField longVersionText;
        private TextField shortVersionText;
        private Toggle keepVersionToggle;
        private Toggle distinguishVersion;
        private Button switchPlatformBtn;
        private Button buildBtn;

        private Button testBtn;

        private Platform platform;
        private bool inited = false;

        private BuildPathConfig pathData { get => (pathConfigField.value as BuildPathConfig); }
        private AndroidPlatformBuildConfig androidData { get => (androidBuildConfigField.value as AndroidPlatformBuildConfig); }
        private iOSPlatformBuildConfig iosData { get => (iosBuildConfigField.value as iOSPlatformBuildConfig); }
        private PCPlatformBuildConfig pcData { get => (pcBuildConfigField.value as PCPlatformBuildConfig); }
        private AssetbundleBuildConfig assetbundleData { get => (assetbundleBuildConfigField.value as AssetbundleBuildConfig); }
        private string[] scriptingDefines { get => ProcessScriptingDefine(); }

        [MenuItem("打包工具/打开BuildWindow")]
        #region UI
        public static void OpenBuildWindow()
        {
            var window = GetWindow<BuildTool>();
            window.titleContent = new GUIContent("打包");
            window.minSize = new Vector2(520, 450);
        }

        private void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(BuildWindowPath);
            visualTree.CloneTree(rootVisualElement);
            data = AssetDatabase.LoadAssetAtPath<BuildToolPersistentData>(BuildToolDataPath);
            if (data == null)
            {
                string directoryPath = BuildToolDataPath.Replace("/BuildToolPersistentData.asset", "");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                var instance = CreateInstance<BuildToolPersistentData>();
                AssetDatabase.CreateAsset(instance, BuildToolDataPath);
                AssetDatabase.SaveAssets();
            }

            pathConfigField = rootVisualElement.Q<ObjectField>("BuildPathConfig");
            androidBuildConfigField = rootVisualElement.Q<ObjectField>("AndroidBuildConfig");
            iosBuildConfigField = rootVisualElement.Q<ObjectField>("iOSBuildConfig");
            pcBuildConfigField = rootVisualElement.Q<ObjectField>("PCBuildConfig");
            assetbundleBuildConfigField = rootVisualElement.Q<ObjectField>("AssetBundleConfig");
            customScriptingDefineField = rootVisualElement.Q<EnumFlagsField>("CustomScriptingDefine");
            afterBuildProcessFiled = rootVisualElement.Q<EnumField>("AutoCopyToRemoteFolder");
            platformFiled = rootVisualElement.Q<EnumField>("PlatformEnum");
            longVersionText = rootVisualElement.Q<TextField>("LongVersion");
            shortVersionText = rootVisualElement.Q<TextField>("ShortVersion");
            keepVersionToggle = rootVisualElement.Q<Toggle>("KeepVersion");
            distinguishVersion = rootVisualElement.Q<Toggle>("DistinguishVersion");
            switchPlatformBtn = rootVisualElement.Q<Button>("SwitchPlatformBtn");
            buildBtn = rootVisualElement.Q<Button>("BuildBtn");
            buildAssetBundleToggle = rootVisualElement.Q<Toggle>("BuildAssetBundle");
            testBtn = rootVisualElement.Q<Button>("TestBtn");

            RefreshUI();
            AddListener();
            inited = true;
        }

        private void RefreshUI()
        {
            Init();
            RefreshDatas();
            RefreshVersion();
            //打包配置显示
            androidBuildConfigField.style.display = DisplayStyle.None;
            iosBuildConfigField.style.display = DisplayStyle.None;
            pcBuildConfigField.style.display = DisplayStyle.None;
            switch (platform)
            {
                case Platform.Android:
                    androidBuildConfigField.style.display = DisplayStyle.Flex;
                    break;
                case Platform.iOS:
                    iosBuildConfigField.style.display = DisplayStyle.Flex;
                    break;
                case Platform.PC:
                    pcBuildConfigField.style.display = DisplayStyle.Flex;
                    break;
            }
            //打包按钮显示
            buildBtn.SetEnabled(CheckPlatform());
            //切换平台按钮显示
            switchPlatformBtn.SetEnabled(!CheckPlatform());
            assetbundleBuildConfigField.style.display = data.BuildAssetBundle ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void AddListener()
        {
            pathConfigField.RegisterValueChangedCallback(v => { data.BuildPathConfig = (BuildPathConfig)v.newValue; });
            androidBuildConfigField.RegisterValueChangedCallback(v => { data.AndroidConfig = (AndroidPlatformBuildConfig)v.newValue; });
            iosBuildConfigField.RegisterValueChangedCallback(v => { data.iOSConfig = (iOSPlatformBuildConfig)v.newValue; });
            pcBuildConfigField.RegisterValueChangedCallback(v => { data.PCConfig = (PCPlatformBuildConfig)v.newValue; });
            assetbundleBuildConfigField.RegisterValueChangedCallback(v => { data.AssetbundleConfig = (AssetbundleBuildConfig)v.newValue; });
            customScriptingDefineField.RegisterValueChangedCallback(v => { data.CustomScriptingDefines = (CustomScriptingDefineConfig.CustomScriptingDefine)v.newValue; });
            longVersionText.RegisterValueChangedCallback(OnLongVersionChange);
            shortVersionText.RegisterValueChangedCallback(OnShortVersionChange);
            platformFiled.RegisterValueChangedCallback(OnPlatformChange);
            switchPlatformBtn.clicked += SwitchPlatform;
            buildBtn.clicked += Build;
            afterBuildProcessFiled.RegisterValueChangedCallback(v => { data.AfterBuildProcess = (AfterBuildProcess)v.newValue; });
            buildAssetBundleToggle.RegisterValueChangedCallback(v => { data.BuildAssetBundle = v.newValue; assetbundleBuildConfigField.style.display = v.newValue ? DisplayStyle.Flex : DisplayStyle.None; });
            keepVersionToggle.RegisterValueChangedCallback(v => { data.KeepVersion = v.newValue; });
            distinguishVersion.RegisterValueChangedCallback(v => { data.DistinguishVersion = v.newValue; });
            testBtn.clicked += Test;
        }

        private void Init()
        {
            if (!inited)
            {
                //平台
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.Android:
                        platformFiled.Init(Platform.Android);
                        platform = Platform.Android;
                        break;
                    case BuildTarget.iOS:
                        platformFiled.Init(Platform.iOS);
                        platform = Platform.iOS;
                        break;
                    case BuildTarget.StandaloneWindows64:
                        platformFiled.Init(Platform.PC);
                        platform = Platform.PC;
                        break;
                    default:
                        platformFiled.Init(Platform.Android);
                        platform = Platform.Android;
                        break;
                }
            }
            else
                platformFiled.Init(platform);
            afterBuildProcessFiled.Init(data.AfterBuildProcess);
        }

        private void RefreshVersion()
        {
            var isPC = platform == Platform.PC;
            longVersionText.SetEnabled(!isPC);
            shortVersionText.SetEnabled(!isPC);
            keepVersionToggle.SetEnabled(!isPC);
            distinguishVersion.SetEnabled(!isPC);
            if (!isPC)
            {
                keepVersionToggle.value = data.KeepVersion;
                distinguishVersion.value = data.DistinguishVersion;
            }
            else
            {
                keepVersionToggle.SetValueWithoutNotify(false);
                distinguishVersion.SetValueWithoutNotify(false);
            }

            if (distinguishVersion.value)
                PlayerSettings.bundleVersion = platform == Platform.Android ? data.AndroidVersion : data.iOSVersion;
            longVersionText.value = PlayerSettings.bundleVersion;
            switch (platform)
            {
                case Platform.Android:
                    shortVersionText.value = PlayerSettings.Android.bundleVersionCode.ToString();//android内部版本号
                    break;
                case Platform.iOS:
                    shortVersionText.value = PlayerSettings.iOS.buildNumber;
                    break;
                case Platform.PC:
                    shortVersionText.value = "";
                    longVersionText.value = "";
                    break;
            }
        }

        private void RefreshDatas()
        {
            pathConfigField.value = data.BuildPathConfig;
            androidBuildConfigField.value = data.AndroidConfig;
            iosBuildConfigField.value = data.iOSConfig;
            pcBuildConfigField.value = data.PCConfig;
            assetbundleBuildConfigField.value = data.AssetbundleConfig;
            customScriptingDefineField.Init(data.CustomScriptingDefines);
            afterBuildProcessFiled.Init(data.AfterBuildProcess);
            buildAssetBundleToggle.value = data.BuildAssetBundle;
            keepVersionToggle.value = data.KeepVersion;
            distinguishVersion.value = data.DistinguishVersion;
        }
        #endregion

        #region ListenerMethods
        private void OnPlatformChange(ChangeEvent<Enum> value)
        {
            platform = (Platform)value.newValue;
            RefreshUI();
        }

        private void OnLongVersionChange(ChangeEvent<string> value)
        {
            if (platform == Platform.PC)
                return;
            PlayerSettings.bundleVersion = value.newValue;
            if (distinguishVersion.value)
                switch (platform)
                {
                    case Platform.Android:
                        data.AndroidVersion = value.newValue;
                        break;
                    case Platform.iOS:
                        data.iOSVersion = value.newValue;
                        break;
                }
            AssetDatabase.SaveAssets();
        }

        private void OnShortVersionChange(ChangeEvent<string> value)
        {
            if (string.IsNullOrEmpty(value.newValue))
                return;
            var version = int.Parse(value.newValue);
            switch (platform)
            {
                case Platform.Android:
                    PlayerSettings.Android.bundleVersionCode = version;//android内部版本号
                    break;
                case Platform.iOS:
                    PlayerSettings.iOS.buildNumber = version.ToString();//ios内部版本号
                    break;
            }
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region IncreaseVersion & SwitchPlatform Methods
        private void IncreaseVersion()
        {
            if (platform == Platform.PC)
                return;
            if (!keepVersionToggle.value)
            {
                try
                {
                    string version = PlayerSettings.bundleVersion;//通用版本号
                    string[] versionList = version.Split(new char[] { '.' });
                    string nowLittleVersion = versionList[versionList.Length - 1];
                    string newLittleVersion = (int.Parse(nowLittleVersion) + 1).ToString();
                    versionList[versionList.Length - 1] = newLittleVersion;
                    string newLongVersion = string.Join(".", versionList);
                    PlayerSettings.bundleVersion = newLongVersion;
                    if (distinguishVersion.value)
                    {
                        switch (platform)
                        {
                            case Platform.Android:
                                data.AndroidVersion = newLongVersion;
                                break;
                            case Platform.iOS:
                                data.iOSVersion = newLongVersion;
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("解析版本号失败:" + e);
                }
            }

            switch (platform)//平台版本号
            {
                case Platform.Android:
                    PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + 1;
                    break;
                case Platform.iOS:
                    PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
                    break;
            }
            RefreshVersion();

            AssetDatabase.Refresh();
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        private void SwitchPlatform()
        {

            switch (platform)
            {
                case Platform.Android:
                    SwitchToPlatform(BuildTarget.Android);
                    break;
                case Platform.iOS:
                    SwitchToPlatform(BuildTarget.iOS);
                    break;
                case Platform.PC:
                    SwitchToPlatform(BuildTarget.StandaloneWindows64);
                    break;
                default:
                    EditorUtility.DisplayDialog("提示", "切换平台方法中没有设置对应平台,请在BuildTool中设置后再次切换", "确定");
                    break;
            }
        }
        #endregion

        #region Build
        #region BuildMethods
        private void Build()
        {
            if (!CheckPlatform())
            {
                EditorUtility.DisplayDialog("平台切换", "请切换至对应平台再次打包", "确定");
                return;
            }

            if (buildAssetBundleToggle.value)
            {
                if (!BuildAssetBundels())
                {
                    EditorUtility.DisplayDialog("提示", "打包AssetBundle时出错", "确定");
                    return;
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            IncreaseVersion();
            switch (platform)
            {
                case Platform.Android:
                    OnBuild_Android(); break;
                case Platform.iOS:
                    OnBuild_iOS(); break;
                case Platform.PC:
                    OnBuild_PC(); break;
                default:
                    OnBuild_Android();
                    break;
            }
        }

        private void OnBuild_Android()
        {
            if (BuildAndroid())
            {
                string localPath = pathData.AndroidLocalBuildPath;
                string remotePath = pathData.AndroidRemoteBuildPath;

                var afterBuildProcess = (AfterBuildProcess)(afterBuildProcessFiled.value);
                if (afterBuildProcess == AfterBuildProcess.无)
                {
                    Process.Start("Explorer.exe", localPath);
                    return;
                }

                string file = FindLatestFile(localPath);
                switch (afterBuildProcess)
                {
                    case AfterBuildProcess.仅压缩:
                        Process.Start("Explorer.exe", localPath);
                        break;
                    case AfterBuildProcess.压缩并复制到公共盘:
                        try
                        {
                            if (string.IsNullOrEmpty(remotePath))
                                throw new Exception("公共盘地址为空");
                            CopyFile(Path.Combine(localPath, file), Path.Combine(remotePath, file));
                            Process.Start("Explorer.exe", remotePath);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            Process.Start("Explorer.exe", localPath);
                        }

                        break;
                    default:
                        break;
                }
            }

        }

        private void OnBuild_iOS()
        {
            if (BuildiOS())
            {
                string localPath = pathData.iOSLocalBuildPath;
                string remotePath = pathData.iOSRemoteBuildPath;

                var afterBuildProcess = (AfterBuildProcess)(afterBuildProcessFiled.value);
                if (afterBuildProcess == AfterBuildProcess.无)
                {
                    Process.Start("Explorer.exe", localPath);
                    return;
                }

                string file = FindLatestFile(localPath);
                switch (afterBuildProcess)
                {
                    case AfterBuildProcess.仅压缩:
                        CompressFolder(Path.Combine(localPath, file), Path.Combine(localPath, file));
                        Process.Start("Explorer.exe", localPath);
                        break;
                    case AfterBuildProcess.压缩并复制到公共盘:
                        CompressFolder(Path.Combine(localPath, file), Path.Combine(localPath, file));
                        file = FindLatestFile(localPath);
                        try
                        {
                            if (string.IsNullOrEmpty(remotePath))
                                throw new Exception("公共盘地址为空");
                            File.Move(Path.Combine(localPath, file), Path.Combine(remotePath, file));
                            Process.Start("Explorer.exe", remotePath);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            Process.Start("Explorer.exe", localPath);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnBuild_PC()
        {
            if (BuildPC())
            {
                string localPath = pathData.PCLocalBuildPath;
                string remotePath = pathData.PCRemoteBuildPath;

                var afterBuildProcess = (AfterBuildProcess)(afterBuildProcessFiled.value);
                if (afterBuildProcess == AfterBuildProcess.无)
                {
                    Process.Start("Explorer.exe", localPath);
                    return;
                }

                string file = FindLatestFile(localPath);
                switch (afterBuildProcess)
                {
                    case AfterBuildProcess.仅压缩:
                        DeleteDonotShipFolders(Path.Combine(localPath, file));
                        CompressFolder(Path.Combine(localPath, file), Path.Combine(localPath, file));
                        Process.Start("Explorer.exe", localPath);
                        break;
                    case AfterBuildProcess.压缩并复制到公共盘:
                        DeleteDonotShipFolders(Path.Combine(localPath, file));
                        CompressFolder(Path.Combine(localPath, file), Path.Combine(localPath, file));
                        file = FindLatestFile(localPath);
                        try
                        {
                            if (string.IsNullOrEmpty(remotePath))
                                throw new Exception("公共盘地址为空");
                            File.Move(Path.Combine(localPath, file), Path.Combine(remotePath, file));
                            Process.Start("Explorer.exe", remotePath);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            Process.Start("Explorer.exe", localPath);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region PlatformCustom
        private bool BuildAndroid()
        {
            if (androidData == null || pathData == null)
            {
                EditorUtility.DisplayDialog("提示", "路径配置或当前平台打包配置为空,请设置后再打包", "确定");
                return false;
            }
            return androidData.OnBuild(pathData, scriptingDefines);
        }

        private bool BuildiOS()
        {
            if (iosData == null || pathData == null)
            {
                EditorUtility.DisplayDialog("提示", "路径配置或当前平台打包配置为空,请设置后再打包", "确定");
                return false;
            }
            return iosData.OnBuild(pathData, scriptingDefines);
        }

        private bool BuildPC()
        {
            if (pcData == null || pathData == null)
            {
                EditorUtility.DisplayDialog("提示", "路径配置或当前平台打包配置为空,请设置后再打包", "确定");
                return false;
            }
            return pcData.OnBuild(pathData, scriptingDefines);
        }

        private bool BuildAssetBundels()
        {
            return assetbundleData.OnBuild(platform);
        }

        #endregion
        #endregion

        #region UtilityMethods
        private static void SwitchToPlatform(BuildTarget target)
        {
            if (EditorUserBuildSettings.activeBuildTarget != target)
            {
                try
                {
                    BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(target);

                    if (EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target))
                    {
                        Debug.Log($"切换平台:切换目标平台成功{target}");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("平台切换", $"切换到 {target} 平台失败", "确定");
                    }
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("错误", $"切换平台时发生错误: {e.Message}", "确定");
                }
            }
            else
            {
                Debug.Log($"切换平台:已是目标平台{target}");
            }
        }

        private bool CheckPlatform()
        {
            bool result = false;
            switch (platform)
            {
                case Platform.Android:
                    result = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
                    break;
                case Platform.iOS:
                    result = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;
                    break;
                case Platform.PC:
                    result = EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;
                    break;
                default:
                    result = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
                    break;
            }
            return result;
        }

        private string[] ProcessScriptingDefine()
        {
            var sds = customScriptingDefineField.value.ToString();
            List<string> sdList = new List<string>();
            if (sds == "0")
                return null;
            else if (sds == "-1")
            {
                foreach (var e in Enum.GetValues(customScriptingDefineField.value.GetType()))
                {
                    sdList.Add(e.ToString());
                }
            }
            else
                sdList.AddRange(sds.Split(','));

            return sdList.ToArray();
        }

        private static bool ExecuteShellScript(string shellName)
        {
            //tip:Program Files\Git\bin文件夹需在C盘
            ProcessStartInfo psi = new ProcessStartInfo();

            string rootPath = Application.dataPath.Replace("/Assets", "");
            psi.FileName = "bash";
            psi.Arguments = $"-c \"{rootPath}/{shellName}.sh\"";


            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;

            try
            {
                Process process = new Process();
                process.StartInfo = psi;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();
                int exitCode = process.ExitCode;

                Debug.Log($"SVNUpdate脚本输出: {output}");
                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"错误信息: {error}");

                // 根据退出代码处理结果
                if (exitCode == 0)
                {
                    EditorUtility.DisplayDialog("提示", "svn更新成功", "确定");
                    return true;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "svn更新失败,请检查相关错误", "确定");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", e.Message, "确定");
                Debug.LogError($"执行脚本失败: {e.Message}");
                return false;
            }
        }

        public static void CompressFolder(string sourcePath, string destinationPath)
        {
            destinationPath = destinationPath + ".zip";
            ZipFile.CreateFromDirectory(sourcePath, destinationPath, System.IO.Compression.CompressionLevel.Fastest, false);
        }

        public static void CopyFile(string sourcePath, string destinationPath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    Debug.LogError($"打包文件不存在{sourcePath}");
                    EditorUtility.DisplayDialog("错误", $"打包文件不存在{sourcePath}", "确定");
                    return;
                }

                string directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(sourcePath, destinationPath, true);
                Debug.Log($"文件已复制: {sourcePath} -> {destinationPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"复制文件时出错{ex.Message}");
                EditorUtility.DisplayDialog("错误", $"复制文件时出错{ex.Message}", "确定");
            }
        }

        public static string FindLatestFile(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"目录不存在: {path}");

            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);

                FileInfo[] files = new FileInfo[0];
                DirectoryInfo[] dirInfos = new DirectoryInfo[0];
                try
                {
                    files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    dirInfos = dir.GetDirectories("*.*", SearchOption.TopDirectoryOnly);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.LogError("无权访问目录");
                }


                if (files.Length == 0 && dirInfos.Length == 0)
                {
                    Debug.Log($"目录为空{path}");
                    return null;
                }

                var latestFile = files
                    .OrderByDescending(f => f.LastWriteTimeUtc)
                    .FirstOrDefault();
                var latestDirectory = dirInfos
                    .OrderByDescending(f => f.LastWriteTimeUtc)
                    .FirstOrDefault();
                string lastestName;
                if (latestFile == null)
                    lastestName = latestDirectory.Name;
                else if (latestDirectory == null)
                    lastestName = latestFile.Name;
                else
                    lastestName = latestFile.LastWriteTimeUtc > latestDirectory?.LastWriteTimeUtc ? latestFile.Name : latestDirectory.Name;

                if (lastestName != null)
                {
                    Debug.Log($"最新包体: {lastestName}");
                }

                return lastestName;
            }
            catch (Exception ex)
            {
                Debug.LogError($"扫描失败: {ex.Message}");
                return null;
            }
        }
        
        public static void DeleteDonotShipFolders(string directoryPath)
        {
            try
            {
                string[] directories = Directory.GetDirectories(directoryPath);
        
                foreach (string dir in directories)
                {
                    string folderName = Path.GetFileName(dir);
                    
                    if (folderName.EndsWith("BurstDebugInformation_DoNotShip", StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.Delete(dir, true);
                        Console.WriteLine($"已删除文件夹: {dir}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除文件夹时出错: {ex.Message}");
            }
        }
        #endregion

        public void Test()
        {
            IncreaseVersion();
        }

        #region Utilities
        [MenuItem("打包工具/OpenFolder/PersistentDataPath")]
        public static void OpenPersistentDataPath()
        {
            string path = Application.persistentDataPath;
            EditorUtility.RevealInFinder(path);
        }

        [MenuItem("打包工具/OpenFolder/DataPath")]
        public static void OpenDataPath()
        {
            string path = Application.dataPath;
            EditorUtility.RevealInFinder(path);
        }
        #endregion
    }
}
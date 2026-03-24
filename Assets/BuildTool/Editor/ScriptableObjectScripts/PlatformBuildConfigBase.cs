using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    public abstract class PlatformBuildConfigBase : ScriptableObject
    {
        [Header("签名包名")]
        public string Identification;
        [Header("外显包名")]
        public string packageName;

        public BuildOptions buildOptions = BuildOptions.None;
        public abstract bool OnBuild(BuildPathConfig pathData, string[] scriptingDefines = null);

        protected static List<string> GetBuildScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            List<string> sceneNames = new List<string>();
            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    sceneNames.Add(scene.path);
                }
            }

            return sceneNames;
        }
    }
}

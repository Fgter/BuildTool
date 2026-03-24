using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    [CustomEditor(typeof(CustomScriptingDefineConfig))]
    public class MyScriptableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CustomScriptingDefineConfig myObject = (CustomScriptingDefineConfig)target;

            GUILayout.Space(10);
            if (GUILayout.Button("编译脚本", GUILayout.Height(30)))
            {
                Compile(myObject);
            }
        }

        private string path;

        public void Compile(CustomScriptingDefineConfig config)
        {
            MonoScript script = MonoScript.FromScriptableObject(config);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            string absolutePath = Path.Combine(Application.dataPath, scriptPath.Replace("Assets/", ""));
            path = absolutePath;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("        {");

            int number = 0;
            foreach (var e in config.allCustomMacros)
            {
                if (!string.IsNullOrEmpty(e.ToString()))
                {
                    sb.AppendLine($"            {e} = 1 << {number},");
                    number++;
                }
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            string tempFile = Path.GetTempFileName();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    string line;

                    while ((line = reader.ReadLine()).Trim() != "public enum CustomScriptingDefine")
                    {
                        writer.WriteLine(line);
                    }
                    writer.WriteLine(line);
                    writer.Write(sb.ToString());
                }

                File.Delete(path);
                File.Move(tempFile, path);
                AssetDatabase.Refresh();
            }
            catch
            {
                File.Delete(tempFile);
                throw;
            }
        }
    }
}
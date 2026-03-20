using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEditor;

[CreateAssetMenu(menuName = "BuildTool/CustomScriptingDefineConfig")]
public class CustomScriptingDefineConfig : ScriptableObject
{
    public bool enableAutoCompile;
    public List<string> allCustomMacros = new List<string>();
    private string path;

    private void OnValidate()
    {
        if (!enableAutoCompile)
            return;
        Compile();
    }

    public void Compile()
    {
        MonoScript script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);
        string absolutePath = Path.Combine(Application.dataPath, scriptPath.Replace("Assets/", ""));
        path = absolutePath;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("    {");

        int number = 0;
        foreach (var e in allCustomMacros)
        {
            if (!string.IsNullOrEmpty(e.ToString()))
            {
                sb.AppendLine($"        {e} = {2 << number},");
                number++;
            }
        }
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

    [Flags]
    public enum CustomScriptingDefine
    {
        ORBIT_GM = 2,
    }
}

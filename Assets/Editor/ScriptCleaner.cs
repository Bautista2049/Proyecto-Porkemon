using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class ScriptCleaner : EditorWindow
{
    [MenuItem("Tools/Clean Script Comments")]
    public static void CleanAllScripts()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int cleanedFiles = 0;

        foreach (string file in files)
        {
            if (file.Contains("ScriptCleaner.cs")) continue;

            string content = File.ReadAllText(file);
            // Remove all comments
            string cleanedContent = Regex.Replace(content, @"//.*?$|/\*.*?\*/", "", RegexOptions.Singleline);
            
            // Remove all Debug.Log statements
            cleanedContent = Regex.Replace(cleanedContent, @"Debug\.Log\(.*?\)\s*;?\s*\n?", "");
            
            // Remove empty lines left by removed Debug statements
            cleanedContent = Regex.Replace(cleanedContent, @"\n\s*\n", "\n");
            
            if (content != cleanedContent)
            {
                File.WriteAllText(file, cleanedContent);
                cleanedFiles++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Cleaned {cleanedFiles} scripts");
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Platformer
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings");
            UnityEditor.AssetDatabase.Refresh();
        }

        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders) {
                var path = Path.Combine(Application.dataPath, root);
                foreach (var folder in folders) {
                    var folderPath = Path.Combine(path, folder);
                    if (!Directory.Exists(folderPath)) {
                        Directory.CreateDirectory(folderPath);
                    }
                }
            }
        }
    }
}

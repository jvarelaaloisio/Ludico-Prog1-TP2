using System;
using System.Collections.Generic;
using System.Linq;
using Prefs.Editor;
using Prefs.Runtime;
using UnityEditor;

namespace VarelaAloisio.Core.Editor
{
    public static class ToolbarProjectSettings
    {
        private const string PrefsKeyPrefix = "Toolbar";
        private static readonly EditorPrefsWrapper EditorPrefs = new();
        
        private const string BootScenePathKey = PrefsKeyPrefix + "BootScenePath";
        public static StringPref BootScenePath { get; }
        private static readonly string BootScenePathLabel = ObjectNames.NicifyVariableName(nameof(BootScenePath));
        private static List<string> scenePaths;
        private static int currentScenePathIndex;

        static ToolbarProjectSettings()
        {
            scenePaths = GetScenePaths();
            var candidates = scenePaths.Where(path
                                                  => path.Contains("boot", StringComparison.CurrentCultureIgnoreCase)
                                                     || path.Contains("Init", StringComparison.CurrentCultureIgnoreCase));

            string defaultPath = candidates.FirstOrDefault() ?? string.Empty;
            currentScenePathIndex = scenePaths.IndexOf(defaultPath);
            BootScenePath = new StringPref(EditorPrefs, BootScenePathKey, defaultPath);
        }

        [SettingsProvider]
        public static SettingsProvider CreateToolbarsSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Toolbar", SettingsScope.Project)
                           {
                               label = "Toolbar Settings",
                               guiHandler = GUIHandler,
                               keywords = new[] { "Toolbar", "Setting" }
                           };

            LoadPrefs();
            return provider;
        }

        private static List<string> GetScenePaths()
        {
            var sceneGUIDs = AssetDatabase.FindAssetGUIDs("t:Scene");
            var paths = sceneGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToList();
            paths.Insert(0, string.Empty);
            return paths;
        }

        private static void LoadPrefs()
        {
            if (BootScenePath.Exists)
            {
                BootScenePath.Load();
                currentScenePathIndex = scenePaths.IndexOf(BootScenePath);
            }
            else
            {
                BootScenePath.Reset();
                BootScenePath.Save();
            }
        }

        private static void GUIHandler(string searchContext)
        {
            EditorGUIUtility.labelWidth = 200;
            int newIndex = EditorGUILayout.Popup(currentScenePathIndex, scenePaths.ToArray());
            if (newIndex != currentScenePathIndex)
            {
                currentScenePathIndex = newIndex;
                BootScenePath.value = scenePaths[newIndex];
                BootScenePath.Save();
            }
        }
    }
}
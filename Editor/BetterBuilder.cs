using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BetterBuilder
{
    public static string projectPath;
    public static List<Object> excludedScenes = new List<Object>();

    [System.Serializable]
    private class SceneListWrapper
    {
        public List<string> excludedScenePaths = new List<string>();
    }

    // Methods
    public static void AddProjectScenesToBuild()
    {
        GetCurrentProjectPath();
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        var buildScenes = new List<EditorBuildSettingsScene>();

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            Object sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (!excludedScenes.Contains(sceneAsset))
            {
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }
        }
        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log($"{buildScenes.Count} scenes have been added to the Build Settings. {BetterBuilder.excludedScenes.Count} have been omitted.");
    }

    public static void SaveExcludedScenes()
    {
        GetCurrentProjectPath();
        List<string> scenePaths = new List<string>();
        foreach (var scene in excludedScenes)
        {
            if (scene != null)
            {
                string path = AssetDatabase.GetAssetPath(scene);
                scenePaths.Add(path);
            }
        }

        SceneListWrapper sceneWrapper = new SceneListWrapper();
        sceneWrapper.excludedScenePaths = scenePaths;

        string json = JsonUtility.ToJson(sceneWrapper);

        string savePath = GetExcludedScenesFilePath();
        File.WriteAllText(savePath, json);
    }

    public static void LoadExcludedScenes()
    {
        GetCurrentProjectPath();

        string savePath = GetExcludedScenesFilePath();
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SceneListWrapper sceneWrapper = JsonUtility.FromJson<SceneListWrapper>(json);

            excludedScenes.Clear();

            foreach (string scenePath in sceneWrapper.excludedScenePaths)
            {
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (scene != null)
                {
                    excludedScenes.Add(scene);
                }
            }
        }
    }

    private static string GetExcludedScenesFilePath()
    {
        string projectFolder = Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        return Path.Combine(Application.persistentDataPath, $"{projectFolder}_excludedScenes.json");
    }

    public static void GetCurrentProjectPath()
    {
        projectPath = Application.dataPath;
    }
}

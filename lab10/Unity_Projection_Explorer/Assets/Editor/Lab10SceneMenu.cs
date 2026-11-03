using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Lab10SceneMenu
{
    private const string ScenePath = "Assets/Scenes/Lab10_3DViewingPipeline.unity";

    [MenuItem("CGVR Labs/Lab 10 - Open 3D Viewing Pipeline")]
    private static void OpenLab10()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("CGVR Labs/Lab 10 - Validate Scene and Shader")]
    public static void Validate()
    {
        bool ok = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null &&
                  AssetDatabase.LoadAssetAtPath<Shader>("Assets/Shaders/Lab10FaceColors.shader") != null;
        if (ok) Debug.Log("Lab 10: scene and shader found. Press Play to test.");
        else Debug.LogError("Lab 10: missing scene or shader.");
    }

    [MenuItem("CGVR Labs/Lab 10 - Build Windows Standalone")]
    public static void BuildWindows()
    {
        string[] scenes = new string[] { ScenePath };
        string buildPath = "Build/Lab10_ProjectionExplorer.exe";
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Lab 10 Build succeeded: {report.summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"Lab 10 Build failed with {report.summary.totalErrors} errors.");
        }
    }
}

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PongSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Pong.unity";

    static PongSceneBuilder()
    {
        EditorApplication.delayCall += CreateSceneOnFirstOpen;
    }

    [MenuItem("Pong/Create Or Refresh Complete Scene")]
    public static void CreateOrRefreshScene()
    {
        Directory.CreateDirectory("Assets/Scenes");

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject gameObject = new GameObject("Pong Game");
        gameObject.AddComponent<PongGame>();

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorSceneManager.OpenScene(ScenePath);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Pong scene created. Press Play to start.");
    }

    private static void CreateSceneOnFirstOpen()
    {
        if (File.Exists(ScenePath))
        {
            return;
        }

        CreateOrRefreshScene();
    }
}

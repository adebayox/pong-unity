using UnityEngine;

public static class PongBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsurePongExists()
    {
        if (Object.FindFirstObjectByType<PongGame>() != null)
        {
            return;
        }

        GameObject gameObject = new GameObject("Pong Game");
        gameObject.AddComponent<PongGame>();
    }
}

using UnityEngine;

public sealed class PongGoal : MonoBehaviour
{
    private PongGame game;
    private int scoringPlayer;

    public void Configure(PongGame owner, int playerWhoScores)
    {
        game = owner;
        scoringPlayer = playerWhoScores;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PongBall>() == null)
        {
            return;
        }

        game.ScorePoint(scoringPlayer);
    }
}

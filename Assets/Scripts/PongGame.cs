using UnityEngine;

public sealed class PongGame : MonoBehaviour
{
    private enum GameMode
    {
        None,
        PlayerVsComputer,
        PlayerVsPlayer
    }

    private enum GameState
    {
        Menu,
        Serving,
        Playing,
        GameOver
    }

    private const float ArenaHalfWidth = 8f;
    private const float ArenaHalfHeight = 4.5f;
    private const float PaddleX = 7.25f;
    private const float GoalX = 8.75f;
    private const int WinningScore = 7;

    [SerializeField] private float humanPaddleSpeed = 8.5f;
    [SerializeField] private float computerPaddleSpeed = 6.6f;
    [SerializeField] private float ballSpeed = 7.25f;
    [SerializeField] private float serveDelay = 1.05f;

    private int leftScore;
    private int rightScore;
    private int nextServeDirection = 1;
    private float serveAt;
    private GameMode mode = GameMode.None;
    private GameState state = GameState.Menu;
    private TextMesh titleText;
    private TextMesh scoreText;
    private TextMesh statusText;
    private TextMesh controlsText;
    private PongBall ball;
    private PongPaddle leftPaddle;
    private PongPaddle rightPaddle;
    private Sprite squareSprite;
    private PhysicsMaterial2D bounceMaterial;

    private void Awake()
    {
        BuildGame();
        ShowMenu();
    }

    private void Update()
    {
        if (state == GameState.Menu)
        {
            HandleMenuInput();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
        {
            ShowMenu();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartMatch(mode);
            return;
        }

        if (state == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                StartMatch(mode);
            }

            return;
        }

        if (state == GameState.Serving)
        {
            UpdateServeCountdown();
        }
    }

    public void ScorePoint(int player)
    {
        if (state != GameState.Playing)
        {
            return;
        }

        if (player == 1)
        {
            leftScore += 1;
            nextServeDirection = 1;
        }
        else
        {
            rightScore += 1;
            nextServeDirection = -1;
        }

        UpdateScoreText();

        if (leftScore >= WinningScore || rightScore >= WinningScore)
        {
            ShowGameOver(player);
            return;
        }

        BeginServe();
    }

    private void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartMatch(GameMode.PlayerVsComputer);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartMatch(GameMode.PlayerVsPlayer);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void StartMatch(GameMode selectedMode)
    {
        mode = selectedMode;
        leftScore = 0;
        rightScore = 0;
        nextServeDirection = Random.value < 0.5f ? -1 : 1;

        leftPaddle.ConfigureHuman(KeyCode.W, KeyCode.S, humanPaddleSpeed, -3.3f, 3.3f);
        if (mode == GameMode.PlayerVsComputer)
        {
            rightPaddle.ConfigureComputer(ball, computerPaddleSpeed, -3.3f, 3.3f, -4.25f, 4.25f);
        }
        else
        {
            rightPaddle.ConfigureHuman(KeyCode.UpArrow, KeyCode.DownArrow, humanPaddleSpeed, -3.3f, 3.3f);
        }

        ResetCourt();
        UpdateScoreText();
        BeginServe();
    }

    private void BeginServe()
    {
        state = GameState.Serving;
        serveAt = Time.time + serveDelay;
        ball.StopAtCenter();
        titleText.text = ModeLabel();
        statusText.text = "Get ready";
        controlsText.text = InGameControls();
    }

    private void UpdateServeCountdown()
    {
        float remaining = Mathf.Max(0f, serveAt - Time.time);
        statusText.text = "Serve in " + Mathf.CeilToInt(remaining).ToString();

        if (Time.time < serveAt)
        {
            return;
        }

        state = GameState.Playing;
        titleText.text = "";
        statusText.text = "";
        ball.Serve(nextServeDirection);
    }

    private void ShowGameOver(int winningPlayer)
    {
        state = GameState.GameOver;
        ball.StopAtCenter();

        string winner;
        if (mode == GameMode.PlayerVsComputer)
        {
            winner = winningPlayer == 1 ? "You win" : "Computer wins";
        }
        else
        {
            winner = winningPlayer == 1 ? "Player 1 wins" : "Player 2 wins";
        }

        titleText.text = "GAME OVER";
        statusText.text = winner + "\nSpace: rematch    M: menu";
        controlsText.text = InGameControls();
    }

    private void ShowMenu()
    {
        mode = GameMode.None;
        state = GameState.Menu;
        leftScore = 0;
        rightScore = 0;
        ResetCourt();
        UpdateScoreText();

        titleText.text = "PONG";
        statusText.text = "1  Play vs computer\n2  Play vs another player";
        controlsText.text = "Player 1: W/S    Player 2: Up/Down    First to " + WinningScore.ToString();
    }

    private void ResetCourt()
    {
        leftPaddle.ResetPosition();
        rightPaddle.ResetPosition();
        ball.StopAtCenter();
    }

    private string ModeLabel()
    {
        if (mode == GameMode.PlayerVsComputer)
        {
            return "Player vs Computer";
        }

        if (mode == GameMode.PlayerVsPlayer)
        {
            return "Player vs Player";
        }

        return "Choose a mode";
    }

    private string InGameControls()
    {
        if (mode == GameMode.PlayerVsComputer)
        {
            return "W/S to move    R: restart    M/Esc: menu";
        }

        return "P1: W/S    P2: Up/Down    R: restart    M/Esc: menu";
    }

    private void BuildGame()
    {
        squareSprite = CreateSquareSprite();
        bounceMaterial = new PhysicsMaterial2D("Pong Bounce")
        {
            friction = 0f,
            bounciness = 1f
        };

        ConfigureCamera();
        CreateBackground();
        CreateCenterLine();
        CreateWalls();
        CreateGoals();
        leftPaddle = CreatePaddle("Player 1 Paddle", new Vector2(-PaddleX, 0f), new Color(0.96f, 0.42f, 0.34f));
        rightPaddle = CreatePaddle("Player 2 / Computer Paddle", new Vector2(PaddleX, 0f), new Color(0.32f, 0.74f, 0.78f));
        CreateBall();
        CreateText();
    }

    private void ConfigureCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        camera.transform.position = new Vector3(0f, 0f, -10f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.06f, 0.08f, 0.09f);
    }

    private void CreateBackground()
    {
        GameObject background = CreateSpriteObject("Background", Vector2.zero, new Vector2(16.5f, 9.2f), new Color(0.08f, 0.11f, 0.12f), -10);
        background.transform.position = new Vector3(0f, 0f, 2f);
    }

    private void CreateCenterLine()
    {
        for (int index = 0; index < 9; index += 1)
        {
            float y = -3.8f + index * 0.95f;
            CreateSpriteObject("Center Dash", new Vector2(0f, y), new Vector2(0.08f, 0.45f), new Color(0.78f, 0.81f, 0.74f, 0.75f), -1);
        }
    }

    private void CreateWalls()
    {
        CreateWall("Top Wall", new Vector2(0f, ArenaHalfHeight), new Vector2(ArenaHalfWidth * 2f, 0.25f));
        CreateWall("Bottom Wall", new Vector2(0f, -ArenaHalfHeight), new Vector2(ArenaHalfWidth * 2f, 0.25f));
    }

    private void CreateWall(string wallName, Vector2 position, Vector2 size)
    {
        GameObject wall = CreateSpriteObject(wallName, position, size, new Color(0.8f, 0.82f, 0.73f), 1);
        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.sharedMaterial = bounceMaterial;
    }

    private void CreateGoals()
    {
        CreateGoal("Left Goal", new Vector2(-GoalX, 0f), 2);
        CreateGoal("Right Goal", new Vector2(GoalX, 0f), 1);
    }

    private void CreateGoal(string goalName, Vector2 position, int scoringPlayer)
    {
        GameObject goal = new GameObject(goalName);
        goal.transform.position = position;

        BoxCollider2D collider = goal.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(0.4f, ArenaHalfHeight * 2f);

        PongGoal goalZone = goal.AddComponent<PongGoal>();
        goalZone.Configure(this, scoringPlayer);
    }

    private PongPaddle CreatePaddle(string paddleName, Vector2 position, Color color)
    {
        GameObject paddle = CreateSpriteObject(paddleName, position, new Vector2(0.28f, 1.45f), color, 2);

        BoxCollider2D collider = paddle.AddComponent<BoxCollider2D>();
        collider.sharedMaterial = bounceMaterial;

        Rigidbody2D body = paddle.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        return paddle.AddComponent<PongPaddle>();
    }

    private void CreateBall()
    {
        GameObject ballObject = CreateSpriteObject("Ball", Vector2.zero, new Vector2(0.34f, 0.34f), new Color(0.98f, 0.96f, 0.84f), 3);

        CircleCollider2D collider = ballObject.AddComponent<CircleCollider2D>();
        collider.sharedMaterial = bounceMaterial;

        Rigidbody2D body = ballObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;

        ball = ballObject.AddComponent<PongBall>();
        ball.Configure(ballSpeed);
    }

    private void CreateText()
    {
        titleText = CreateTextObject("Title", new Vector3(0f, 2.55f, 0f), 0.16f, new Color(0.96f, 0.95f, 0.86f));
        scoreText = CreateTextObject("Score", new Vector3(0f, 4.04f, 0f), 0.16f, new Color(0.96f, 0.95f, 0.86f));
        statusText = CreateTextObject("Status", new Vector3(0f, 0.95f, 0f), 0.08f, new Color(0.9f, 0.92f, 0.84f));
        controlsText = CreateTextObject("Controls", new Vector3(0f, -4.08f, 0f), 0.055f, new Color(0.7f, 0.74f, 0.7f));
    }

    private TextMesh CreateTextObject(string objectName, Vector3 position, float characterSize, Color color)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.position = position;

        TextMesh text = textObject.AddComponent<TextMesh>();
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.characterSize = characterSize;
        text.fontSize = 72;
        text.lineSpacing = 0.9f;
        text.color = color;

        MeshRenderer renderer = textObject.GetComponent<MeshRenderer>();
        renderer.sortingOrder = 10;

        return text;
    }

    private void UpdateScoreText()
    {
        scoreText.text = leftScore + " : " + rightScore;
    }

    private GameObject CreateSpriteObject(string objectName, Vector2 position, Vector2 size, Color color, int sortingOrder = 0)
    {
        GameObject spriteObject = new GameObject(objectName);
        spriteObject.transform.position = position;
        spriteObject.transform.localScale = size;

        SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
        renderer.sprite = squareSprite;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;

        return spriteObject;
    }

    private static Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1)
        {
            filterMode = FilterMode.Point
        };

        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}

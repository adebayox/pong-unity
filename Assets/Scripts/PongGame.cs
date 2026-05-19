using UnityEngine;

public sealed class PongGame : MonoBehaviour
{
    private const float ArenaHalfWidth = 8f;
    private const float ArenaHalfHeight = 4.5f;
    private const float PaddleX = 7.25f;
    private const float GoalX = 8.75f;

    [SerializeField] private float paddleSpeed = 8.5f;
    [SerializeField] private float ballSpeed = 7.25f;

    private int leftScore;
    private int rightScore;
    private TextMesh scoreText;
    private PongBall ball;
    private Sprite squareSprite;
    private PhysicsMaterial2D bounceMaterial;

    private void Awake()
    {
        BuildGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            leftScore = 0;
            rightScore = 0;
            UpdateScoreText();
            ball.ResetAndLaunch(Random.value < 0.5f ? -1 : 1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ScorePoint(int player)
    {
        if (player == 1)
        {
            leftScore += 1;
            ball.ResetAndLaunch(-1);
        }
        else
        {
            rightScore += 1;
            ball.ResetAndLaunch(1);
        }

        UpdateScoreText();
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
        CreatePaddle("Left Paddle", new Vector2(-PaddleX, 0f), KeyCode.W, KeyCode.S, new Color(0.96f, 0.42f, 0.34f));
        CreatePaddle("Right Paddle", new Vector2(PaddleX, 0f), KeyCode.UpArrow, KeyCode.DownArrow, new Color(0.32f, 0.74f, 0.78f));
        CreateBall();
        CreateText();
        UpdateScoreText();
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

    private void CreatePaddle(string paddleName, Vector2 position, KeyCode up, KeyCode down, Color color)
    {
        GameObject paddle = CreateSpriteObject(paddleName, position, new Vector2(0.28f, 1.45f), color, 2);

        BoxCollider2D collider = paddle.AddComponent<BoxCollider2D>();
        collider.sharedMaterial = bounceMaterial;

        Rigidbody2D body = paddle.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        PongPaddle controller = paddle.AddComponent<PongPaddle>();
        controller.Configure(up, down, paddleSpeed, -3.3f, 3.3f);
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
        scoreText = CreateTextObject("Score", new Vector3(0f, 4.04f, 0f), 0.16f, new Color(0.96f, 0.95f, 0.86f));

        TextMesh controls = CreateTextObject("Controls", new Vector3(0f, -4.08f, 0f), 0.055f, new Color(0.7f, 0.74f, 0.7f));
        controls.text = "W/S    R to reset    Up/Down";
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

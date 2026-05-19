using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PongPaddle : MonoBehaviour
{
    private enum ControlMode
    {
        Human,
        Computer
    }

    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float minY = -3.35f;
    [SerializeField] private float maxY = 3.35f;
    [SerializeField] private float aiDeadZone = 0.12f;
    [SerializeField] private float playfieldMinY = -4.25f;
    [SerializeField] private float playfieldMaxY = 4.25f;

    private Rigidbody2D body;
    private ControlMode controlMode = ControlMode.Human;
    private PongBall targetBall;
    private float humanMovement;

    public float Height => transform.localScale.y;

    public void ConfigureHuman(KeyCode up, KeyCode down, float paddleSpeed, float lowerLimit, float upperLimit)
    {
        controlMode = ControlMode.Human;
        upKey = up;
        downKey = down;
        speed = paddleSpeed;
        minY = lowerLimit;
        maxY = upperLimit;
        targetBall = null;
    }

    public void ConfigureComputer(PongBall ball, float paddleSpeed, float lowerLimit, float upperLimit, float lowerWall, float upperWall)
    {
        controlMode = ControlMode.Computer;
        targetBall = ball;
        speed = paddleSpeed;
        minY = lowerLimit;
        maxY = upperLimit;
        playfieldMinY = lowerWall;
        playfieldMaxY = upperWall;
    }

    public void ResetPosition()
    {
        EnsureBody();
        body.position = new Vector2(body.position.x, 0f);
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        humanMovement = 0f;
    }

    private void Awake()
    {
        EnsureBody();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
    }

    private void Update()
    {
        if (controlMode != ControlMode.Human)
        {
            return;
        }

        humanMovement = 0f;

        if (Input.GetKey(upKey))
        {
            humanMovement += 1f;
        }

        if (Input.GetKey(downKey))
        {
            humanMovement -= 1f;
        }
    }

    private void FixedUpdate()
    {
        EnsureBody();

        if (controlMode == ControlMode.Computer)
        {
            MoveComputer();
            return;
        }

        Vector2 nextPosition = body.position + Vector2.up * humanMovement * speed * Time.fixedDeltaTime;
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
        body.MovePosition(nextPosition);
    }

    private void MoveComputer()
    {
        float targetY = PredictComputerTargetY();
        float difference = targetY - body.position.y;

        if (Mathf.Abs(difference) <= aiDeadZone)
        {
            return;
        }

        float nextY = Mathf.MoveTowards(body.position.y, targetY, speed * Time.fixedDeltaTime);
        nextY = Mathf.Clamp(nextY, minY, maxY);
        body.MovePosition(new Vector2(body.position.x, nextY));
    }

    private float PredictComputerTargetY()
    {
        if (targetBall == null || !targetBall.IsMoving)
        {
            return 0f;
        }

        Vector2 ballPosition = targetBall.Position;
        Vector2 ballVelocity = targetBall.Velocity;

        if (Mathf.Abs(ballVelocity.x) < 0.01f)
        {
            return ballPosition.y;
        }

        bool ballMovingTowardPaddle = Mathf.Sign(ballVelocity.x) == Mathf.Sign(transform.position.x);
        if (!ballMovingTowardPaddle)
        {
            return Mathf.MoveTowards(body.position.y, 0f, speed * Time.fixedDeltaTime);
        }

        float timeToReachPaddle = Mathf.Abs((transform.position.x - ballPosition.x) / ballVelocity.x);
        float predictedY = ballPosition.y + ballVelocity.y * timeToReachPaddle;
        return ReflectInsidePlayfield(predictedY);
    }

    private float ReflectInsidePlayfield(float y)
    {
        float height = playfieldMaxY - playfieldMinY;
        if (height <= 0.01f)
        {
            return Mathf.Clamp(y, minY, maxY);
        }

        while (y > playfieldMaxY || y < playfieldMinY)
        {
            if (y > playfieldMaxY)
            {
                y = playfieldMaxY - (y - playfieldMaxY);
            }
            else if (y < playfieldMinY)
            {
                y = playfieldMinY + (playfieldMinY - y);
            }
        }

        return Mathf.Clamp(y, minY, maxY);
    }

    private void EnsureBody()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PongPaddle : MonoBehaviour
{
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float minY = -3.35f;
    [SerializeField] private float maxY = 3.35f;

    private Rigidbody2D body;
    private float movement;

    public float Height => transform.localScale.y;

    public void Configure(KeyCode up, KeyCode down, float paddleSpeed, float lowerLimit, float upperLimit)
    {
        upKey = up;
        downKey = down;
        speed = paddleSpeed;
        minY = lowerLimit;
        maxY = upperLimit;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
    }

    private void Update()
    {
        movement = 0f;

        if (Input.GetKey(upKey))
        {
            movement += 1f;
        }

        if (Input.GetKey(downKey))
        {
            movement -= 1f;
        }
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition = body.position + Vector2.up * movement * speed * Time.fixedDeltaTime;
        nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
        body.MovePosition(nextPosition);
    }
}

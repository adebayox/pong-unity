using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PongBall : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float maxVerticalBounce = 0.85f;

    private Rigidbody2D body;
    private float pausedUntil;

    public void Configure(float ballSpeed)
    {
        speed = ballSpeed;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 0f;
#if UNITY_6000_0_OR_NEWER
        body.angularDamping = 0f;
        body.linearDamping = 0f;
#else
        body.angularDrag = 0f;
        body.drag = 0f;
#endif
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        ResetAndLaunch(Random.value < 0.5f ? -1 : 1);
    }

    private void FixedUpdate()
    {
        if (Time.time < pausedUntil)
        {
            return;
        }

        Vector2 velocity = PongVelocity.Get(body);
        if (velocity.sqrMagnitude < 0.01f)
        {
            Launch(Random.value < 0.5f ? -1 : 1);
            return;
        }

        if (Mathf.Abs(velocity.x) < speed * 0.35f)
        {
            velocity.x = Mathf.Sign(velocity.x == 0f ? Random.Range(-1f, 1f) : velocity.x) * speed * 0.5f;
        }

        PongVelocity.Set(body, velocity.normalized * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PongPaddle paddle = collision.collider.GetComponent<PongPaddle>();
        if (paddle == null)
        {
            return;
        }

        float normalizedHit = Mathf.Clamp(
            (transform.position.y - paddle.transform.position.y) / (paddle.Height * 0.5f),
            -1f,
            1f
        );

        float xDirection = paddle.transform.position.x < transform.position.x ? 1f : -1f;
        Vector2 direction = new Vector2(xDirection, normalizedHit * maxVerticalBounce).normalized;
        PongVelocity.Set(body, direction * speed);
    }

    public void ResetAndLaunch(int xDirection)
    {
        transform.position = Vector3.zero;
        body.position = Vector2.zero;
        body.angularVelocity = 0f;
        PongVelocity.Set(body, Vector2.zero);

        pausedUntil = Time.time + 0.75f;
        CancelInvoke(nameof(LaunchAfterPause));
        Invoke(nameof(LaunchAfterPause), 0.75f);

        pendingDirection = xDirection >= 0 ? 1 : -1;
    }

    private int pendingDirection = 1;

    private void LaunchAfterPause()
    {
        Launch(pendingDirection);
    }

    private void Launch(int xDirection)
    {
        float y = Random.Range(-0.45f, 0.45f);
        Vector2 direction = new Vector2(xDirection >= 0 ? 1f : -1f, y).normalized;
        PongVelocity.Set(body, direction * speed);
    }
}

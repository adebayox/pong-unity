using UnityEngine;

public static class PongVelocity
{
    public static Vector2 Get(Rigidbody2D body)
    {
#if UNITY_6000_0_OR_NEWER
        return body.linearVelocity;
#else
        return body.velocity;
#endif
    }

    public static void Set(Rigidbody2D body, Vector2 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        body.linearVelocity = velocity;
#else
        body.velocity = velocity;
#endif
    }
}

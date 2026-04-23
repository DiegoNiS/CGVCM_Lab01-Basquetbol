using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool hasScored = false;
    private bool enteredFromAbove = false;
    private bool isLaunched = false;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void Launch(Vector2 velocity)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.linearVelocity = velocity;
        isLaunched = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HoopTop") && isLaunched)
        {
            if (rb.linearVelocity.y < 0)
                enteredFromAbove = true;
        }

        if (other.CompareTag("HoopBottom") && enteredFromAbove && !hasScored)
        {
            hasScored = true;
            if (gameManager != null)
                gameManager.AddScore(2);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HoopTop") && rb.linearVelocity.y > 0)
            enteredFromAbove = false;
    }

    public void ResetBall()
    {
        hasScored = false;
        enteredFromAbove = false;
        isLaunched = false;
        if (rb == null) {
            rb = GetComponent<Rigidbody2D>();
        }
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public bool IsLaunched() => isLaunched;
}
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float groundY = -3f;

    [Header("Parámetros de Física del Lanzamiento")]
    public float launchForce = 8f;   
    public float launchAngle = 60f;
    public float gravity = -9.81f;

    [Header("Referencias")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public LineRenderer trajectoryLine;

    private Ball currentBall;
    private bool hasBall = true;
    private Rigidbody2D rb;

    private float leftLimit = -8f;
    private float rightLimit = 8f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        Physics2D.gravity = new Vector2(0, gravity);
        SpawnBall();
    }

    void Update()
    {
        HandleMovement();
        HandleAngleInput();

        if (hasBall)
        {
            DrawTrajectory();
            if (currentBall != null)
                currentBall.transform.position = ballSpawnPoint.position;
        }

        if (Input.GetKeyDown(KeyCode.Space) && hasBall)
            Shoot();

        if (Input.GetKeyDown(KeyCode.R) && !hasBall)
            RecoverBall();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float newX = transform.position.x + horizontal * moveSpeed * Time.deltaTime;
        newX = Mathf.Clamp(newX, leftLimit, rightLimit);
        transform.position = new Vector3(newX, groundY, 0);
    }

    void HandleAngleInput()
    {
        if (Input.GetKey(KeyCode.W))
            launchAngle = Mathf.Min(launchAngle + 60f * Time.deltaTime, 85f);
        if (Input.GetKey(KeyCode.S))
            launchAngle = Mathf.Max(launchAngle - 60f * Time.deltaTime, 10f);
        if (Input.GetKey(KeyCode.E))
            launchForce = Mathf.Min(launchForce + 5f * Time.deltaTime, 25f);
        if (Input.GetKey(KeyCode.Q))
            launchForce = Mathf.Max(launchForce - 5f * Time.deltaTime, 3f);
    }

    void Shoot()
    {
        if (currentBall == null) return;
        hasBall = false;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        Vector2 velocity = new Vector2(
            Mathf.Cos(angleRad) * launchForce,
            Mathf.Sin(angleRad) * launchForce
        );

        currentBall.Launch(velocity);

        if (trajectoryLine != null)
            trajectoryLine.positionCount = 0;

        // 5 segundos para que la pelota termine su trayectoria
        Invoke("AllowRecover", 5f);
    }

    void AllowRecover()
    {
        if (!hasBall)
            RecoverBall();
    }

    void RecoverBall()
    {
        CancelInvoke("AllowRecover");
        if (currentBall != null)
            Destroy(currentBall.gameObject);
        hasBall = true;
        SpawnBall();
    }

    void SpawnBall()
    {
        if (ballPrefab == null) return;
        GameObject ballObj = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        currentBall = ballObj.GetComponent<Ball>();
        currentBall.ResetBall();
    }

    void DrawTrajectory()
    {
        if (trajectoryLine == null) return;

        int steps = 25;
        trajectoryLine.positionCount = steps;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        float g = Physics2D.gravity.y; 

        Vector2 startVelocity = new Vector2(
            Mathf.Cos(angleRad) * launchForce,
            Mathf.Sin(angleRad) * launchForce
        );
        Vector3 startPos = ballSpawnPoint.position;

        for (int i = 0; i < steps; i++)
        {
            float t = i * 0.04f;
            float x = startPos.x + startVelocity.x * t;
            float y = startPos.y + startVelocity.y * t + 0.5f * g * t * t;
            trajectoryLine.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public float GetAngle() => launchAngle;
    public float GetForce() => launchForce;
    public bool HasBall() => hasBall;
}
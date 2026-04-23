using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI angleText;
    public TextMeshProUGUI forceText;
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI messageText;

    [Header("Referencias")]
    public PlayerController player;

    [Header("Sprites de personajes")]
    public Sprite[] characterSprites; 

    private int score = 0;
    private float messageTimer = 0f;
    private int currentCharacterIndex = 0;
    private SpriteRenderer playerRenderer;

    void Start()
    {
        playerRenderer = player.GetComponent<SpriteRenderer>();
        UpdateUI();

        if (instructionsText != null)
        {
            instructionsText.text =
                "A/D → Mover\n" +
                "W/S → Cambiar ángulo\n" +
                "Q/E → Cambiar fuerza\n" +
                "ESPACIO → Lanzar\n" +
                "R → Recuperar pelota";
        }
    }

    void Update()
    {
        UpdateUI();

        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0 && messageText != null)
                messageText.text = "";
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
        ShowMessage("¡CANASTA! +" + points);
        ChangeCharacter();
    }

    void ChangeCharacter()
    {
        if (characterSprites == null || characterSprites.Length == 0) return;

        currentCharacterIndex = (currentCharacterIndex + 1) % characterSprites.Length;
        if (playerRenderer != null)
            playerRenderer.sprite = characterSprites[currentCharacterIndex];
    }

    void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageTimer = 2f;
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score;

        if (player != null)
        {
            if (angleText != null)
                angleText.text = "Angulo: " + player.GetAngle().ToString("F0") + "°";
            if (forceText != null)
                forceText.text = "Fuerza: " + player.GetForce().ToString("F1");
        }
    }

    public int GetScore() => score;
}
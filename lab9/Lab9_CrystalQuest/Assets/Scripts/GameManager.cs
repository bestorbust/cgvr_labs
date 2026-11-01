
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text messageText;
    public TMP_Text winText;

    [Header("Game Settings")]
    public int totalCrystals = 5;

    private int collected = 0;
    private float elapsedTime = 0f;
    private float messageTime = 0f;
    private bool gameWon = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        winText.gameObject.SetActive(false);
        ShowMessage("Collect all 5 crystals!");
        UpdateScore();
    }

    void Update()
    {
        if (!gameWon)
        {
            elapsedTime += Time.deltaTime;
            timerText.text =
                "Time: " + elapsedTime.ToString("F1") + "s";
        }

        if (messageTime > 0f)
        {
            messageTime -= Time.deltaTime;

            if (messageTime <= 0f)
                messageText.text = "";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    public void CollectCrystal()
    {
        if (gameWon) return;

        collected++;
        UpdateScore();

        int remaining = totalCrystals - collected;

        if (remaining <= 0)
        {
            gameWon = true;

            winText.gameObject.SetActive(true);
            winText.text =
                "CONGRATULATIONS!\n\n" +
                "You collected all 5 crystals!\n" +
                "Time taken: " +
                elapsedTime.ToString("F1") +
                " seconds\n\nPress R to Play Again";

            messageText.text = "";
        }
        else if (remaining == 1)
        {
            ShowMessage("Amazing! Only 1 crystal left!");
        }
        else
        {
            ShowMessage(
                "Crystal collected! " +
                remaining + " remaining"
            );
        }
    }

    void ShowMessage(string message)
    {
        messageText.text = message;
        messageTime = 2.5f;
    }

    void UpdateScore()
    {
        scoreText.text =
            "Crystals: " + collected +
            " / " + totalCrystals;
    }
}

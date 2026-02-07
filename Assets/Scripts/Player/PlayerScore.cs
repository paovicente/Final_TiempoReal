using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScore : MonoBehaviour
{
    public static PlayerScore Instance;

    [Header("UI")]
    [SerializeField] private Text scoreText;

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignScoreText();

        if (scene.name == "Level1")
        {
            currentScore = 0;
        }

        UpdateScoreUI();
    }

    private void AssignScoreText()
    {
        if (scoreText != null) return;

        Text[] texts = FindObjectsByType<Text>(FindObjectsSortMode.None);

        foreach (Text t in texts)
        {
            if (t.gameObject.name == "ScoreText")
            {
                scoreText = t;
                break;
            }
        }
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
        else
        {
            Debug.Log("score text null");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


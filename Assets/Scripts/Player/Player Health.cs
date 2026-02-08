
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Image healthBarFill;

    [Header("Damage Settings")]
    public float damageCooldown = 2f;
    private float lastDamageTime = -10f;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level1")
        {
            currentHealth = maxHealth;
            PlayerPrefs.SetInt("PlayerHealth", currentHealth);
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.HasKey("PlayerHealth"))
                currentHealth = PlayerPrefs.GetInt("PlayerHealth");
            else
                currentHealth = maxHealth;
        }

        AssignHealthBar();
        UpdateHealthBar();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignHealthBar();
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = Time.time;

        UpdateHealthBar();

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();
    }

    private void AssignHealthBar()
    {
        if (healthBarFill != null) return;

        Image[] images = FindObjectsByType<Image>(FindObjectsSortMode.None);

        foreach (Image img in images)
        {
            if (img.name == "HealthBarFill")
            {
                healthBarFill = img;
                break;
            }
        }

        if (healthBarFill == null)
            Debug.LogWarning("HealthBarFill not found");
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        float fillAmount = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = fillAmount;

        healthBarFill.color =
            fillAmount > 0.6f ? Color.green :
            fillAmount > 0.3f ? Color.yellow :
            Color.red;
    }

    private void Die()
    {
        PlayerPrefs.SetString("GameResult", "Game Over");
        PlayerPrefs.DeleteKey("PlayerHealth");

        LevelManager.instance.LoadScene("ResultScene");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}


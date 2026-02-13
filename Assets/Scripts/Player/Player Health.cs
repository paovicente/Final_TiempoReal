using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public bool IsInvincible { get; private set; }

    public int maxHealth = 100;
    private int currentHealth;

    private Image healthBarFill;

    [Header("Damage Settings")]
    public float damageCooldown = 2f;
    private float lastDamageTime = -10f;

    private void OnEnable()
    {
        LevelManager.instance.SceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (LevelManager.instance != null)
            LevelManager.instance.SceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (CheatsManager.Instance != null)
            CheatsManager.Instance.RegisterPlayerHealth(this);

        string sceneName = LevelManager.instance.GetActiveScene();

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
        if (IsInvincible) //if the F2 cheat - player invincible is active, then the player doesnt take damage
            return;

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
       
    }

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }

}


using UnityEngine;
using UnityEngine.InputSystem;

public class CheatsManager : MonoBehaviour
{
    public static CheatsManager Instance;

    [Header("References")]
    [SerializeField] private InputActionReference cheatsInfoAction;
    [SerializeField] private InputActionReference invincibleAction;
    [SerializeField] private InputActionReference nextLevelAction;
    [SerializeField] private InputActionReference killEnemiesAction;
    [SerializeField] private InputActionReference speedAction;


    [SerializeField] private GameObject cheatsInfoPanel;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private float speedMultiplier = 2f;

    public bool InvincibleCheatActive { get; private set; }
    private bool speedCheatActive = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlayerHealth(PlayerHealth player)
    {
        playerHealth = player;

        if (playerHealth != null)
            playerHealth.SetInvincible(InvincibleCheatActive);
    }

    public void RegisterPlayerController(PlayerController player)
    {
        playerController = player;

        if (speedCheatActive)
            playerController.SetSpeedMultiplier(speedMultiplier);
    }

    private void Start()
    {
        cheatsInfoAction.action.performed += ActivateCheatsInfo;
        invincibleAction.action.performed += InvinciblePlayer;
        killEnemiesAction.action.performed += KillEnemies;
        speedAction.action.performed += FastestPlayer;
    }

    private void ActivateCheatsInfo(InputAction.CallbackContext context)
    {
        if (cheatsInfoPanel != null)
        {
            if (cheatsInfoPanel.activeSelf == true)
                cheatsInfoPanel.SetActive(false);
            else
                cheatsInfoPanel.SetActive(true);
        }
        else
        {
            Debug.Log("cheats Panel info NULL");
        }
    }

    private void InvinciblePlayer(InputAction.CallbackContext context)
    {
        InvincibleCheatActive = !InvincibleCheatActive;
        
        //starts in false, then when press F2 the player is invincible
        if (playerHealth != null)
            playerHealth.SetInvincible(InvincibleCheatActive);

        Debug.Log("Invincible: " +  playerHealth.IsInvincible);
    }

    private void KillEnemies(InputAction.CallbackContext context)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        Debug.Log("All enemies killed");
    }

    private void FastestPlayer(InputAction.CallbackContext context)
    {
        speedCheatActive = !speedCheatActive;

        if (playerController != null)
        {
            if (speedCheatActive)
                playerController.SetSpeedMultiplier(speedMultiplier);
            else
                playerController.SetSpeedMultiplier(1f);
        }

    }
}

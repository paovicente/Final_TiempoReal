using UnityEngine;

public class PlayerPauseHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerShoot playerShoot;
    [SerializeField] private Rigidbody2D playerRb;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (playerController == null) playerController = GetComponent<PlayerController>();
        if (playerShoot == null) playerShoot = GetComponent<PlayerShoot>();
    }

    public void PausePlayer()
    {
        if (animator != null)
            animator.speed = 0f;

        if (audioSource != null)
            audioSource.Pause();

        if (playerController != null)
            playerController.enabled = false;

        if (playerShoot != null)
            playerShoot.enabled = false;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;
    }

    public void ResumePlayer()
    {
        if (animator != null)
            animator.speed = 1f;

        if (audioSource != null)
            audioSource.UnPause();

        if (playerController != null)
            playerController.enabled = true;

        if (playerShoot != null)
            playerShoot.enabled = true;

        playerRb.simulated = true;
    }
}


using UnityEngine;
using UnityEngine.Audio;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioSource healSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                
                if (healSound != null && healSource != null)
                    healSource.PlayOneShot(healSound);
            }

            Destroy(gameObject);
        }

    }
}

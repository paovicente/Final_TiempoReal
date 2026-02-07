using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public int pointsValue = 10;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioSource audioSource;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerScore.Instance != null)
            {
                PlayerScore.Instance.AddPoints(pointsValue);
            }
            else
            {
                Debug.Log("Player score instance NULL");
            }

            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            Destroy(gameObject);
        }
    }

}

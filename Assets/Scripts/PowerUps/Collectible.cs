using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public int pointsValue = 10;          
    public AudioClip collectSound;         
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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

                gameObject.SetActive(false);
 
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
    }

}

using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private PlayerController player;

    [Header("Audio References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip shootClip;
    
    [Header("Audio Configurations")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float shootVolume = 0.1f;

    private bool wasRunning = false;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleJumpSound();
        HandleRunSound();
    }

    private void HandleJumpSound()
    {

        if (player.IsJumping() && jumpClip != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(jumpClip,0.04f);
        }
    }

    private void HandleRunSound()
    {
        bool isRunning = player.IsRunning();

        if (isRunning && !wasRunning)
        {
            audioSource.clip = runClip;
            audioSource.loop = true;
            audioSource.volume = 1f;
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }
        else if (!isRunning && wasRunning)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.volume = 1f;
        }

        wasRunning = isRunning;
    }

    public void PlayShootSound()
    {
        if (shootClip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(shootClip, shootVolume);
    }

}

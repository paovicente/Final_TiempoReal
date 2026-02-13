using UnityEngine;
using System.Collections;

public class TurretEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHits = 3;
    [SerializeField] private int damageToPlayer = 10;
    [SerializeField] private float shootInterval = 1.5f;

    [Header("Detection & Shooting")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab; 
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;

    [Header("References")]
    [SerializeField] private GameObject player;

    private int currentHits = 0;
    private float lastShootTime = 0f;
    private bool isDead = false;
    
    private Color originalColor;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    private void Update()
    {

        if (isDead || player == null) return;

        bool playerOnRight = player.transform.position.x > transform.position.x;
        spriteRenderer.flipX = !playerOnRight;

        //shoot if player is in range and the cooldown is completed
        float distance = Vector2.Distance(transform.position, player.transform.position);
        
        if (distance <= detectionRange && Time.time >= lastShootTime + shootInterval)
        {
            ShootAtPlayer();
            lastShootTime = Time.time;
        }
    }

    private void ShootAtPlayer()
    {
        if (firePoint == null || bulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletEnemy be = bullet.GetComponent<BulletEnemy>();
        if (be != null)
        {
            be.damage = damageToPlayer;
            be.speed = bulletSpeed;
            be.Initialize(player.transform, GetComponent<Collider2D>());
        }

        Vector2 dir = (player.transform.position - firePoint.position).normalized;
        Vector3 scale = bullet.transform.localScale;
        scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
        bullet.transform.localScale = scale;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Bullet")) //player bullets
        {
            TakeHit();
            collision.gameObject.SetActive(false);
        }
    }

    private void TakeHit()
    {
        currentHits++;
        StartCoroutine(BlinkFeedback());

        if (currentHits >= maxHits)
            Die();
    }

    private IEnumerator BlinkFeedback()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject, 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damageToPlayer);
        }
    }
}

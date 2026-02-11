using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class AdvancedEnemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int damageToPlayer = 10;

    [Header("Patrol")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float leftLimit = -5f;
    [SerializeField] private float rightLimit = 5f;
    private bool movingRight = true;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpInterval = 15f;
    private float lastJumpTime;

    [Header("Shooting")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float shootCooldown = 1f;
    private float lastShootTime;

    [Header("Burst")]
    [SerializeField] private float burstInterval = 8f;
    [SerializeField] private int burstBullets = 12;
    private float lastBurstTime;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;
    private Color originalColor;

    [SerializeField] private BoxCollider2D boxCollider;

    private Rigidbody2D rb;
    private GameObject player;
    private int currentHealth;
    private bool isDead = false;

    [Header("Death Spawn")]
    [SerializeField] private GameObject onDeathActivate;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;

        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        lastJumpTime = Time.time;
        lastShootTime = Time.time;
        lastBurstTime = Time.time;
    }

    private void Update()
    {
        if (isDead) return;

        Patrol();
        HandleJump();
        HandleShooting();
        HandleBurst();
    }

    private void Patrol()
    {
        float dir = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (movingRight && transform.position.x >= rightLimit)
            FlipDirection(false);
        else if (!movingRight && transform.position.x <= leftLimit)
            FlipDirection(true);
    }

    private void FlipDirection(bool toRight)
    {
        movingRight = toRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = !movingRight;
    }

    private void HandleJump()
    {
        if (Time.time >= lastJumpTime + jumpInterval && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
        }
    }

    private bool IsGrounded()
    {
        if (boxCollider == null) return false;

        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            extraHeight,
            LayerMask.GetMask("Ground")
        );

        return hit.collider != null;
    }

    private void HandleShooting()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= detectionRange && Time.time >= lastShootTime + shootCooldown)
        {
            ShootAtPlayer();
            lastShootTime = Time.time;
        }
    }

    private void ShootAtPlayer()
    {
        if (firePoint == null || bulletPrefab == null) return;

        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.gravityScale = 0;
            rbBullet.linearVelocity = direction * bulletSpeed;
        }

        BulletEnemy bd = bullet.GetComponent<BulletEnemy>();
        if (bd != null) bd.damage = damageToPlayer;
    }

    private void HandleBurst()
    {
        if (Time.time >= lastBurstTime + burstInterval)
        {
            StartCoroutine(BurstShoot());
            lastBurstTime = Time.time;
        }
    }

    private IEnumerator BurstShoot()
    {
        if (firePoint == null || bulletPrefab == null) yield break;

        float angleStep = 360f / burstBullets;
        float angle = 0f;

        for (int i = 0; i < burstBullets; i++)
        {
            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 dir = new Vector2(dirX, dirY);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            if (rbBullet != null)
            {
                rbBullet.gravityScale = 0;
                rbBullet.linearVelocity = dir * bulletSpeed;
            }

            BulletEnemy bd = bullet.GetComponent<BulletEnemy>();
            if (bd != null) bd.damage = damageToPlayer;

            angle += angleStep;
        }

        yield return null;
    }

    public void TakeHit(int damageAmount = 1)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        StartCoroutine(BlinkFeedback());

        if (currentHealth <= 0) Die();
    }

    private IEnumerator BlinkFeedback()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        isDead = true;

        if (onDeathActivate != null)
            onDeathActivate.SetActive(true);

        Destroy(gameObject, 0.15f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damageToPlayer);

            FlipDirection(!movingRight);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Bullet"))
        {
            TakeHit();
            collision.gameObject.SetActive(false);
        }
    }
}

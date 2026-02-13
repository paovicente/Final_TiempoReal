using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 14f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;
    private float lifeTimer;
    private Vector2 pendingDirection;
    private bool hasPendingDirection = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D bulletCollider = GetComponent<Collider2D>();
            if (playerCollider != null && bulletCollider != null)
                Physics2D.IgnoreCollision(bulletCollider, playerCollider);
        }
    }


    private void OnEnable()
    {
        lifeTimer = 0f;

        if (hasPendingDirection)
        {
            rb.linearVelocity = pendingDirection * speed;
            hasPendingDirection = false;
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Calls to fire the bullet from outside. Works whether the bullet is already active (apply speed immediately)
    /// as if it is inactive (save the direction and apply it in OnEnable).
    /// </summary>
    public void Fire(Vector2 direction)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }

        if (gameObject.activeInHierarchy)
        {
            rb.linearVelocity = direction * speed;
            hasPendingDirection = false;
        }
        else
        {
            pendingDirection = direction;
            hasPendingDirection = true;
        }
    }
}

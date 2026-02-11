using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BulletEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 10;
    public float speed = 10f;

    private Rigidbody2D rb;

    private Collider2D ignoreCollider;

    public void Initialize(Transform target, Collider2D turretCollider)
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        //calculate direction to target when shooting
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        //ignore collision with turret
        Collider2D bulletCollider = GetComponent<Collider2D>();
        if (turretCollider != null && bulletCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, turretCollider);
        }

        ignoreCollider = turretCollider;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);

            gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Assign in inspector")]
    public RuntimeAnimatorController armedAnimator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerShoot shooter = collision.GetComponent<PlayerShoot>();

        if (shooter != null)
            shooter.EquipWeapon(armedAnimator);

        Destroy(gameObject);
    }
}

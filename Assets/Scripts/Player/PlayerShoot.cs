using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private bool aimAtMouse = false;

    [Header("Recoil (Opcional)")]
    [SerializeField] private float recoilForce = 0f;

    [Header("Power-up")]
    [SerializeField] private float powerupFireRateMultiplier = 0.2f; // 20% del fireRate original
    private bool isPowerupActive = false;
    private float powerupEndTime = 0f;
    private float originalFireRate;

    [Header("Power-up Visual Feedback")]
    [SerializeField] private SpriteRenderer playerSprite; // asignar el sprite del jugador
    [SerializeField] private Color powerupColor = Color.yellow;
    [SerializeField] private float blinkInterval = 0.2f;

    private Coroutine blinkRoutine;
    private Color originalColor;

    private float nextFireTime;

    private void Start()
    {
        originalColor = playerSprite.color;
        originalFireRate = fireRate;
    }

    private void Update()
    {
        FlipFirePoint();

        // Si el power-up está activo y pasó el tiempo, lo desactivamos
        if (isPowerupActive && Time.time >= powerupEndTime)
        {
            fireRate = originalFireRate;
            isPowerupActive = false;
        }
    }


    private void FlipFirePoint()
    {
        if (!firePoint || !spriteRenderer) return;

        if (spriteRenderer.flipX)
        {
            firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, 0);
            firePoint.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, 0);
            firePoint.localRotation = Quaternion.identity;
        }
    }

    private void OnEnable()
    {
        if (shootAction != null)
        {
            shootAction.action.performed += OnShootPerformed;
            shootAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (shootAction != null)
        {
            shootAction.action.performed -= OnShootPerformed;
            shootAction.action.Disable();
        }
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        GameObject bullet = BulletPoolPlayer.Instance.GetBullet();
        Debug.Log("instancia  nula?" + BulletPoolPlayer.Instance);
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;

        // Dirección 
        Vector2 direction;
        if (aimAtMouse)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            direction = (mousePos - (Vector2)firePoint.position).normalized;
        }
        else
        {
            direction = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }

        // 🔥 ROTACIÓN CORRECTA SEGÚN DIRECCIÓN 
        if (direction.x < 0) bullet.transform.rotation = Quaternion.Euler(0, 180, 0); // mirando izquierda 
        else bullet.transform.rotation = Quaternion.identity; // mirando derecha 

        // Velocidad de la bala 
        Bullet b = bullet.GetComponent<Bullet>();
        b.Fire(direction);

        // Recoil opcional 
        if (recoilForce > 0f && playerRb != null) playerRb.AddForce(-direction * recoilForce, ForceMode2D.Impulse); bullet.SetActive(true);
    }

    public void ActivateFireRatePowerup(float duration)
    {
        fireRate = originalFireRate * powerupFireRateMultiplier;
        isPowerupActive = true;
        powerupEndTime = Time.time + duration;

        // Iniciar corutina de blink
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(PowerupBlinkRoutine(duration));
    }

    private System.Collections.IEnumerator PowerupBlinkRoutine(float duration)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            playerSprite.color = powerupColor;
            yield return new WaitForSeconds(blinkInterval);
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Asegurar que al final quede el color normal
        playerSprite.color = originalColor;
    }


}

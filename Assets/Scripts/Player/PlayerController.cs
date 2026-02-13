using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("Inputs")]
    private Vector2 moveInput;

    [Header("Player variables")]
    [SerializeField] private float playerSpeed = 2f;
    private float speedMultiplier = 1f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool isGrounded;
    private float originalGravity;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    private bool isDashing;
    private float lastDashTime = -10f;

    private void Start()
    {
        if (CheatsManager.Instance != null)
            CheatsManager.Instance.RegisterPlayerController(this);

        originalGravity = playerRigidbody.gravityScale;
        
        moveAction.action.started += HandleMoveInput;
        moveAction.action.performed += HandleMoveInput;
        moveAction.action.canceled += HandleMoveInput;

        //jumpAction.action.started += HandleJumpInput;
        jumpAction.action.performed += HandleJumpInput;
        jumpAction.action.canceled += HandleJumpInput;

        dashAction.action.performed += HandleDashInput;

    }

    private void Update()
    {
        CheckGround();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
            MovePlayer();
    }

    private void HandleMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void HandleJumpInput(InputAction.CallbackContext context) 
    {
        if (!isGrounded || isDashing) return;

        playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, jumpForce);
    }

    private void HandleDashInput(InputAction.CallbackContext context)
    {
        TryDash();
    }

    private void MovePlayer()
    {
        playerRigidbody.linearVelocity = new Vector2(moveInput.x * playerSpeed * speedMultiplier, playerRigidbody.linearVelocity.y); //the player moves in x according to the input and maintains velocity in y 
        
        if (moveInput.x != 0)
            sprite.flipX = moveInput.x < 0;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void TryDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        if (isDashing) return;

        Vector2 dashDirection = moveInput;

        //if no input, dash in sprite direction
        if (dashDirection == Vector2.zero)
            dashDirection = sprite.flipX ? Vector2.left : Vector2.right;

        StartCoroutine(DashRoutine(dashDirection.normalized));
    }

    private IEnumerator DashRoutine(Vector2 direction)
    {
        isDashing = true;
        lastDashTime = Time.time;

        playerRigidbody.gravityScale = 0;
        playerRigidbody.linearVelocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        playerRigidbody.gravityScale = originalGravity;
        isDashing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            Debug.Log("You found the door.");
            PlayerPrefs.SetString("GameResult", "You Win!");
            LevelManager.instance.LoadScene("ResultScene");
        }
    }

    // --------------------------------------------------
    // ANIMATOR
    // --------------------------------------------------
    private void UpdateAnimator()
    {
        bool running = Mathf.Abs(playerRigidbody.linearVelocity.x) > 0.1f;
        bool jumping = playerRigidbody.linearVelocity.y > 0.1f && !isGrounded;
        bool falling = playerRigidbody.linearVelocity.y < -0.1f && !isGrounded;
        bool idle = !running && isGrounded && !isDashing;

        animator.SetBool("isIdle", idle);
        animator.SetBool("isRunning", running);
        animator.SetBool("isJumping", jumping);
        animator.SetBool("isFalling", falling);
        animator.SetBool("isDashing", isDashing);
    }

    public bool IsRunning()
    {
        return Mathf.Abs(playerRigidbody.linearVelocity.x) > 0.1f && isGrounded && !isDashing;
    }

    public bool IsJumping()
    {
        return playerRigidbody.linearVelocity.y > 0.1f && !isGrounded;
    }


    // --------------------------------------------------
    // GIZMOS
    // --------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

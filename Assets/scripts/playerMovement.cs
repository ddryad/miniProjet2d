using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    private bool jumpRequested;

    // Cache the parameter name for better performance
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        ReadInput();
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        // Maintain gravity/vertical velocity while setting horizontal speed
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
        {
            horizontalInput = 0f;
            return;
        }

        horizontalInput = 0f;

        // Left / Right inputs
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            horizontalInput = -1f;

        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            horizontalInput = 1f;

        // Jump input using wasPressedThisFrame (equivalent to GetButtonDown)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void UpdateVisuals()
    {
        // Toggle walking animation
        bool isWalking = Mathf.Abs(horizontalInput) > 0.01f;
        anim.SetBool(IsWalkingHash, isWalking);

        // Flip sprite facing direction
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningCharacter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float sideSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpCooldown = 0.2f;

    [Header("Components")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -25f;   // gravity lebih kuat agar tidak melayang
    private float jumpTimer;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Ground Check otomatis
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = check.transform;
        }
    }

    void Update()
    {
        GroundCheckLogic();
        HandleMovement();
        HandleJump();
        ApplyGravity();

        controller.Move(velocity * Time.deltaTime);
    }

    void GroundCheckLogic()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -5f;     // press ke tanah supaya tidak ngambang
        }

        if (jumpTimer > 0)
            jumpTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;

        // Jalankan otomatis ke depan
        Vector3 move = (transform.forward * runSpeed) +
                       (transform.right * horizontal * sideSpeed);

        // Masukkan ke velocity, hanya XZ
        velocity.x = move.x;
        velocity.z = move.z;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpTimer <= 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpTimer = jumpCooldown;
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, 0.3f);
    }
}
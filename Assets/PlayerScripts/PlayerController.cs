using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Movement Settings")]
    public float walkSpeed = 2.8f;
    public float jumpSpeed = 4;
    public float jumpCutMultiplier = 0.5f;
    public float coolDown = 0.2f;
    private float cDwnCounter;
    public bool isGrounded;
    public bool facingRight;

    private bool isJumping;
    private float xInput;
    private float xVelocity;
    private float yVelocity;

    [Header("References")]
    public Rigidbody2D body;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;

    [Header("Camera Logic")]
    public Camera playerCamera;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float cameraSmoothing = 0.20f;
    private Vector3 velocity = Vector3.zero;



    void Start() {
        if (body == null)
            body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        CheckGrounded();
    }

    void Update() {
        xVelocity = body.linearVelocity.x;
        yVelocity = body.linearVelocity.y;

        //Clock starts the min the collider is off the edge
        if (isGrounded) {
            cDwnCounter = coolDown;
        } else {
            cDwnCounter -= Time.deltaTime;
        }

        HandleMovement();
        HandleJump();
    }

    void LateUpdate() {
        Vector3 targetPosition = transform.position + cameraOffset;

        playerCamera.transform.position = Vector3.SmoothDamp(
        playerCamera.transform.position,
        targetPosition,
        ref velocity,
        cameraSmoothing);
    }

    void HandleMovement() {
        xInput = Input.GetAxisRaw("Horizontal");
        body.linearVelocity = new Vector2(walkSpeed * xInput, yVelocity);
    }

    void HandleJump() {
        if (Input.GetButtonDown("Jump") && (isGrounded || cDwnCounter > 0)) {
            isJumping = true;
            cDwnCounter = 0;
            body.linearVelocity = new Vector2(xVelocity, jumpSpeed);
        }

        if (Input.GetButtonUp("Jump") && isJumping && yVelocity > 0) {
            body.linearVelocity = new Vector2(xVelocity, yVelocity * jumpCutMultiplier);
            isJumping = false;
        }

        if (isGrounded && body.linearVelocity.y <= 0) {
            isJumping = false;
        }
    }

    void CheckGrounded() {
        isGrounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }
}

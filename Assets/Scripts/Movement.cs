using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
  [Header("Move")]
  [SerializeField] private float runSpeed = 8f;
  private int _horizontalFlag = 0;

  [Header("Jump")]
  [SerializeField] private float jumpSpeed = 20f;
  [SerializeField] private float maxJumpHeight = 3.5f;
  [SerializeField] private LayerMask platformLayerMask;
  [SerializeField] private Transform groundCheckTransform;
  private bool _justJumped = false;
  private const float _groundCheckRadius = 0.2f;
  private bool _isGrounded = false;

  [Header("Animation")]
  [SerializeField] private Animator animator;

  [Header("Physics")]
  private Rigidbody2D _rb2D;

  private void Awake()
  {
    _rb2D = GetComponent<Rigidbody2D>();

    _rb2D.gravityScale = (((jumpSpeed * jumpSpeed) / maxJumpHeight) / 2) / Physics2D.gravity.magnitude;
  }

  private void FixedUpdate()
  {
    float moveVelocity = runSpeed * _horizontalFlag;
    float jumpVelocity = _rb2D.velocity.y;
    if (_justJumped)
    {
      jumpVelocity = jumpSpeed;
      _justJumped = false;
    }

    _rb2D.velocity = new Vector2(moveVelocity, jumpVelocity);
  }

  private void Update()
  {
    animator.SetFloat("Speed", Mathf.Abs(_horizontalFlag));
    if (Input.GetKey(KeyCode.A))
    {
      transform.rotation = Quaternion.Euler(0, 180f, 0);
      _horizontalFlag = -1;
    }
    else if (Input.GetKey(KeyCode.D))
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
      _horizontalFlag = 1;
    }
    else
    {
      _horizontalFlag = 0;
    }
    GroundCheck();
    if (_isGrounded && Input.GetButtonDown("Jump"))
    {
      _justJumped = true;
      animator.SetBool("IsJumping", true);
    }
  }

  private void GroundCheck()
  {
    bool wasGrounded = _isGrounded;
    _isGrounded = false;

    Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckTransform.position, _groundCheckRadius, platformLayerMask);
    for (int i = 0; i < colliders.Length; i++)
    {
      if (colliders[i].gameObject != gameObject)
      {
        _isGrounded = true;
        if (!wasGrounded)
        {
          animator.SetBool("IsJumping", false);
        }
      }
    }

  }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public InputActionAsset inputActions;

    public float horizontalInput;
    public float verticalInput;

    public float horizontalSpeed;
    public float verticalSpeed;

    public float acceleration = 100f;
    public float moveSpeed = 5f;

    public int jumpForce = 600;

    [SerializeField]
    Rigidbody2D playerRB = null;
    [SerializeField]
    Animator animator = null;

    public Vector2 boxSize;
    public float raycastDistance;
    public LayerMask groundLayer;
    public bool inputEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool IsStanding()
    {

        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, raycastDistance, groundLayer))
        {
            
            if(animator != null) animator.SetBool("IsJumping", false);
            return true;
        }
        else
        {

            if (animator != null) animator.SetBool("IsJumping", true);
            return false;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * raycastDistance, boxSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") )
        {
            if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, raycastDistance, groundLayer)) ;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        IsStanding();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        
        if (IsStanding())
        {
            playerRB.linearVelocity = new Vector2(horizontalSpeed, 0);
            playerRB.AddForce(new Vector2(0, jumpForce));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!inputEnabled) return;

        verticalSpeed = playerRB.linearVelocityY;
        horizontalSpeed = playerRB.linearVelocityX;

        if (Mathf.Abs(playerRB.linearVelocityX) < moveSpeed) playerRB.AddForce(new Vector2(horizontalInput * acceleration, 0));
    }
}

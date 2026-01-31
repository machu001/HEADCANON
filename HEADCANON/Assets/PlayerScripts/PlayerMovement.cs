using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Values")]
    public float horizontalInput;
    public float verticalInput;

    public float horizontalSpeed;
    public float verticalSpeed;

    public float acceleration = 100f;
    public float moveSpeedCap = 5f;

    public int jumpForce = 600;

    [Header("Component References")]
    
    public Rigidbody2D playerRB = null;
    public PlayerSliding slideScript = null;
    [SerializeField]
    Animator animator = null;

    [Header ("Ground Check")]
    public Vector2 boxSize;
    public float raycastDistance;
    public LayerMask groundLayer;
    public bool isStanding;
    [Header ("Input Handling")]
    public bool inputEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        slideScript = GetComponent<PlayerSliding>();
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
        IsStanding();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }



    public void Jump(InputAction.CallbackContext context)
    {
        
        if (IsStanding() && context.started)
        {
            verticalSpeed = 0;
            playerRB.linearVelocityY = 0;
            playerRB.AddForce(Vector2.up * jumpForce);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!inputEnabled) return;
        isStanding = IsStanding();

        

        verticalSpeed = playerRB.linearVelocityY;
        horizontalSpeed = playerRB.linearVelocityX;










        if (slideScript.sliding) return;
        //ruch lewo/prawo, ograniczany sztywno do limitu jeœli postaæ jest na ziemi
        if (horizontalSpeed < moveSpeedCap && horizontalInput > 0.5f)
        {
            float deltaSpeed = playerRB.mass * horizontalSpeed * Time.fixedDeltaTime;

            if(horizontalSpeed + deltaSpeed > moveSpeedCap)
            {
                float speedToAdd = moveSpeedCap - horizontalSpeed;
                playerRB.AddForce(new Vector2(horizontalInput * acceleration * speedToAdd, 0));
            }
            else playerRB.AddForce(new Vector2(horizontalInput * acceleration, 0));
        }
        else if(horizontalSpeed > -moveSpeedCap && horizontalInput < -0.5f)
        {
            
            float deltaSpeed = playerRB.mass * horizontalSpeed * Time.fixedDeltaTime;
            Debug.Log(deltaSpeed);
            Debug.Log(horizontalSpeed + deltaSpeed);
            if(horizontalSpeed + deltaSpeed < -moveSpeedCap)
            {
                float speedToAdd = Mathf.Abs(-moveSpeedCap - horizontalSpeed);
                playerRB.AddForce(new Vector2(horizontalInput * acceleration * speedToAdd, 0));
            }
            else playerRB.AddForce(new Vector2(horizontalInput * acceleration, 0));
        }
        
        
    }
}

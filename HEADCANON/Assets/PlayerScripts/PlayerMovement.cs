using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Values")]
    public float orientation = 1;

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
    public Vector2 gcBoxSize;
    public float gcRaycastDistance;
    public LayerMask groundLayer;
    public bool isStanding;

    [Header ("Input Handling")]
    public bool inputEnabled = true;

    [Header("Wall Check")]
    public Transform wcPos;
    public Vector2 wcBoxSize;
    public float wcRaycastDistance;
    public LayerMask wallLayer;
    public bool isNearWall;

    [Header("Wall Movement")]
    public float wallSlideSpeed = 1;
    public bool isWallSliding;

    public bool isWallJumping;
    float wallJumpDirection;
    public float wallJumpTime;
    public float wallJumpTimer;
    public Vector2 wallJumpPower;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        slideScript = GetComponent<PlayerSliding>();
    }

    public bool IsStanding()
    {

        if(Physics2D.BoxCast(transform.position, gcBoxSize, 0, -transform.up, gcRaycastDistance, groundLayer))
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position-transform.up * gcRaycastDistance, gcBoxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wcPos.position-transform.up * wcRaycastDistance, wcBoxSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<Vector2>().x;
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wcPos.position, wcBoxSize, 0, wallLayer);
    }

    private void ProcessWallSlide()
    {
        if (isWallJumping) return;
        if(!IsStanding() && WallCheck() && horizontalInput != 0)
        {
            isWallSliding = true;
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocityX, Mathf.Max(playerRB.linearVelocityY, -wallSlideSpeed)); // fall rate cap
        }
        else
        {
            isWallSliding = false;
        }

    }

    private void ProcessWallJump()
    {
        wallJumpDirection = -orientation;
        if (WallCheck() && !isWallJumping) 
        {
            ProcessOrientation();
            isWallJumping = false;
            
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }

    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (IsStanding() && context.performed)
        {
            if (playerRB.linearVelocityY < 0) playerRB.linearVelocityY = 0;
            playerRB.AddForce(Vector2.up * jumpForce);
        }
        else if(context.performed && WallCheck())
        {
            isWallSliding = false;
            isWallJumping = true;
            if (playerRB.linearVelocityY < 0) playerRB.linearVelocityY = 0;
            playerRB.linearVelocityX = 0;
            Vector2 wallJumpVector = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            playerRB.AddForce(wallJumpVector);
            wallJumpTimer = 0;



            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
         
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ProcessOrientation()
    {
        if (horizontalSpeed > 0.03f) orientation = 1;
        if (horizontalSpeed < -0.03f) orientation = -1;
        float wcPosX = Mathf.Abs(wcPos.localPosition.x);
        if (Mathf.Abs(horizontalSpeed) > 0.1f)
        {
            if(orientation > 0)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void ProcessMovement()
    {
        if (horizontalSpeed < moveSpeedCap && horizontalInput > 0.5f) // ruch w prawo
        {
            
            float deltaSpeed = playerRB.mass * horizontalSpeed * Time.fixedDeltaTime;

            if (horizontalSpeed + deltaSpeed > moveSpeedCap * 0.75f)
            {
                float speedToAdd = moveSpeedCap - horizontalSpeed;
                if (isStanding) speedToAdd *= slideScript.playerFeetDefaultMaterial.friction;
                playerRB.AddForce(new Vector2(horizontalInput * acceleration * speedToAdd, 0));
            }
            else playerRB.AddForce(new Vector2(horizontalInput * acceleration, 0));
        }
        else if (horizontalSpeed > -moveSpeedCap && horizontalInput < -0.5f) // ruch w lewo
        {
            
            float deltaSpeed = playerRB.mass * horizontalSpeed * Time.fixedDeltaTime;

            if (horizontalSpeed + deltaSpeed < -moveSpeedCap * 0.75f)
            {
                float speedToAdd = Mathf.Abs(-moveSpeedCap - horizontalSpeed);
                if (isStanding) speedToAdd *= slideScript.playerFeetDefaultMaterial.friction;
                playerRB.AddForce(new Vector2(horizontalInput * acceleration * speedToAdd, 0));
            }
            else playerRB.AddForce(new Vector2(horizontalInput * acceleration, 0));
        }
    }
    private void FixedUpdate()
    {
        if (!inputEnabled) return;
        isStanding = IsStanding();
        ProcessOrientation();
        if (!slideScript.sliding && !isWallJumping) ProcessWallSlide();
        ProcessWallJump();
        ProcessOrientation();
        
        if (!slideScript.sliding && !isWallJumping) ProcessMovement();

        verticalSpeed = playerRB.linearVelocityY;
        horizontalSpeed = playerRB.linearVelocityX;
    }
}

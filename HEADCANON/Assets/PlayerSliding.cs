using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")]
    
    private Rigidbody2D playerRB;
    public PlayerMovement pm;
    public BoxCollider2D playerFeet;
    
    public PhysicsMaterial2D playerFeetSlideMaterial;
    public PhysicsMaterial2D playerFeetDefaultMaterial;

    [Header("Sliding Values")]
    public float maxSlideTime;
    public float minimalSlideSpeed = 2;

    public float slideYscale;
    private float startYscale;

    public bool sliding;
    public bool slideInput = false;

    [Header ("Head Check")]
    public Transform hcPos;
    public Vector2 hcBoxSize;
    public float hcRaycastDistance;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hcPos.position - transform.up * hcRaycastDistance, hcBoxSize);
    }

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        startYscale = playerRB.gameObject.transform.localScale.y;
    }

    private void FixedUpdate()
    {
        if(sliding) SlideMovement();
        if (!slideInput)
        {
            StopSlide();
        }
        
    }

    public void Slide(InputAction.CallbackContext context)
    {
        Debug.Log(pm.horizontalInput);
        if(context.performed)
        {
            slideInput = true;
            sliding = true;
            playerRB.gameObject.transform.localScale = new Vector3(playerRB.gameObject.transform.localScale.x, slideYscale, playerRB.gameObject.transform.localScale.z);
            pm.gcRaycastDistance = 0.565f;
            hcRaycastDistance = -0.53f;
            if (pm.isStanding) playerRB.AddForce(Vector2.down * 10, ForceMode2D.Impulse);
            StartSlide();
            playerFeet.sharedMaterial = playerFeetSlideMaterial;
        }

        if (context.canceled) slideInput = false;

        if(context.canceled && sliding)
        {
            Debug.Log("Slide canceled");
            StopSlide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
               
    }

    void StartSlide()
    {
        sliding = true;
        if(Mathf.Abs(playerRB.linearVelocityX) < 4)
        {
            Vector2 slideDir = Vector2.right * pm.orientation;
            playerRB.linearVelocityX = slideDir.x * 4; 
        }

    }

    void SlideMovement()
    {
        Vector2 slideDir = Vector2.right * pm.orientation;
        Debug.Log(Mathf.Abs(playerRB.linearVelocityX) + " " + Mathf.Abs(slideDir.x * minimalSlideSpeed));
        if (Mathf.Abs(playerRB.linearVelocityX) < Mathf.Abs(slideDir.x * minimalSlideSpeed)) playerRB.linearVelocityX = slideDir.x * minimalSlideSpeed; 
        
    }

    bool HeadCheck()
    {
        return Physics2D.BoxCast(transform.position, hcBoxSize, 0, -transform.up, hcRaycastDistance, pm.groundLayer);
    }

    void StopSlide()
    {
        if (HeadCheck())
        {
            return;
        }
        if (sliding == false) return;
        sliding = false;
        playerFeet.sharedMaterial = playerFeetDefaultMaterial;
        playerRB.gameObject.transform.localScale = new Vector3(playerRB.gameObject.transform.localScale.x, startYscale, playerRB.gameObject.transform.localScale.z);
        pm.gcRaycastDistance = 1.13f;
        hcRaycastDistance = -0.03f;
    }
}

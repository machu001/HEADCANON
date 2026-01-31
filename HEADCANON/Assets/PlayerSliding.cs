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
    public float slideForce;

    public float slideYscale;
    private float startYscale;

    public bool sliding;

    [Header("Input")]
    public KeyCode slideKey;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        startYscale = playerRB.gameObject.transform.localScale.y;
    }

    private void FixedUpdate()
    {
        if(sliding) SlideMovement();
    }

    public void Slide(InputAction.CallbackContext context)
    {
        Debug.Log(pm.horizontalInput);
        if(context.started && pm.horizontalInput != 0)
        {
            StartSlide();
            playerRB.gameObject.transform.localScale = new Vector3(playerRB.gameObject.transform.localScale.x, slideYscale, playerRB.gameObject.transform.localScale.z);
            if (pm.isStanding) playerRB.AddForce(Vector2.down * 50, ForceMode2D.Impulse);
            playerFeet.sharedMaterial = playerFeetSlideMaterial;
        }

        if(context.canceled && sliding)
        {
            StopSlide();
            playerRB.gameObject.transform.localScale = new Vector3(playerRB.gameObject.transform.localScale.x, startYscale, playerRB.gameObject.transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
               
    }

    void StartSlide()
    {
        sliding = true;
        
    }

    void SlideMovement()
    {
        Vector2 slideDir = Vector2.right * pm.horizontalInput;

        if (Mathf.Abs(pm.horizontalSpeed) < 1f) StopSlide();
        
    }

    void StopSlide()
    {
        sliding = false;
        playerFeet.sharedMaterial = playerFeetDefaultMaterial;
        playerRB.gameObject.transform.localScale = new Vector3(playerRB.gameObject.transform.localScale.x, startYscale, playerRB.gameObject.transform.localScale.z);
    }
}

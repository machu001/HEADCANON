using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // debug
    [SerializeField] GameObject debugCanvas;
    private TextMeshProUGUI velocityDisplay;
    private TextMeshProUGUI groundContactDisplay;
    private bool currentGroundContactInfo = false;
    private TextMeshProUGUI airbourneJumpsDisplay;

    [SerializeField] Rigidbody player = null;
    [SerializeField] private GameObject weaponContainer;
    public Animator animator = null;

    public float deathTimer = 2f;
    public bool playerAlive = true;
    public bool inputEnabled = true;
    private SpriteRenderer spriteRenderer;

    // rorate to mouse
    private Camera m_Camera;
    private Vector3 mousePos;

    // jump handling
    public bool isAirbourne = true;
    // 0 - no airburne jumps, 1 - double jump...
    [SerializeField] private byte airbourneJumpsCount = 3; 
    [SerializeField]float airbourneJumpCooldown = .3f;
    float nextTimeAirbourneJump = 0f;
    private byte airbourneJumpsPerformed;

    // rocket jumps
    [SerializeField] float rocketJumpForce = 10f;

    void Start()
    {
        velocityDisplay = debugCanvas.transform.Find("VelocityDisplay").GetComponent<TextMeshProUGUI>();
        groundContactDisplay = debugCanvas.transform.Find("GroundContactDisplay").GetComponent<TextMeshProUGUI>();
        airbourneJumpsDisplay = debugCanvas.transform.Find("AirbourneJumpsDisplay").GetComponent<TextMeshProUGUI>();

        if (!weaponContainer)
        {
            weaponContainer = GameObject.Find("WeaponContainer");
        }

        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        float verticalSpeed = player.linearVelocity.y;
        float horizontalSpeed = player.linearVelocity.x;

        // R/L input
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontal) > 0.3f)
        {
            float maxSpeed = 8f;
            if ((horizontal < 0 && player.linearVelocity.x > -maxSpeed) || (horizontal > 0 && player.linearVelocity.x < maxSpeed))
            {
                player.AddForce(Vector2.right * horizontal * 50f);
            }
        }

        // player flip
        if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        // jump input
        if (Input.GetAxisRaw("Vertical") > 0 && Time.time >= nextTimeAirbourneJump)
        {
            if (!isAirbourne) 
            {
                Jump(horizontalSpeed);
            }
            else
            {
                if (airbourneJumpsPerformed < airbourneJumpsCount)
                {
                    airbourneJumpsPerformed++;
                    Jump(horizontalSpeed);
                }
            }

            nextTimeAirbourneJump = Time.time + airbourneJumpCooldown;
        }

        RotateToMouseUpdate();

        animator.SetFloat("Speed", Mathf.Abs(horizontalSpeed));
        animator.SetFloat("Vert", verticalSpeed);

        UpdateDebugCanvas();

        // nie wiem czy to dobry pomys³, na razie dzia³a
        if (player.position.z != 0)
        {
            player.transform.position = new Vector3(player.position.x, player.position.y, 0);
        }

        Vector3 v = player.linearVelocity;
        v.x = Mathf.Clamp(v.x, -15f, 15f);
        player.linearVelocity = v;
    }
    public void ExplosionKnockback(Vector3 explosionPos, float force)
    {
        Vector3 rawDir = (player.position - explosionPos);
        bool explosionBelow = rawDir.y > 0f;

        Vector3 dir = rawDir.normalized;

        float fallSpeed = player.linearVelocity.y;
        float verticalComp = 0f;

        if (explosionBelow && fallSpeed < 0f)
        {
            verticalComp = -fallSpeed * 0.4f;
            verticalComp = Mathf.Min(verticalComp, force * 0.3f);
        }

        Vector3 impulse = dir * force;

        if (explosionBelow)
        {
            impulse.y += verticalComp;
        }

        player.AddForce(impulse, ForceMode.Impulse);

        if (explosionBelow)
        {
            float minUpVelocity = 8f;
            if (player.linearVelocity.y < minUpVelocity)
            {
                player.linearVelocity = new Vector3(player.linearVelocity.x, minUpVelocity);
            }
        }
    }
    private void Jump(float hs)
    {
        player.linearVelocity = new Vector3(hs, 10);
        FindAnyObjectByType<AudioManager>().Play("Jump");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentGroundContactInfo = true;
            isAirbourne = false;
            airbourneJumpsPerformed = 0;
            nextTimeAirbourneJump = Time.time;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentGroundContactInfo = false;
            isAirbourne = true;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("KnockbackSource"))
        {
            GameObject explosionSource = collision.gameObject;
            ExplosionKnockback(explosionSource.transform.position, rocketJumpForce);
        }

        if (collision.gameObject.CompareTag("Death") && playerAlive)
        {
            KillPlayer();
        }
    }
    private void KillPlayer()
    {
        //inputEnabled = false;
        //playerAlive = false;
        //player.linearVelocity = Vector2.zero;
        //animator.SetBool("IsDead", true);

        player.transform.position = new Vector3(130f, -5.3f, 0.06f);
    }
    private void RotateToMouseUpdate()
    {
        mousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 rotation = mousePos - player.transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        weaponContainer.transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
    private void UpdateDebugCanvas()
    {
        velocityDisplay.text = $"Horizonal velocity : {Mathf.Abs(player.linearVelocity.x)}";
        groundContactDisplay.text = $"Ground contact : {currentGroundContactInfo}";
        airbourneJumpsDisplay.text = $"AirJumps : {airbourneJumpsPerformed}/{airbourneJumpsCount}";
    }
}

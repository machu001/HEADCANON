using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private AudioManager au;
    [SerializeField] public GameObject explosion;
    public bool hasExploded = false;
    public float speed = 20f;
    public float force = 40f;
    public Rigidbody rb;

    private void Awake()
    {
        au = FindAnyObjectByType<AudioManager>();
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Physics.IgnoreCollision(gameObject.GetComponent<BoxCollider>(), player.gameObject.GetComponent<CapsuleCollider>());
    }

    private void Update()
    {
        rb.linearVelocity = transform.right * speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody itself = gameObject.GetComponent<Rigidbody>();
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Weapon"))
        {
            Physics.IgnoreCollision(gameObject.GetComponent<CapsuleCollider>(), collision.gameObject.GetComponent<CapsuleCollider>());

        }
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Weapon"))
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            if (au)
            {
                au.Play("Explosion");
            }
            Destroy(this.gameObject);
        }
    }
}
